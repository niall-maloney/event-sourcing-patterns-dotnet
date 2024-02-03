using NiallMaloney.EventSourcing;

namespace NiallMaloney.PendingCreation.Service.Users.Events;

[Event("pending_creation.user_accepted")]
public record UserAccepted(string UserId) : IEvent;
