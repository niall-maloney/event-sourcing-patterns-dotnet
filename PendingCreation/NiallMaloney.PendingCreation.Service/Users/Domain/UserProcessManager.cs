using MediatR;
using NiallMaloney.EventSourcing;
using NiallMaloney.EventSourcing.Subscriptions;
using NiallMaloney.PendingCreation.Cassandra.Users;
using NiallMaloney.PendingCreation.Service.Users.Commands;
using NiallMaloney.PendingCreation.Service.Users.Events;

namespace NiallMaloney.PendingCreation.Service.Users.Domain;

[SubscriberName("UserProcessManager")]
[Subscription("$ce-pending_creation.user")]
public class UserProcessManager : SubscriberBase
{
    private readonly IMediator _mediator;
    private readonly IDuplicateUserTrackingRepository _repository;

    public UserProcessManager(IMediator mediator, IDuplicateUserTrackingRepository repository)
    {
        _mediator = mediator;
        _repository = repository;

        When<UserRequested>(Handle);
        When<UserAccepted>(Handle);
        When<UserRejected>(Handle);
    }

    private async Task Handle(UserRequested evnt, EventMetadata metadata)
    {
        var alreadyExists = await _repository.HasUserWithEmailAddress(evnt.EmailAddress);

        await _repository.AddUser(new UserDataRow
        {
            UserId = evnt.UserId,
            EmailAddress = evnt.EmailAddress
        });

        if (alreadyExists)
        {
            await _mediator.Send(new RejectUser(evnt.UserId, "User with same email address already exists."));
        }
        else
        {
            await _mediator.Send(new AcceptUser(evnt.UserId));
        }
    }

    private Task Handle(UserAccepted evnt, EventMetadata metadata)
    {
        return Task.CompletedTask;
    }

    private Task Handle(UserRejected evnt, EventMetadata metadata)
    {
        //TODO delete from duplicate tracking
        return Task.CompletedTask;
    }
}
