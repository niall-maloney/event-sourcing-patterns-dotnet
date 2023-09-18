using Cassandra;
using Cassandra.Data.Linq;
using Cassandra.Mapping;

namespace NiallMaloney.AggregateProcessManager.Cassandra;

public class CassandraExpectationsRepository : IExpectationsRepository
{
    private readonly ISession _session;
    private readonly Mapper _mapper;

    public CassandraExpectationsRepository()
    {
        var cluster = Cluster.Builder().AddContactPoint("localhost").WithPort(9042).Build();
        _session = cluster.Connect(Configuration.Keyspace);
        _mapper = new Mapper(_session);
        CreateTables();
    }

    private void CreateTables()
    {
        //CREATE TABLE IF NOT EXISTS expectations ( expectationId text PRIMARY KEY, iban text, status text, amount decimal, reference text, version varint
        _session.Execute(
            "CREATE TABLE IF NOT EXISTS expectations ( expectationId text PRIMARY KEY, iban text, status text, amount decimal, reference text, version varint)");
    }

    public Task AddExpectation(ExpectationRow expectation) => _mapper.InsertAsync(expectation);

    public Task UpdateExpectation(ExpectationRow expectation) => _mapper.UpdateAsync(expectation);

    public Task<ExpectationRow?> GetExpectation(string expectationId) =>
        _mapper.SingleOrDefaultAsync<ExpectationRow?>("SELECT * FROM expectations where expectationId=?",
            expectationId);

    public Task<IEnumerable<ExpectationRow>> SearchExpectations(
        string? expectationId = null,
        string? iban = null,
        decimal? amount = null,
        string? reference = null,
        string? status = null)
    {
        CqlQuery<ExpectationRow> expectations = new Table<ExpectationRow>(_session);

        if (!string.IsNullOrEmpty(expectationId))
        {
            expectations = expectations.Where(b => b.ExpectationId == expectationId);
        }
        if (!string.IsNullOrEmpty(iban))
        {
            expectations = expectations.Where(b => b.Iban == iban).AllowFiltering();
        }
        if (amount.HasValue)
        {
            expectations = expectations.Where(b => b.Amount == amount.Value).AllowFiltering();
        }
        if (!string.IsNullOrEmpty(reference))
        {
            expectations = expectations.Where(b => b.Reference == reference).AllowFiltering();
        }
        if (!string.IsNullOrEmpty(status))
        {
            expectations = expectations.Where(b => b.Status == status).AllowFiltering();
        }

        return expectations.ExecuteAsync();
    }
}
