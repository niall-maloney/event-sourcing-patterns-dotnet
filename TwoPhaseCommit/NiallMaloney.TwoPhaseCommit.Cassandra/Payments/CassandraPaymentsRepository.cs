using Cassandra;
using Cassandra.Data.Linq;
using Cassandra.Mapping;

namespace NiallMaloney.TwoPhaseCommit.Cassandra.Payments;

public class CassandraPaymentsRepository : IPaymentsRepository
{
    private readonly Mapper _mapper;
    private readonly ISession _session;

    public CassandraPaymentsRepository()
    {
        var cluster = Cluster.Builder().AddContactPoint("localhost").WithPort(9042).Build();
        _session = cluster.Connect(Configuration.Keyspace);
        _mapper = new Mapper(_session);
        CreateTables();
    }

    public Task AddPayment(PaymentRow payment)
    {
        return _mapper.InsertAsync(payment);
    }

    public Task UpdatePayment(PaymentRow payment)
    {
        return _mapper.UpdateAsync(payment);
    }

    public Task<PaymentRow?> GetPayment(string paymentId)
    {
        return _mapper.SingleOrDefaultAsync<PaymentRow?>(
            "SELECT * FROM payments where paymentId=?",
            paymentId
        );
    }

    public Task<IEnumerable<PaymentRow>> SearchPayments(
        string? paymentId = null,
        string? iban = null,
        decimal? amount = null,
        string? reference = null,
        string? status = null
    )
    {
        CqlQuery<PaymentRow> payments = new Table<PaymentRow>(_session);

        if (!string.IsNullOrEmpty(paymentId))
        {
            payments = payments.Where(b => b.PaymentId == paymentId);
        }

        if (!string.IsNullOrEmpty(iban))
        {
            payments = payments.Where(b => b.Iban == iban).AllowFiltering();
        }

        if (amount.HasValue)
        {
            payments = payments.Where(b => b.Amount == amount.Value).AllowFiltering();
        }

        if (!string.IsNullOrEmpty(reference))
        {
            payments = payments.Where(b => b.Reference == reference).AllowFiltering();
        }

        if (!string.IsNullOrEmpty(status))
        {
            payments = payments.Where(b => b.Status == status).AllowFiltering();
        }

        return payments.ExecuteAsync();
    }

    private void CreateTables()
    {
        //CREATE TABLE IF NOT EXISTS payments ( paymentId text PRIMARY KEY, iban text, status text, amount decimal, reference text, version varint
        _session.Execute(
            "CREATE TABLE IF NOT EXISTS payments ( paymentId text PRIMARY KEY, iban text, status text, amount decimal, reference text, version varint)"
        );
    }
}