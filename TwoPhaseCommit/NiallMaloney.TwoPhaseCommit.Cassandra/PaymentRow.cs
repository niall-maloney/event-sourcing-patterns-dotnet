namespace NiallMaloney.TwoPhaseCommit.Cassandra;

public record PaymentRow
{
    public string PaymentId { get; init; }
    public string Iban { get; init; }
    public decimal Amount { get; init; }
    public string Reference { get; init; }
    public string Status { get; init; }
    public ulong Version { get; init; }
}
