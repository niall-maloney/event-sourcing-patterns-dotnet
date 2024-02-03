using MediatR;

namespace NiallMaloney.PendingCreation.Service.Users.Commands;

public record AddUser(string UserId, string EmailAddress, string Forename, string Surname) : IRequest;
