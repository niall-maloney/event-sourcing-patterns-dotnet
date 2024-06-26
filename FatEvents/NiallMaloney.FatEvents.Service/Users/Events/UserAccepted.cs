using NiallMaloney.EventSourcing;

namespace NiallMaloney.FatEvents.Service.Users.Events;

[Event("fat_events.user_accepted")]
public record UserAccepted(string UserId, string EmailAddress, string Forename, string Surname) : IEvent;