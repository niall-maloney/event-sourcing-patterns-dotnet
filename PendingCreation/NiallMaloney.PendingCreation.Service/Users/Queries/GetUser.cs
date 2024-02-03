using MediatR;
using NiallMaloney.PendingCreation.Cassandra;
using NiallMaloney.PendingCreation.Cassandra.Users;

namespace NiallMaloney.PendingCreation.Service.Users.Queries;

public record GetUser(string UserId) : IRequest<UserRow?>;
