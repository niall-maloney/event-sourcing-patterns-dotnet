using Cassandra;
using Cassandra.Mapping;

namespace NiallMaloney.ProcessManager.Cassandra;

public class CassandraLedgersRepository : ILedgersRepository
{
    private readonly Mapper _mapper;
    private readonly ISession _session;

    public CassandraLedgersRepository()
    {
        var cluster = Cluster.Builder().AddContactPoint("localhost").WithPort(9042).Build();
        _session = cluster.Connect("process_manager");
        _mapper = new Mapper(_session);
        CreateTables();
    }

    public async Task<IEnumerable<LedgerRow>> GetLedgers() =>
        await _mapper.FetchAsync<LedgerRow>("SELECT * FROM ledgers");

    public async Task<LedgerRow?> GetLedger(string ledger) =>
        await _mapper.SingleOrDefaultAsync<LedgerRow?>("SELECT * FROM ledgers WHERE ledger=?", ledger);

    public async Task<decimal?> GetBalance(string ledger)
    {
        var r = await GetLedger(ledger);
        return r?.Amount;
    }

    public async Task<(bool, decimal)> UpdateBalance(string ledger, decimal updatedBalance, decimal? currentBalance)
    {
        Statement statement;
        if (currentBalance is null)
        {
            var query = "INSERT INTO ledgers (ledger, amount) VALUES (?, ?) IF NOT EXISTS";
            var prepared = await _session.PrepareAsync(query);
            statement = prepared.Bind(ledger, updatedBalance);
        }
        else
        {
            var query = "UPDATE ledgers SET amount=? WHERE ledger =? IF amount=?";
            var prepared = await _session.PrepareAsync(query);
            statement = prepared.Bind(updatedBalance, ledger, currentBalance);
        }
        var rs = await _session.ExecuteAsync(statement);
        var r = rs.Single().GetValue<bool>(0);
        if (r == false)
        {
            throw new InvalidOperationException("Unexpected amount");
        }
        return (true, updatedBalance);
    }

    private void CreateTables()
    {
        //CREATE TABLE IF NOT EXISTS process_manager.ledgers ( ledger text PRIMARY KEY, amount varint);
        _session.Execute(
            "CREATE TABLE IF NOT EXISTS ledgers ( ledger text PRIMARY KEY, amount varint)");
    }
}