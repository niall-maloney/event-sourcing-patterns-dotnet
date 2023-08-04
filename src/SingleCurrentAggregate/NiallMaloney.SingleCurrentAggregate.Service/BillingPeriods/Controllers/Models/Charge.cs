namespace NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Controllers.Models;

public record Charge(string Id, decimal Amount, string Status, ulong Version);