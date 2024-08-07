using NiallMaloney.PendingCreation.Cassandra.Users;

namespace NiallMaloney.PendingCreation.Service.Users.Controllers.Models;

public record User(
    string? Id,
    string? EmailAddress,
    string? Forename,
    string? Surname,
    string? Status,
    ulong Version
)
{
    public static User Map(UserRow r)
    {
        return new User(r.UserId, r.EmailAddress, r.Forename, r.Surname, r.Status, r.Version);
    }

    public static IEnumerable<User> Map(IEnumerable<UserRow> rs)
    {
        return rs.Select(
            r => new User(r.UserId, r.EmailAddress, r.Forename, r.Surname, r.Status, r.Version)
        );
    }
}