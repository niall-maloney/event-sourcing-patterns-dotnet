using NiallMaloney.EventSourcing;

namespace NiallMaloney.PendingCreation.Service.Users.Events;

[Event("pending_creation.user_rejected")]
public record UserRejected(string UserId, string Reason) : IEvent;