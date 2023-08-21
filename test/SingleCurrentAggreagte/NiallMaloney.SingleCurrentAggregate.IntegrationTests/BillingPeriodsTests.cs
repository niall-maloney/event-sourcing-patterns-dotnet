using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Controllers.Models;
using NiallMaloney.SingleCurrentAggregate.Service.Customers.Controllers.Models;
using Polly;

namespace NiallMaloney.SingleCurrentAggregate.IntegrationTests;

public class BillingPeriodsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public BillingPeriodsTests(WebApplicationFactory<Program> app)
    {
        _client = app.CreateClient();
    }

    [Fact]
    public async Task When_Customer_Added_Then_Billing_Period_Is_Opened()
    {
        //Arrange
        const string expectedStatus = "Open";

        //Act
        var customerReference = await AddCustomer();
        var customerId = customerReference.CustomerId;
        var billingPeriod = await GetOrWaitForSingleOpenBillingPeriod(customerId);

        //Assert
        billingPeriod.Status.Should().Be(expectedStatus);
        billingPeriod.TotalAmount.Should().Be(0);
        billingPeriod.Version.Should().Be(0);
    }

    [Fact]
    public async Task Given_An_Open_Billing_Period_When_Adding_A_Charge_Then_Charge_Should_Be_Added()
    {
        //Arrange
        const decimal amount = 50m;
        const string expectedChargeStatus = "Charged";

        var customerReference = await AddCustomer();
        var customerId = customerReference.CustomerId;
        var billingPeriod = await GetOrWaitForSingleOpenBillingPeriod(customerId);
        var billingPeriodId = billingPeriod.Id!;

        //Act
        var chargeReference = await AddCharge(customerId, amount);
        var chargeId = chargeReference.ChargeId;

        //Assert
        var charge = await GetOrWaitForExpectedCharge(chargeId, expectedChargeStatus);
        charge.BillingPeriodId.Should().Be(billingPeriodId);
        charge.Amount.Should().Be(amount);
        charge.Status.Should().Be(expectedChargeStatus);

        billingPeriod = await GetBillingPeriod(billingPeriodId);
        billingPeriod.Should().NotBeNull();
        billingPeriod!.TotalAmount.Should().Be(amount);
    }

    [Fact]
    public async Task Given_A_Charge_When_Removing_Charge_Then_Charge_Should_Be_Removed()
    {
        //Arrange
        const decimal amount = 50m;
        const string expectedChargeStatus = "Removed";

        var customerReference = await AddCustomer();
        var customerId = customerReference.CustomerId;

        var billingPeriod = await GetOrWaitForSingleOpenBillingPeriod(customerId);
        var billingPeriodId = billingPeriod.Id!;

        var chargeReference = await AddCharge(customerId, amount);
        var chargeId = chargeReference.ChargeId;
        await GetOrWaitForExpectedCharge(chargeId, "Charged");

        //Act
        await RemoveCharge(chargeId);

        //Assert
        var charge = await GetOrWaitForExpectedCharge(chargeId, expectedChargeStatus);
        charge.BillingPeriodId.Should().Be(billingPeriodId);
        charge.Amount.Should().Be(amount);
        charge.Status.Should().Be(expectedChargeStatus);

        billingPeriod = await GetBillingPeriod(billingPeriodId);
        billingPeriod.Should().NotBeNull();
        billingPeriod!.TotalAmount.Should().Be(0);
    }

    [Fact]
    public async Task Given_An_Opening_Billing_Period_When_Closing_Then_A_New_Billing_Period_Is_Opened()
    {
        //Arrange
        var customerReference = await AddCustomer();
        var customerId = customerReference.CustomerId;

        var firstBillingPeriod = await GetOrWaitForSingleOpenBillingPeriod(customerId);
        var firstBillingPeriodId = firstBillingPeriod.Id!;

        //Act
        await CloseBillingPeriod(firstBillingPeriodId);
        firstBillingPeriod = await GetOrWaitForExpectedBillingPeriod(firstBillingPeriodId, "Closed");
        var secondBillingPeriod = await GetOrWaitForSingleOpenBillingPeriod(customerId);

        //Assert
        firstBillingPeriod.Status.Should().Be("Closed");
        firstBillingPeriod.CustomerId.Should().Be(customerId);
        firstBillingPeriod.TotalAmount.Should().Be(0);
        firstBillingPeriod.Version.Should().Be(1);

        secondBillingPeriod.Id.Should().NotBe(firstBillingPeriodId);
        secondBillingPeriod.Status.Should().Be("Open");
        secondBillingPeriod.CustomerId.Should().Be(customerId);
        secondBillingPeriod.TotalAmount.Should().Be(0);
        secondBillingPeriod.Version.Should().Be(0);
    }

    private async Task<BillingPeriod> GetOrWaitForSingleOpenBillingPeriod(string customerId)
    {
        var billingPeriods = await RetryUntil(
            async () => await SearchBillingPeriods(customerId, "Open"),
            b => b.Any());
        return billingPeriods.Single();
    }

    private async Task<BillingPeriod> GetOrWaitForExpectedBillingPeriod(
        string billingPeriodId,
        string? expectedStatus = null)
    {
        var billingPeriod = await RetryUntil(async () => await GetBillingPeriod(billingPeriodId),
            b => expectedStatus is null || b?.Status == expectedStatus);
        billingPeriod.Should().NotBeNull();
        return billingPeriod!;
    }

    private async Task<IEnumerable<BillingPeriod>> SearchBillingPeriods(string customerId, string status)
    {
        var getResponseMessage = await _client.GetAsync($"/billing-periods?customerId={customerId}&status={status}");
        getResponseMessage.EnsureSuccessStatusCode();
        return await getResponseMessage.Content.ReadFromJsonAsync<IEnumerable<BillingPeriod>>() ??
               throw new InvalidOperationException();
    }

    private async Task<BillingPeriod?> GetBillingPeriod(string billingPeriodId)
    {
        var getResponseMessage = await _client.GetAsync($"/billing-periods/{billingPeriodId}");
        if (getResponseMessage.IsSuccessStatusCode)
        {
            return await getResponseMessage.Content.ReadFromJsonAsync<BillingPeriod>();
        }
        if (getResponseMessage.StatusCode is HttpStatusCode.NotFound)
        {
            return null;
        }
        throw new HttpRequestException();
    }

    private async Task<CustomerReference> AddCustomer(bool assertSuccess = true)
    {
        var postResponseMessage = await _client.PostAsJsonAsync("/customers", new { });
        if (assertSuccess)
        {
            postResponseMessage.IsSuccessStatusCode.Should().BeTrue();
        }
        var reference = await postResponseMessage.Content.ReadFromJsonAsync<CustomerReference>();
        reference.Should().NotBeNull();
        return reference!;
    }

    private async Task CloseBillingPeriod(string billingPeriodId, bool assertSuccess = true)
    {
        var postResponseMessage =
            await _client.PostAsJsonAsync($"/billing-periods/{billingPeriodId}/actions/close", new { });
        if (assertSuccess)
        {
            postResponseMessage.IsSuccessStatusCode.Should().BeTrue();
        }
    }

    private async Task<Charge> GetOrWaitForExpectedCharge(
        string chargeId,
        string? expectedStatus = null)
    {
        var charge = await RetryUntil(async () => await GetCharge(chargeId),
            b => expectedStatus is null || b?.Status == expectedStatus);
        charge.Should().NotBeNull();
        return charge!;
    }

    private async Task<Charge?> GetCharge(string chargeId)
    {
        var getResponseMessage = await _client.GetAsync($"/charges/{chargeId}");
        if (getResponseMessage.IsSuccessStatusCode)
        {
            return await getResponseMessage.Content.ReadFromJsonAsync<Charge>();
        }
        if (getResponseMessage.StatusCode is HttpStatusCode.NotFound)
        {
            return null;
        }
        throw new HttpRequestException();
    }

    private async Task<ChargeReference> AddCharge(string customerId, decimal amount, bool assertSuccess = true)
    {
        var postResponseMessage =
            await _client.PostAsJsonAsync("/charges", new { CustomerId = customerId, Amount = amount });
        if (assertSuccess)
        {
            postResponseMessage.IsSuccessStatusCode.Should().BeTrue();
        }
        var reference = await postResponseMessage.Content.ReadFromJsonAsync<ChargeReference>();
        reference.Should().NotBeNull();
        return reference!;
    }

    private async Task RemoveCharge(string chargeId, bool assertSuccess = true)
    {
        var postResponseMessage = await _client.PostAsJsonAsync($"/charges/{chargeId}/actions/remove", new { });
        if (assertSuccess)
        {
            postResponseMessage.IsSuccessStatusCode.Should().BeTrue();
        }
    }

    private static async Task<T> RetryUntil<T>(
        Func<Task<T>> action,
        Func<T, bool> retryUntilPredicate,
        int retryCount = 50,
        int sleepDurationInMilliseconds = 100) =>
        await Policy.HandleResult<T>(r => !retryUntilPredicate.Invoke(r))
            .WaitAndRetryAsync(retryCount, _ => TimeSpan.FromMilliseconds(sleepDurationInMilliseconds))
            .ExecuteAsync(action);
}
