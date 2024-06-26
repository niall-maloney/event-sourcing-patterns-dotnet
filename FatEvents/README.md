# Fat Events

The fat event pattern is all about capturing a lot of details in a single event to make a system more efficient and
easier to maintain. Instead of spreading information across multiple small events, you bundle related data together into
a "fat" event. This way, each event contains all the context needed for that specific change, which reduces the need to
fetch and combine multiple events later on.

```csharp

[Event("pending_creation.user_requested")]
public record UserRequested(string UserId, string EmailAddress, string Forename, string Surname) : IEvent;

[Event("pending_creation.user_accepted")]
public record UserAccepted(string UserId, string EmailAddress, string Forename, string Surname) : IEvent;
```

Here, UserAccepted is a fat event because it includes all the necessary details about the user beyond what is needed.
This reduces the complexity of reconstructing the state of the user later.
