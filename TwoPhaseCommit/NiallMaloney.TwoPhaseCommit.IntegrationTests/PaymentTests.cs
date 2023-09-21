using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc.Testing;
using NiallMaloney.TwoPhaseCommit.Service.Payments.Controllers.Models;
using Polly;

namespace NiallMaloney.TwoPhaseCommit.IntegrationTests;

public class PaymentTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public PaymentTests(WebApplicationFactory<Program> app)
    {
        _client = app.CreateClient();
    }

    [Fact]
    public async Task Given_A_Payment_Request_Then_Payment_Is_Received()
    {
        //Arrange
        var iban = "GB97BARC20031869128817";
        var amount = 10m;
        var reference = "REF";

        //Act
        var paymentRef = await ReceivePayment(iban, amount, reference);
        var paymentId = paymentRef.PaymentId;
        var payment = await GetOrWaitForExpectedPayment(paymentId, "Received");

        //Assert
        using var scope = new AssertionScope();

        payment.Id.Should().Be(paymentId);
        payment.Status.Should().Be("Received");
        payment.Amount.Should().Be(amount);
        payment.Reference.Should().Be(reference);
        payment.Version.Should().Be(0);
    }

    private async Task<Payment> GetOrWaitForExpectedPayment(
        string paymentId,
        string? expectedStatus = null
    )
    {
        var booking = await RetryUntil(
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

    private static async Task<T> RetryUntil<T>(
        Func<Task<T>> action,
        Func<T, bool> retryUntilPredicate,
        int retryCount = 50,
        int sleepDurationInMilliseconds = 100
    ) =>
        await Policy
            .HandleResult<T>(r => !retryUntilPredicate.Invoke(r))
            .WaitAndRetryAsync(
                retryCount,
                _ => TimeSpan.FromMilliseconds(sleepDurationInMilliseconds)
            )
            .ExecuteAsync(action);
}
