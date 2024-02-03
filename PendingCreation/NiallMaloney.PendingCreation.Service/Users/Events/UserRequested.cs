using NiallMaloney.EventSourcing;

namespace NiallMaloney.PendingCreation.Service.Users.Events;

[Event("pending_creation.user_requested")]
public record UserRequested(string UserId, string EmailAddress, string Forename, string Surname) : IEvent;
