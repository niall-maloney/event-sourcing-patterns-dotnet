using MediatR;

namespace NiallMaloney.FatEvents.Service.Users.Commands;

public record AddUser(string UserId, string EmailAddress, string Forename, string Surname) : IRequest;