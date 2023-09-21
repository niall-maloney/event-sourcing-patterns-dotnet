using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc.Testing;
using NiallMaloney.Shared.TestUtils;
using NiallMaloney.TwoPhaseCommit.Service.Expectations.Controllers.Models;

namespace NiallMaloney.TwoPhaseCommit.IntegrationTests;

public class ExpectationTests : TestBase<Program>
{
    public ExpectationTests(WebApplicationFactory<Program> app) : base(app) { }

    [Fact]
    public async Task Given_A_Expectation_Request_Then_Expectation_Is_Created()
    {
        //Arrange
        var iban = "GB97BARC20031869128817";
        var amount = 10m;
        var reference = "REF";
        var expectedStatus = "Created";

        //Act
        var expectationRef = await ReceiveExpectation(iban, amount, reference);
        var expectationId = expectationRef.ExpectationId;
        var expectation = await GetOrWaitForExpectedExpectation(expectationId, expectedStatus);

        //Assert
        using var scope = new AssertionScope();

        expectation.Id.Should().Be(expectationId);
        expectation.Status.Should().Be(expectedStatus);
        expectation.Amount.Should().Be(amount);
        expectation.Reference.Should().Be(reference);
        expectation.Version.Should().Be(0);
    }

    private async Task<Expectation> GetOrWaitForExpectedExpectation(
        string expectationId,
        string? expectedStatus = null
    )
    {
        var booking = await RetryUntil(
            async () => await GetExpectation(expectationId),
            b => expectedStatus is null || b?.Status == expectedStatus
        );
        booking.Should().NotBeNull();
        return booking!;
    }

    private async Task<Expectation?> GetExpectation(string expectationId)
    {
        var getResponseMessage = await Client.GetAsync($"/expectations/{expectationId}");
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

    private async Task<ExpectationReference> ReceiveExpectation(
        string iban,
        decimal amount,
        string reference,
        bool assertSuccess = true
    )
    {
        var postResponseMessage = await Client.PostAsJsonAsync(
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
}
