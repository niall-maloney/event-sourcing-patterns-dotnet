using MediatR;

namespace NiallMaloney.PendingCreation.Service.Users.Commands;

public record AcceptUser(string UserId) : IRequest;