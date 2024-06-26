using MediatR;
using NiallMaloney.FatEvents.Cassandra.Users;

namespace NiallMaloney.FatEvents.Service.Users.Queries;

public record GetUser(string UserId) : IRequest<UserRow?>;