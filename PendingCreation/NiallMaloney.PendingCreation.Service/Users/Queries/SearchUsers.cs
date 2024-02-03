using MediatR;
using NiallMaloney.PendingCreation.Cassandra;
using NiallMaloney.PendingCreation.Cassandra.Users;

namespace NiallMaloney.PendingCreation.Service.Users.Queries;

public record SearchUsers(string? Status = null) : IRequest<IEnumerable<UserRow>>;
