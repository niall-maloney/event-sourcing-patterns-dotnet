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

    public async Task<(decimal?, decimal?)> GetBalance(string ledger)
    {
        var r = await GetLedger(ledger);
        return (r?.PendingAmount, r?.CommittedAmount);
    }

    public async Task UpdateBalance(
        string ledger,
        decimal updatedPendingBalance,
        decimal? currentPendingBalance,
        decimal updatedCommittedBalance,
        decimal? currentCommittedBalance)
    {
        Statement statement;
        if (currentCommittedBalance is null && currentPendingBalance is null)
        {
            var query = "INSERT INTO ledgers (ledger, pendingAmount, committedAmount) VALUES (?, ?, ?) IF NOT EXISTS";
            var prepared = await _session.PrepareAsync(query);
            statement = prepared.Bind(ledger, updatedPendingBalance, updatedCommittedBalance);
        }
        else
        {
            var query =
                "UPDATE ledgers SET pendingAmount=?, committedAmount=? WHERE ledger =? IF pendingAmount=? AND committedAmount=?";
            var prepared = await _session.PrepareAsync(query);
            statement = prepared.Bind(updatedPendingBalance, updatedCommittedBalance, ledger, currentPendingBalance,
                currentCommittedBalance);
        }
        var rs = await _session.ExecuteAsync(statement);
        var r = rs.Single().GetValue<bool>(0);
        if (r == false)
        {
            throw new InvalidOperationException("Unexpected amount");
        }
    }

    public async Task UpdateCommittedBalance(string ledger, decimal updatedBalance, decimal? currentBalance)
    {
        Statement statement;
        if (currentBalance is null)
        {
            var query = "INSERT INTO ledgers (ledger, pendingAmount, committedAmount) VALUES (?, ?, ?) IF NOT EXISTS";
            var prepared = await _session.PrepareAsync(query);
            statement = prepared.Bind(ledger, 0, updatedBalance);
        }
        else
        {
            var query =
                "UPDATE ledgers SET committedAmount=? WHERE ledger =? IF committedAmount=?";
            var prepared = await _session.PrepareAsync(query);
            statement = prepared.Bind(updatedBalance, ledger, currentBalance);
        }
        var rs = await _session.ExecuteAsync(statement);
        var r = rs.Single().GetValue<bool>(0);
        if (r == false)
        {
            throw new InvalidOperationException("Unexpected committed amount");
        }
    }

    public async Task UpdatePendingBalance(string ledger, decimal updatedBalance, decimal? currentBalance)
    {
        Statement statement;
        if (currentBalance is null)
        {
            var query = "INSERT INTO ledgers (ledger, pendingAmount, committedAmount) VALUES (?, ?, ?) IF NOT EXISTS";
            var prepared = await _session.PrepareAsync(query);
            statement = prepared.Bind(ledger, updatedBalance, 0);
        }
        else
        {
            var query =
                "UPDATE ledgers SET pendingAmount=? WHERE ledger =? IF pendingAmount=?";
            var prepared = await _session.PrepareAsync(query);
            statement = prepared.Bind(updatedBalance, ledger, currentBalance);
        }
        var rs = await _session.ExecuteAsync(statement);
        var r = rs.Single().GetValue<bool>(0);
        if (r == false)
        {
            throw new InvalidOperationException("Unexpected pending amount");
        }
    }

    private void CreateTables()
    {
        //CREATE TABLE IF NOT EXISTS process_manager.ledgers ( ledger text PRIMARY KEY, pendingAmount decimal, committedAmount decimal);
        _session.Execute(
            "CREATE TABLE IF NOT EXISTS ledgers ( ledger text PRIMARY KEY, pendingAmount decimal, committedAmount decimal)");
    }
}
