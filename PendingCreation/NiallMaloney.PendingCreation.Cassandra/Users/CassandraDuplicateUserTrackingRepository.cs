using Cassandra;
using Cassandra.Data.Linq;
using Cassandra.Mapping;

namespace NiallMaloney.PendingCreation.Cassandra.Users;

public class CassandraDuplicateUserTrackingRepository : IDuplicateUserTrackingRepository
{
    private readonly ISession _session;
    private readonly Mapper _mapper;

    public CassandraDuplicateUserTrackingRepository()
    {
        var cluster = Cluster.Builder().AddContactPoint("localhost").WithPort(9042).Build();
        _session = cluster.Connect(Configuration.Keyspace);
        _mapper = new Mapper(_session);
        CreateTables();
    }

    private void CreateTables()
    {
        //CREATE TABLE IF NOT EXISTS duplicateUserTracking ( userId text PRIMARY KEY, emailAddress text)
        _session.Execute(
            "CREATE TABLE IF NOT EXISTS duplicateUserTracking ( userId text PRIMARY KEY, emailAddress text)"
        );
    }

    public async Task<bool> HasUserWithEmailAddress(string emailAddress)
    {
        CqlQuery<UserDataRow> table = new Table<UserDataRow>(_session);
        var query = table.Where(u => u.EmailAddress == emailAddress).AllowFiltering().Count();
        var count = await query.ExecuteAsync();
        return count > 0;
    }

    public Task AddUser(UserDataRow userDataRow) => _mapper.InsertAsync(userDataRow);
}

public interface IDuplicateUserTrackingRepository
{
    Task<bool> HasUserWithEmailAddress(string emailAddress);
    Task AddUser(UserDataRow userDataRow);
}

public record UserDataRow
{
    public string UserId { get; init; }
    public string EmailAddress { get; init; }
}
