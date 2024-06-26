using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc.Testing;
using NiallMaloney.ProcessManager.Cassandra;
using NiallMaloney.ProcessManager.Service;
using NiallMaloney.ProcessManager.Service.Ledgers.Controllers.Models;
using Polly;

namespace NiallMaloney.ProcessManager.IntegrationTests;

public class LedgersTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public LedgersTest(WebApplicationFactory<Program> app)
    {
        _client = app.CreateClient();
    }

    [Fact]
    public async Task Given_A_Booking_Request_When_Committed_Then_Balance_Is_Updated()
    {
        //Arrange
        var ledger = Guid.NewGuid().ToString();
        var amount = 10m;

        //Act
        var reference = await RequestBooking(ledger, amount);
        var bookingId = reference.BookingId;
        var booking = await GetOrWaitForExpectedBooking(bookingId, "Committed");
        var balance = await GetBalance(ledger);

        //Assert
        using var scope = new AssertionScope();

        booking.Id.Should().Be(bookingId);
        booking.Status.Should().Be("Committed");
        booking.Ledger.Should().Be(ledger);
        booking.Amount.Should().Be(amount);
        booking.Version.Should().Be(1);

        balance.Ledger.Should().Be(ledger);
        balance.CommittedAmount.Should().Be(10);
        balance.PendingAmount.Should().Be(0);
    }

    [Fact]
    public async Task Given_A_Balance_When_Requesting_A_Negative_Booking_Larger_Than_Balance_Then_Booking_Is_Rejected()
    {
        //Arrange
        var ledger = Guid.NewGuid().ToString();
        var amount = -100m;

        //Act
        _ = await RequestBooking(ledger, 99);
        var reference = await RequestBooking(ledger, amount);
        var bookingId = reference.BookingId;
        var booking = await GetOrWaitForExpectedBooking(bookingId, "Rejected");
        var balance = await GetBalance(ledger);

        //Assert
        using var scope = new AssertionScope();

        booking.Id.Should().Be(bookingId);
        booking.Status.Should().Be("Rejected");
        booking.Ledger.Should().Be(ledger);
        booking.Amount.Should().Be(amount);
        booking.Version.Should().Be(1);

        balance.Ledger.Should().Be(ledger);
        balance.CommittedAmount.Should().Be(99);
        balance.PendingAmount.Should().Be(0);
    }

    private async Task<BookingReference> RequestBooking(
        string ledger = "john:gbp",
        decimal amount = 100,
        bool assertSuccess = true
    )
    {
        var postResponseMessage = await _client.PostAsJsonAsync(
            "/bookings",
            new { Ledger = ledger, Amount = amount }
        );
        if (assertSuccess)
        {
            postResponseMessage.IsSuccessStatusCode.Should().BeTrue();
        }

        var reference = await postResponseMessage.Content.ReadFromJsonAsync<BookingReference>();
        reference.Should().NotBeNull();
        return reference!;
    }

    private async Task<Booking> GetOrWaitForExpectedBooking(
        string bookingId,
        string? expectedStatus = null
    )
    {
        var booking = await RetryUntil(
            async () => await GetBooking(bookingId),
            b => expectedStatus is null || b?.Status == expectedStatus
        );
        booking.Should().NotBeNull();
        return booking!;
    }

    private async Task<Booking?> GetBooking(string bookingId)
    {
        var getResponseMessage = await _client.GetAsync($"/bookings/{bookingId}");
        if (getResponseMessage.IsSuccessStatusCode)
        {
            return await getResponseMessage.Content.ReadFromJsonAsync<Booking>();
        }

        if (getResponseMessage.StatusCode is HttpStatusCode.NotFound)
        {
            return null;
        }

        throw new HttpRequestException();
    }

    private async Task<LedgerRow> GetBalance(string ledger)
    {
        var balance = await _client.GetFromJsonAsync<LedgerRow?>($"/balances/{ledger}");
        balance.Should().NotBeNull();
        return balance!;
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