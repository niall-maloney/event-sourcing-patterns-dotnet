using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc.Testing;
using NiallMaloney.Shared.TestUtils;
using NiallMaloney.TwoPhaseCommit.Service.Payments.Controllers.Models;

namespace NiallMaloney.TwoPhaseCommit.IntegrationTests;

public class PaymentTests : TestBase<Program>
{
    public PaymentTests(WebApplicationFactory<Program> app) : base(app) { }

    [Fact]
    public async Task Given_A_Payment_Request_Then_Payment_Is_Received()
    {
        //Arrange
        var iban = "GB97BARC20031869128817";
        var amount = 10m;
        var reference = "REF";
        var expectedStatus = "Received";

        //Act
        var paymentRef = await ReceivePayment(iban, amount, reference);
        var paymentId = paymentRef.PaymentId;
        var payment = await GetOrWaitForExpectedPayment(paymentId, expectedStatus);

        //Assert
        using var scope = new AssertionScope();

        payment.Id.Should().Be(paymentId);
        payment.Status.Should().Be(expectedStatus);
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
        var getResponseMessage = await Client.GetAsync($"/payments/{paymentId}");
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
        var postResponseMessage = await Client.PostAsJsonAsync(
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
}
