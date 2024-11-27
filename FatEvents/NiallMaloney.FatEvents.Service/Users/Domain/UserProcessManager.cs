using MediatR;
using NiallMaloney.EventSourcing;
using NiallMaloney.EventSourcing.Subscriptions;
using NiallMaloney.FatEvents.Service.Users.Commands;
using NiallMaloney.FatEvents.Service.Users.Events;

namespace NiallMaloney.FatEvents.Service.Users.Domain;

[SubscriberName("UserProcessManager")]
[Subscription("$ce-fat_events.user", begin: CursorFromStream.End)]
public class UserProcessManager : SubscriberBase
{
    private readonly IMediator _mediator;

    public UserProcessManager(IMediator mediator)
    {
        _mediator = mediator;

        When<UserRequested>(Handle);
        When<UserAccepted>(Handle);
    }

    private async Task Handle(UserRequested evnt, EventMetadata metadata)
    {
        await _mediator.Send(new AcceptUser(evnt.UserId));
    }

    private Task Handle(UserAccepted evnt, EventMetadata metadata)
    {
        return Task.CompletedTask;
    }
}
