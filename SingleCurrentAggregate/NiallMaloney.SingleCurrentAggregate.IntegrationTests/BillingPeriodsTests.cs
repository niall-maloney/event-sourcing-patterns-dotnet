using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Controllers.Models;
using NiallMaloney.SingleCurrentAggregate.Service.Customers.Controllers.Models;
using Polly;
using Shouldly;

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
        billingPeriod.Status.ShouldBe(expectedStatus);
        billingPeriod.TotalAmount.ShouldBe(0);
        billingPeriod.Version.ShouldBe(0ul);
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
        charge.BillingPeriodId.ShouldBe(billingPeriodId);
        charge.Amount.ShouldBe(amount);
        charge.Status.ShouldBe(expectedChargeStatus);

        billingPeriod = await GetBillingPeriod(billingPeriodId);
        billingPeriod.ShouldNotBeNull();
        billingPeriod.TotalAmount.ShouldBe(amount);
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
        charge.BillingPeriodId.ShouldBe(billingPeriodId);
        charge.Amount.ShouldBe(amount);
        charge.Status.ShouldBe(expectedChargeStatus);

        billingPeriod = await GetBillingPeriod(billingPeriodId);
        billingPeriod.ShouldNotBeNull();
        billingPeriod.TotalAmount.ShouldBe(0);
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
        firstBillingPeriod = await GetOrWaitForExpectedBillingPeriod(
            firstBillingPeriodId,
            "Closed"
        );
        var secondBillingPeriod = await GetOrWaitForSingleOpenBillingPeriod(customerId);

        //Assert
        firstBillingPeriod.Status.ShouldBe("Closed");
        firstBillingPeriod.CustomerId.ShouldBe(customerId);
        firstBillingPeriod.TotalAmount.ShouldBe(0);
        firstBillingPeriod.Version.ShouldBe(1ul);

        secondBillingPeriod.Id.ShouldNotBe(firstBillingPeriodId);
        secondBillingPeriod.Status.ShouldBe("Open");
        secondBillingPeriod.CustomerId.ShouldBe(customerId);
        secondBillingPeriod.TotalAmount.ShouldBe(0);
        secondBillingPeriod.Version.ShouldBe(0ul);
    }

    private async Task<BillingPeriod> GetOrWaitForSingleOpenBillingPeriod(string customerId)
    {
        var billingPeriods = await RetryUntil(
            async () => await SearchBillingPeriods(customerId, "Open"),
            b => b.Any()
        );
        return billingPeriods.Single();
    }

    private async Task<BillingPeriod> GetOrWaitForExpectedBillingPeriod(
        string billingPeriodId,
        string? expectedStatus = null
    )
    {
        var billingPeriod = await RetryUntil(
            async () => await GetBillingPeriod(billingPeriodId),
            b => expectedStatus is null || b?.Status == expectedStatus
        );
        billingPeriod.ShouldNotBeNull();
        return billingPeriod;
    }

    private async Task<IEnumerable<BillingPeriod>> SearchBillingPeriods(
        string customerId,
        string status
    )
    {
        var getResponseMessage = await _client.GetAsync(
            $"/billing-periods?customerId={customerId}&status={status}"
        );
        getResponseMessage.EnsureSuccessStatusCode();
        return await getResponseMessage.Content.ReadFromJsonAsync<IEnumerable<BillingPeriod>>()
               ?? throw new InvalidOperationException();
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
            postResponseMessage.IsSuccessStatusCode.ShouldBeTrue();
        }

        var reference = await postResponseMessage.Content.ReadFromJsonAsync<CustomerReference>();
        reference.ShouldNotBeNull();
        return reference;
    }

    private async Task CloseBillingPeriod(string billingPeriodId, bool assertSuccess = true)
    {
        var postResponseMessage = await _client.PostAsJsonAsync(
            $"/billing-periods/{billingPeriodId}/actions/close",
            new { }
        );
        if (assertSuccess)
        {
            postResponseMessage.IsSuccessStatusCode.ShouldBeTrue();
        }
    }

    private async Task<Charge> GetOrWaitForExpectedCharge(
        string chargeId,
        string? expectedStatus = null
    )
    {
        var charge = await RetryUntil(
            async () => await GetCharge(chargeId),
            b => expectedStatus is null || b?.Status == expectedStatus
        );
        charge.ShouldNotBeNull();
        return charge;
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

    private async Task<ChargeReference> AddCharge(
        string customerId,
        decimal amount,
        bool assertSuccess = true
    )
    {
        var postResponseMessage = await _client.PostAsJsonAsync(
            "/charges",
            new { CustomerId = customerId, Amount = amount }
        );
        if (assertSuccess)
        {
            postResponseMessage.IsSuccessStatusCode.ShouldBeTrue();
        }

        var reference = await postResponseMessage.Content.ReadFromJsonAsync<ChargeReference>();
        reference.ShouldNotBeNull();
        return reference;
    }

    private async Task RemoveCharge(string chargeId, bool assertSuccess = true)
    {
        var postResponseMessage = await _client.PostAsJsonAsync(
            $"/charges/{chargeId}/actions/remove",
            new { }
        );
        if (assertSuccess)
        {
            postResponseMessage.IsSuccessStatusCode.ShouldBeTrue();
        }
    }

    private static async Task<T> RetryUntil<T>(
        Func<Task<T>> action,
        Func<T, bool> retryUntilPredicate,
        int retryCount = 50,
        int sleepDurationInMilliseconds = 100
    )
    {
        return await Policy
            .HandleResult<T>(r => !retryUntilPredicate.Invoke(r))
            .WaitAndRetryAsync(
                retryCount,
                _ => TimeSpan.FromMilliseconds(sleepDurationInMilliseconds)
            )
            .ExecuteAsync(action);
    }
}
