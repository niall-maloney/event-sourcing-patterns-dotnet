using MediatR;

namespace NiallMaloney.FatEvents.Service.Users.Commands;

public record AcceptUser(string UserId) : IRequest;