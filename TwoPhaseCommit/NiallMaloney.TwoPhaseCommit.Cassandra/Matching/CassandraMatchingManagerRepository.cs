using Cassandra;
using Cassandra.Data.Linq;
using Cassandra.Mapping;

namespace NiallMaloney.TwoPhaseCommit.Cassandra.Matching;

public class CassandraMatchingManagerRepository : IMatchingManagerRepository
{
    private readonly ISession _session;
    private readonly Mapper _mapper;

    public CassandraMatchingManagerRepository()
    {
        var cluster = Cluster.Builder().AddContactPoint("localhost").WithPort(9042).Build();
        _session = cluster.Connect(Configuration.Keyspace);
        _mapper = new Mapper(_session);
        CreateTables();
    }

    private void CreateTables()
    {
        //CREATE TABLE IF NOT EXISTS matching_managers ( matchingId text PRIMARY KEY, paymentId text, expectationId text, iban text, status text, amount decimal, reference text, version varint
        _session.Execute(
            "CREATE TABLE IF NOT EXISTS matching_managers ( matchingId text PRIMARY KEY, paymentId text, expectationId text, iban text, status text, amount decimal, reference text, version varint)"
        );
    }

    public Task AddManager(MatchingManagerRow manager) => _mapper.InsertAsync(manager);

    public Task UpdateManager(MatchingManagerRow manager) => _mapper.UpdateAsync(manager);

    public Task<MatchingManagerRow?> GetManager(string matchingId) =>
        _mapper.SingleOrDefaultAsync<MatchingManagerRow?>(
            "SELECT * FROM matching_managers where matchingId=?",
            matchingId
        );

    public Task<IEnumerable<MatchingManagerRow>> SearchManagers(
        string? matchingId = null,
        string? paymentId = null,
        string? expectationId = null,
        string? status = null
    )
    {
        CqlQuery<MatchingManagerRow> managers = new Table<MatchingManagerRow>(_session);

        if (!string.IsNullOrEmpty(matchingId))
        {
            managers = managers.Where(b => b.MatchingId == matchingId).AllowFiltering();
        }
        if (!string.IsNullOrEmpty(paymentId))
        {
            managers = managers.Where(b => b.PaymentId == paymentId);
        }
        if (!string.IsNullOrEmpty(expectationId))
        {
            managers = managers.Where(b => b.ExpectationId == expectationId).AllowFiltering();
        }

        return managers.ExecuteAsync();
    }
}
