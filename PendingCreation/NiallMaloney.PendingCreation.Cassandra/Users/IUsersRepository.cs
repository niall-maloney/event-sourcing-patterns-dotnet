namespace NiallMaloney.PendingCreation.Cassandra.Users;

public interface IUsersRepository
{
    public Task AddUser(UserRow user);
    public Task UpdateUser(UserRow user);
    Task<UserRow?> GetUser(string userId);
    Task<IEnumerable<UserRow>> SearchUsers(string? status);
}
