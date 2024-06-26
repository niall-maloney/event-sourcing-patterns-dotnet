namespace NiallMaloney.TwoPhaseCommit.Cassandra.Payments;

public interface IPaymentsRepository
{
    public Task AddPayment(PaymentRow payment);
    public Task UpdatePayment(PaymentRow payment);
    public Task<PaymentRow?> GetPayment(string paymentId);

    public Task<IEnumerable<PaymentRow>> SearchPayments(
        string? paymentId = null,
        string? iban = null,
        decimal? amount = null,
        string? reference = null,
        string? status = null
    );
}