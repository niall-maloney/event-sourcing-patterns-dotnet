using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc.Testing;
using NiallMaloney.Shared.TestUtils;
using NiallMaloney.TwoPhaseCommit.Service.Expectations.Controllers.Models;
using NiallMaloney.TwoPhaseCommit.Service.Matching.Controllers.Models;
using NiallMaloney.TwoPhaseCommit.Service.Payments.Controllers.Models;

namespace NiallMaloney.TwoPhaseCommit.IntegrationTests;

public class MatchingTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public MatchingTests(WebApplicationFactory<Program> app)
    {
        _client = app.CreateClient();
    }
    
    [Fact]
    public async Task Given_An_Expectation_And_Payment_When_Request_To_Match_Then_Matched()
    {
        //Arrange
        var iban = "GB97BARC20031869128817";
        var amount = 10m;
        var reference = "REF";

        var expectationRef = await CreateExpectation(iban, amount, reference);
        var expectationId = expectationRef.ExpectationId;
        await GetOrWaitForExpectedExpectation(expectationId, "Created");

        var paymentRef = await ReceivePayment(iban, amount, reference);
        var paymentId = paymentRef.PaymentId;
        await GetOrWaitForExpectedPayment(paymentId, "'Received");

        //Act
        var matchingRef = await BeginMatching(paymentId, expectationId, iban, amount, reference);
        var payment = await GetOrWaitForExpectedPayment(paymentId, "Matched");
        var expectation = await GetOrWaitForExpectedExpectation(expectationId, "Matched");

        //Assert
        using var scope = new AssertionScope();

        expectation.Id.Should().Be(expectationId);
        expectation.Status.Should().Be("Matched");

        payment.Id.Should().Be(paymentId);
        payment.Status.Should().Be("Matched");
    }

    private async Task<MatchingReference> BeginMatching(
        string paymentId,
        string expectationId,
        string iban,
        decimal amount,
        string reference,
        bool assertSuccess = true
    )
    {
        var postResponseMessage = await _client.PostAsJsonAsync(
            "/matching-managers",
            new
            {
                PaymentId = paymentId,
                ExpectationId = expectationId,
                Iban = iban,
                Amount = amount,
                Reference = reference
            }
        );
        if (assertSuccess)
        {
            postResponseMessage.IsSuccessStatusCode.Should().BeTrue();
        }
        var matchingRef = await postResponseMessage.Content.ReadFromJsonAsync<MatchingReference>();
        matchingRef.Should().NotBeNull();
        return matchingRef!;
    }

    #region Expectation Helpers

    private async Task<Expectation> GetOrWaitForExpectedExpectation(
        string expectationId,
        string? expectedStatus = null
    )
    {
        var booking = await TestHelpers.RetryUntil(
            async () => await GetExpectation(expectationId),
            b => expectedStatus is null || b?.Status == expectedStatus
        );
        booking.Should().NotBeNull();
        return booking!;
    }

    private async Task<Expectation?> GetExpectation(string expectationId)
    {
        var getResponseMessage = await _client.GetAsync($"/expectations/{expectationId}");
        if (getResponseMessage.IsSuccessStatusCode)
        {
            return await getResponseMessage.Content.ReadFromJsonAsync<Expectation>();
        }
        if (getResponseMessage.StatusCode is HttpStatusCode.NotFound)
        {
            return null;
        }
        throw new HttpRequestException();
    }

    private async Task<ExpectationReference> CreateExpectation(
        string iban,
        decimal amount,
        string reference,
        bool assertSuccess = true
    )
    {
        var postResponseMessage = await _client.PostAsJsonAsync(
            "/expectations",
            new
            {
                Iban = iban,
                Amount = amount,
                Reference = reference
            }
        );
        if (assertSuccess)
        {
            postResponseMessage.IsSuccessStatusCode.Should().BeTrue();
        }
        var expectationRef = await postResponseMessage.Content.ReadFromJsonAsync<ExpectationReference>();
        expectationRef.Should().NotBeNull();
        return expectationRef!;
    }

    #endregion

    #region Payment Helpers

    private async Task<Payment> GetOrWaitForExpectedPayment(
        string paymentId,
        string? expectedStatus = null
    )
    {
        var booking = await TestHelpers.RetryUntil(
            async () => await GetPayment(paymentId),
            b => expectedStatus is null || b?.Status == expectedStatus
        );
        booking.Should().NotBeNull();
        return booking!;
    }

    private async Task<Payment?> GetPayment(string paymentId)
    {
        var getResponseMessage = await _client.GetAsync($"/payments/{paymentId}");
        if (getResponseMessage.IsSuccessStatusCode)
        {
            return await getResponseMessage.Content.ReadFromJsonAsync<Payment>();
        }
        if (getResponseMessage.StatusCode is HttpStatusCode.NotFound)
        {
            return null;
        }
        throw new HttpRequestException();
    }

    private async Task<PaymentReference> ReceivePayment(
        string iban,
        decimal amount,
        string reference,
        bool assertSuccess = true
    )
    {
        var postResponseMessage = await _client.PostAsJsonAsync(
            "/payments",
            new
            {
                Iban = iban,
                Amount = amount,
                Reference = reference
            }
        );
        if (assertSuccess)
        {
            postResponseMessage.IsSuccessStatusCode.Should().BeTrue();
        }
        var paymentRef = await postResponseMessage.Content.ReadFromJsonAsync<PaymentReference>();
        paymentRef.Should().NotBeNull();
        return paymentRef!;
    }

    #endregion
}
