using System.Net;
using System.Net.Http.Json;
using EventStore.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NiallMaloney.Shared.TestUtils;
using NiallMaloney.TwoPhaseCommit.Service.Expectations.Controllers.Models;
using NiallMaloney.TwoPhaseCommit.Service.Matching.Controllers.Models;
using NiallMaloney.TwoPhaseCommit.Service.Payments.Controllers.Models;
using NiallMaloney.TwoPhaseCommit.Service.Payments.Events;
using Shouldly;
using EventStoreClient = NiallMaloney.EventSourcing.EventStoreClient;

namespace NiallMaloney.TwoPhaseCommit.IntegrationTests;

public class MatchingTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly EventStoreClient _eventStore;

    public MatchingTests(WebApplicationFactory<Program> app)
    {
        _client = app.CreateClient();
        _eventStore = app.Services.GetRequiredService<EventStoreClient>();
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
        var matchingId = matchingRef.MatchingId;
        var manager = await GetOrWaitForExpectedManager(matchingId, "Completed");
        var payment = await GetOrWaitForExpectedPayment(paymentId);
        var expectation = await GetOrWaitForExpectedExpectation(expectationId);

        //Assert
        manager.ShouldSatisfyAllConditions(
            () => manager.Id.ShouldBe(matchingId),
            () => manager.PaymentId.ShouldBe(paymentId),
            () => manager.ExpectationId.ShouldBe(expectationId),
            () => manager.Status.ShouldBe("Completed"));

        payment.ShouldSatisfyAllConditions(
            () => payment.Id.ShouldBe(paymentId),
            () => payment.Status.ShouldBe("Matched"));

        expectation.ShouldSatisfyAllConditions(
            () => expectation.Id.ShouldBe(expectationId),
            () => expectation.Status.ShouldBe("Matched"));
    }

    [Fact]
    public async Task Given_An_Already_Matched_Payment_When_Request_To_Match_Then_Rejected()
    {
        //Arrange
        var iban = "GB97BARC20031869128817";
        var amount = 10m;
        var reference = "REF";

        var expectation1Ref = await CreateExpectation(iban, amount, reference);
        var expectation1Id = expectation1Ref.ExpectationId;
        await GetOrWaitForExpectedExpectation(expectation1Id, "Created");

        var paymentRef = await ReceivePayment(iban, amount, reference);
        var paymentId = paymentRef.PaymentId;
        await GetOrWaitForExpectedPayment(paymentId, "'Received");

        var matching1Ref = await BeginMatching(paymentId, expectation1Id, iban, amount, reference);
        var matching1Id = matching1Ref.MatchingId;
        await GetOrWaitForExpectedManager(matching1Id, "Completed");

        var expectation2Ref = await CreateExpectation(iban, amount, reference);
        var expectation2Id = expectation2Ref.ExpectationId;
        await GetOrWaitForExpectedExpectation(expectation2Id, "Created");

        //Act
        var matching2Ref = await BeginMatching(paymentId, expectation2Id, iban, amount, reference);
        var matching2Id = matching2Ref.MatchingId;
        var manager2 = await GetOrWaitForExpectedManager(matching2Id, "Failed");
        var payment = await GetOrWaitForExpectedPayment(paymentId);
        var expectation2 = await GetOrWaitForExpectedExpectation(expectation2Id);

        //Assert
        manager2.ShouldSatisfyAllConditions(
            () => manager2.Id.ShouldBe(matching2Id),
            () => manager2.PaymentId.ShouldBe(paymentId),
            () => manager2.ExpectationId.ShouldBe(expectation2Id),
            () => manager2.Status.ShouldBe("Failed"));

        payment.ShouldSatisfyAllConditions(
            () => payment.Id.ShouldBe(paymentId),
            () => payment.Status.ShouldBe("Matched"));

        expectation2.ShouldSatisfyAllConditions(
            () => expectation2.Id.ShouldBe(expectation2Id),
            () => expectation2.Status.ShouldBe("Created"));
    }

    [Fact]
    public async Task Given_An_Already_Matched_Expectation_When_Request_To_Match_Then_Rejected()
    {
        //Arrange
        var iban = "GB97BARC20031869128817";
        var amount = 10m;
        var reference = "REF";

        var expectationRef = await CreateExpectation(iban, amount, reference);
        var expectationId = expectationRef.ExpectationId;
        await GetOrWaitForExpectedExpectation(expectationId, "Created");

        var payment1Ref = await ReceivePayment(iban, amount, reference);
        var payment1Id = payment1Ref.PaymentId;
        await GetOrWaitForExpectedPayment(payment1Id, "'Received");

        var matching1Ref = await BeginMatching(payment1Id, expectationId, iban, amount, reference);
        var matching1Id = matching1Ref.MatchingId;
        await GetOrWaitForExpectedManager(matching1Id, "Completed");

        var payment2Ref = await ReceivePayment(iban, amount, reference);
        var payment2Id = payment2Ref.PaymentId;
        await GetOrWaitForExpectedPayment(payment2Id, "Received");

        //Act
        var matching2Ref = await BeginMatching(payment2Id, expectationId, iban, amount, reference);
        var matching2Id = matching2Ref.MatchingId;
        var manager2 = await GetOrWaitForExpectedManager(matching2Id, "Failed");
        var payment2 = await GetOrWaitForExpectedPayment(payment2Id);
        var expectation = await GetOrWaitForExpectedExpectation(expectationId);

        //Assert
        manager2.ShouldSatisfyAllConditions(
            () => manager2.Id.ShouldBe(matching2Id),
            () => manager2.PaymentId.ShouldBe(payment2Id),
            () => manager2.ExpectationId.ShouldBe(expectationId),
            () => manager2.Status.ShouldBe("Failed"));

        payment2.ShouldSatisfyAllConditions(
            () => payment2.Id.ShouldBe(payment2Id),
            () => payment2.Status.ShouldBe("Received"));

        expectation.ShouldSatisfyAllConditions(
            () => expectation.Id.ShouldBe(expectationId),
            () => expectation.Status.ShouldBe("Matched"));

        //Assert that Payment is released
        var eventEnvelopes = await _eventStore.ReadStreamAsync($"two_phase_commit.payment-{payment2Id}",
            StreamPosition.End, Direction.Backwards, 1);
        var envelope = (await eventEnvelopes!.ToListAsync()).Single();

        var lastEvent = envelope.Event as PaymentReleased;
        lastEvent.ShouldNotBeNull();
        lastEvent.PaymentId.ShouldBe(payment2Id);
        lastEvent.MatchingId.ShouldBe(matching2Id);
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
                Reference = reference,
            }
        );
        if (assertSuccess)
        {
            postResponseMessage.IsSuccessStatusCode.ShouldBeTrue();
        }

        var matchingRef = await postResponseMessage.Content.ReadFromJsonAsync<MatchingReference>();
        matchingRef.ShouldNotBeNull();
        return matchingRef;
    }

    private async Task<MatchingManager> GetOrWaitForExpectedManager(
        string matchingId,
        string? expectedStatus = null
    )
    {
        var manager = await TestHelpers.RetryUntil(
            async () => await GetManager(matchingId),
            b => expectedStatus is null || b?.Status == expectedStatus
        );
        manager.ShouldNotBeNull();
        return manager;
    }

    private async Task<MatchingManager?> GetManager(string matchingId)
    {
        var getResponseMessage = await _client.GetAsync($"/matching-managers/{matchingId}");
        if (getResponseMessage.IsSuccessStatusCode)
        {
            return await getResponseMessage.Content.ReadFromJsonAsync<MatchingManager>();
        }

        if (getResponseMessage.StatusCode is HttpStatusCode.NotFound)
        {
            return null;
        }

        throw new HttpRequestException();
    }


    #region Expectation Helpers

    private async Task<Expectation> GetOrWaitForExpectedExpectation(
        string expectationId,
        string? expectedStatus = null
    )
    {
        var expectation = await TestHelpers.RetryUntil(
            async () => await GetExpectation(expectationId),
            b => expectedStatus is null || b?.Status == expectedStatus
        );
        expectation.ShouldNotBeNull();
        return expectation;
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
                Reference = reference,
            }
        );
        if (assertSuccess)
        {
            postResponseMessage.IsSuccessStatusCode.ShouldBeTrue();
        }

        var expectationRef = await postResponseMessage.Content.ReadFromJsonAsync<ExpectationReference>();
        expectationRef.ShouldNotBeNull();
        return expectationRef;
    }

    #endregion

    #region Payment Helpers

    private async Task<Payment> GetOrWaitForExpectedPayment(
        string paymentId,
        string? expectedStatus = null
    )
    {
        var payment = await TestHelpers.RetryUntil(
            async () => await GetPayment(paymentId),
            b => expectedStatus is null || b?.Status == expectedStatus
        );
        payment.ShouldNotBeNull();
        return payment;
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
                Reference = reference,
            }
        );
        if (assertSuccess)
        {
            postResponseMessage.IsSuccessStatusCode.ShouldBeTrue();
        }

        var paymentRef = await postResponseMessage.Content.ReadFromJsonAsync<PaymentReference>();
        paymentRef.ShouldNotBeNull();
        return paymentRef;
    }

    #endregion
}
