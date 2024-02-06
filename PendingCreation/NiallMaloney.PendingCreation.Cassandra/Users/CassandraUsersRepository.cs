using Cassandra;
using Cassandra.Data.Linq;
using Cassandra.Mapping;

namespace NiallMaloney.PendingCreation.Cassandra.Users;

public class CassandraUsersRepository : IUsersRepository
{
    private readonly ISession _session;
    private readonly Mapper _mapper;

    public CassandraUsersRepository()
    {
        var cluster = Cluster.Builder().AddContactPoint("localhost").WithPort(9042).Build();
        _session = cluster.Connect(Configuration.Keyspace);
        _mapper = new Mapper(_session);
        CreateTables();
    }

    private void CreateTables()
    {
        //CREATE TABLE IF NOT EXISTS users ( userId text PRIMARY KEY, emailAddress text, forename text, surname text, status text, version varint)
        _session.Execute(
            "CREATE TABLE IF NOT EXISTS users ( userId text PRIMARY KEY, emailAddress text, forename text, surname text, status text, version varint)"
        );
    }

    public Task AddUser(UserRow user) => _mapper.InsertAsync(user);

    public Task UpdateUser(UserRow user) => _mapper.UpdateAsync(user);

    public Task<UserRow?> GetUser(string userId) =>
        _mapper.SingleOrDefaultAsync<UserRow?>(
            "SELECT * FROM users where userId=?",
            userId
        );

    public Task<IEnumerable<UserRow>> SearchUsers(string? emailAddress, string? status)
    {
        CqlQuery<UserRow> users = new Table<UserRow>(_session);

        if (!string.IsNullOrEmpty(emailAddress))
        {
            users = users.Where(b => b.EmailAddress == emailAddress).AllowFiltering();
        }
        if (!string.IsNullOrEmpty(status))
        {
            users = users.Where(b => b.Status == status).AllowFiltering();
        }

        return users.ExecuteAsync();
    }
}
