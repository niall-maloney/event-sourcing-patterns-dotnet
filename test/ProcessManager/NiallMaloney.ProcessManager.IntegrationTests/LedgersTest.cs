using System.Net.Http.Json;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc.Testing;
using NiallMaloney.ProcessManager.Cassandra;
using NiallMaloney.ProcessManager.Service.Ledgers.Controllers.Models;

namespace NiallMaloney.ProcessManager.IntegrationTests;

public class LedgersTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public LedgersTest(WebApplicationFactory<Program> applicationFactory)
    {
        _client = applicationFactory.CreateClient();
    }

    [Fact]
    public async Task CanBook()
    {
        //Arrange
        var ledger = Guid.NewGuid().ToString();
        var amount = 10m;

        //Act
        var reference = await RequestBooking(ledger, amount);
        await WaitForCommittedBooking(reference.BookingId);
        var balance = await GetBalance(ledger);

        //Assert
        using var scope = new AssertionScope();
        balance.Ledger.Should().Be(ledger);
        balance.CommittedAmount.Should().Be(10);
        balance.PendingAmount.Should().Be(0);
    }

    [Fact]
    public async Task BelowZeroBalanceRejects()
    {
        //Arrange
        var ledger = Guid.NewGuid().ToString();
        var amount = -100m;

        //Act
        _ = await RequestBooking(ledger, 1);
        var reference = await RequestBooking(ledger, amount);
        await WaitForCommittedBooking(reference.BookingId);
        var balance = await GetBalance(ledger);

        //Assert
        using var scope = new AssertionScope();
        balance.Ledger.Should().Be(ledger);
        balance.CommittedAmount.Should().Be(1);
        balance.PendingAmount.Should().Be(0);
    }

    private async Task<BookingReference> RequestBooking(
        string ledger = "john:gbp",
        decimal amount = 100,
        bool assertSuccess = true)
    {
        var postResponseMessage = await _client.PostAsJsonAsync("/bookings", new { Ledger = ledger, Amount = amount });
        if (assertSuccess)
        {
            postResponseMessage.IsSuccessStatusCode.Should().BeTrue();
        }
        var reference = await postResponseMessage.Content.ReadFromJsonAsync<BookingReference>();
        reference.Should().NotBeNull();
        return reference!;
    }

    private static Task WaitForCommittedBooking(string bookingId) => Task.Delay(1000);

    private async Task<LedgerRow> GetBalance(string ledger)
    {
        var balance = await _client.GetFromJsonAsync<LedgerRow?>($"/balances/{ledger}");
        balance.Should().NotBeNull();
        return balance!;
    }
}