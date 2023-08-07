using System.Numerics;
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

    public async Task UpdateBalance(
        string ledger,
        decimal newPendingBalance,
        decimal newCommittedBalance,
        ulong newStreamPosition,
        ulong? lastStreamPosition)
    {
        Statement statement;
        if (lastStreamPosition is null)
        {
            var query =
                "INSERT INTO ledgers (ledger, pendingAmount, committedAmount, lastStreamPosition) VALUES (?, ?, ?, ?) IF NOT EXISTS";
            var prepared = await _session.PrepareAsync(query);
            statement = prepared.Bind(ledger, newPendingBalance, newCommittedBalance, (BigInteger)newStreamPosition);
        }
        else
        {
            var query =
                "UPDATE ledgers SET pendingAmount=?, committedAmount=?, lastStreamPosition=? WHERE ledger =? IF lastStreamPosition=?";
            var prepared = await _session.PrepareAsync(query);
            statement = prepared.Bind(newPendingBalance, newCommittedBalance, (BigInteger)newStreamPosition, ledger,
                (BigInteger)lastStreamPosition);
        }
        var rs = await _session.ExecuteAsync(statement);
        var r = rs.Single().GetValue<bool>(0);
        if (r == false)
        {
            throw new InvalidOperationException("Unexpected amount");
        }
    }

    public async Task UpdateCommittedBalance(
        string ledger,
        decimal newBalance,
        ulong newStreamPosition,
        ulong? lastStreamPosition)
    {
        Statement statement;
        if (lastStreamPosition is null)
        {
            var query =
                "INSERT INTO ledgers (ledger, pendingAmount, committedAmount, lastStreamPosition) VALUES (?, ?, ?, ?) IF NOT EXISTS";
            var prepared = await _session.PrepareAsync(query);
            statement = prepared.Bind(ledger, 0m, newBalance, (BigInteger)newStreamPosition);
        }
        else
        {
            var query =
                "UPDATE ledgers SET committedAmount=?, lastStreamPosition=? WHERE ledger =? IF lastStreamPosition=?";
            var prepared = await _session.PrepareAsync(query);
            statement = prepared.Bind(newBalance, (BigInteger)newStreamPosition, ledger,
                (BigInteger)lastStreamPosition);
        }
        var rs = await _session.ExecuteAsync(statement);
        var r = rs.Single().GetValue<bool>(0);
        if (r == false)
        {
            throw new InvalidOperationException("Unexpected committed amount");
        }
    }

    public async Task UpdatePendingBalance(
        string ledger,
        decimal newBalance,
        ulong newStreamPosition,
        ulong? lastStreamPosition)
    {
        Statement statement;
        if (lastStreamPosition is null)
        {
            var query =
                "INSERT INTO ledgers (ledger, pendingAmount, committedAmount, lastStreamPosition) VALUES (?, ?, ?, ?) IF NOT EXISTS";
            var prepared = await _session.PrepareAsync(query);
            statement = prepared.Bind(ledger, newBalance, 0m, (BigInteger)newStreamPosition);
        }
        else
        {
            var query =
                "UPDATE ledgers SET pendingAmount=?, lastStreamPosition=? WHERE ledger =? IF lastStreamPosition=?";
            var prepared = await _session.PrepareAsync(query);
            statement = prepared.Bind(newBalance, (BigInteger)newStreamPosition, ledger,
                (BigInteger)lastStreamPosition);
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
        //CREATE TABLE IF NOT EXISTS process_manager.ledgers ( ledger text PRIMARY KEY, pendingAmount decimal, committedAmount decimal, lastStreamPosition varint );
        _session.Execute(
            "CREATE TABLE IF NOT EXISTS ledgers ( ledger text PRIMARY KEY, pendingAmount decimal, committedAmount decimal, lastStreamPosition varint )");
    }
}