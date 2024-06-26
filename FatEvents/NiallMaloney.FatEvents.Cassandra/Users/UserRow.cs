namespace NiallMaloney.FatEvents.Cassandra.Users;

public record UserRow
{
    public string? UserId { get; init; }
    public string? EmailAddress { get; init; }
    public string? Forename { get; init; }
    public string? Surname { get; init; }
    public string? Status { get; init; }
    public ulong Version { get; init; }
}