using MediatR;

namespace NiallMaloney.PendingCreation.Service.Users.Commands;

public record RejectUser(string UserId, string Reason) : IRequest;