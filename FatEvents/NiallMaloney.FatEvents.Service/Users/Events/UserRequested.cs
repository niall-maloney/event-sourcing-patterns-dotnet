using NiallMaloney.EventSourcing;

namespace NiallMaloney.FatEvents.Service.Users.Events;

[Event("fat_events.user_requested")]
public record UserRequested(string UserId, string EmailAddress, string Forename, string Surname) : IEvent;