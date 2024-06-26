using MediatR;
using NiallMaloney.FatEvents.Cassandra.Users;

namespace NiallMaloney.FatEvents.Service.Users.Queries;

public record SearchUsers(string? EmailAddress = null, string? Status = null) : IRequest<IEnumerable<UserRow>>;