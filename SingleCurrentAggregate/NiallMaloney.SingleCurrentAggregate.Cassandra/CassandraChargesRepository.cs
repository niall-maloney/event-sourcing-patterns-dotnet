using Cassandra;
using Cassandra.Data.Linq;
using Cassandra.Mapping;

namespace NiallMaloney.SingleCurrentAggregate.Cassandra;

public class CassandraChargesRepository : IChargesRepository
{
    private readonly Mapper _mapper;
    private readonly ISession _session;

    public CassandraChargesRepository()
    {
        var cluster = Cluster.Builder().AddContactPoint("localhost").WithPort(9042).Build();
        _session = cluster.Connect(Configuration.Keyspace);
        _mapper = new Mapper(_session);
        CreateTables();
    }

    public Task<ChargeRow> GetCharge(string chargeId)
    {
        return _mapper.SingleOrDefaultAsync<ChargeRow>("SELECT * FROM charges where chargeId=?", chargeId);
    }

    public Task<IEnumerable<ChargeRow>> SearchCharges(
        string? chargeId = null,
        string? billingPeriodId = null,
        string? status = null
    )
    {
        CqlQuery<ChargeRow> charges = new Table<ChargeRow>(_session);

        if (!string.IsNullOrEmpty(chargeId))
        {
            charges = charges.Where(b => b.ChargeId == chargeId);
        }

        if (!string.IsNullOrEmpty(billingPeriodId))
        {
            charges = charges.Where(b => b.BillingPeriodId == billingPeriodId).AllowFiltering();
        }

        if (!string.IsNullOrEmpty(status))
        {
            charges = charges.Where(b => b.Status == status).AllowFiltering();
        }

        return charges.ExecuteAsync();
    }

    public Task AddCharge(ChargeRow charge)
    {
        return _mapper.InsertAsync(charge);
    }

    public Task UpdateCharge(ChargeRow charge)
    {
        return _mapper.UpdateAsync(charge);
    }

    private void CreateTables()
    {
        //CREATE TABLE IF NOT EXISTS charges ( chargeId text PRIMARY KEY, billingPeriodId text, status text, amount decimal, version varint);
        _session.Execute(
            "CREATE TABLE IF NOT EXISTS charges ( chargeId text PRIMARY KEY, billingPeriodId text, status text, amount decimal, version varint)"
        );
    }
}