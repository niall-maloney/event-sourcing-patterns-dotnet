using Cassandra;
using Cassandra.Data.Linq;
using Cassandra.Mapping;

namespace NiallMaloney.SingleCurrentAggregate.Cassandra;

public class CassandraBillingPeriodsRepository : IBillingPeriodsRepository
{
    private readonly Mapper _mapper;
    private readonly ISession _session;

    public CassandraBillingPeriodsRepository()
    {
        var cluster = Cluster.Builder().AddContactPoint("localhost").WithPort(9042).Build();
        _session = cluster.Connect("single_current_aggregate");
        _mapper = new Mapper(_session);
        CreateTables();
    }

    public Task<BillingPeriodRow?> GetBillingPeriod(string billingPeriodId) =>
        _mapper.SingleOrDefaultAsync<BillingPeriodRow?>("SELECT * FROM billing_periods where billingPeriodId=?",
            billingPeriodId);

    public Task<IEnumerable<BillingPeriodRow>> SearchBillingPeriods(
        string? billingPeriodId = null,
        string? customerId = null,
        string? status = null)
    {
        CqlQuery<BillingPeriodRow> billingPeriods = new Table<BillingPeriodRow>(_session);

        if (!string.IsNullOrEmpty(billingPeriodId))
        {
            billingPeriods = billingPeriods.Where(b => b.BillingPeriodId == billingPeriodId);
        }
        if (!string.IsNullOrEmpty(customerId))
        {
            billingPeriods = billingPeriods.Where(b => b.CustomerId == customerId).AllowFiltering();
        }
        if (!string.IsNullOrEmpty(status))
        {
            billingPeriods = billingPeriods.Where(b => b.Status == status).AllowFiltering();
        }

        return billingPeriods.ExecuteAsync();
    }

    public Task AddBillingPeriod(BillingPeriodRow billingPeriod) => _mapper.InsertAsync(billingPeriod);

    public Task UpdateBillingPeriod(BillingPeriodRow billingPeriod) => _mapper.UpdateAsync(billingPeriod);

    private void CreateTables()
    {
        //CREATE TABLE IF NOT EXISTS billing_periods ( billingPeriodId text PRIMARY KEY, customerId text, status text, totalAmount decimal, version varint);
        _session.Execute(
            "CREATE TABLE IF NOT EXISTS billing_periods ( billingPeriodId text PRIMARY KEY, customerId text, status text, totalAmount decimal, version varint)");
    }
}
