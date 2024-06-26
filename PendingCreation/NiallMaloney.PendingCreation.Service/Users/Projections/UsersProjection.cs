using NiallMaloney.EventSourcing;
using NiallMaloney.EventSourcing.Projections;
using NiallMaloney.EventSourcing.Subscriptions;
using NiallMaloney.PendingCreation.Cassandra.Users;
using NiallMaloney.PendingCreation.Service.Users.Events;

namespace NiallMaloney.PendingCreation.Service.Users.Projections;

[SubscriberName("UsersProjection")]
[Subscription("$ce-pending_creation.user")]
public class UsersProjection : Projection
{
    private readonly IUsersRepository _repository;

    public UsersProjection(IUsersRepository repository)
    {
        _repository = repository;

        When<UserRequested>(Handle);
        When<UserAccepted>(Handle);
        When<UserRejected>(Handle);
    }

    private async Task Handle(UserRequested evnt, EventMetadata metadata)
    {
        var user = await _repository.GetUser(evnt.UserId);
        if (user is not null)
        {
            return;
        }

        await _repository.AddUser(new UserRow
        {
            UserId = evnt.UserId,
            EmailAddress = evnt.EmailAddress,
            Forename = evnt.Forename,
            Surname = evnt.Surname,
            Status = "Pending",
            Version = metadata.StreamPosition,
        });
    }

    private async Task Handle(UserAccepted evnt, EventMetadata metadata)
    {
        var user = await _repository.GetUser(evnt.UserId);
        if (user is null || !TryUpdateVersion(user, metadata.StreamPosition, out user))
        {
            return;
        }

        user = user with { Status = "Active" };
        await _repository.UpdateUser(user);
    }

    private async Task Handle(UserRejected evnt, EventMetadata metadata)
    {
        var user = await _repository.GetUser(evnt.UserId);
        if (user is null || !TryUpdateVersion(user, metadata.StreamPosition, out user))
        {
            return;
        }

        user = user with { Status = "Rejected" };
        await _repository.UpdateUser(user);
    }

    private bool TryUpdateVersion(UserRow user, ulong newVersion, out UserRow newUser)
    {
        var expectedVersion = newVersion - 1;
        var actualVersion = user.Version;
        if (actualVersion >= newVersion)
        {
            newUser = user;
            return false;
        }

        if (actualVersion != expectedVersion)
        {
            throw new InvalidOperationException(
                $"Version mismatch, expected {expectedVersion} actual {actualVersion}"
            );
        }

        newUser = user with { Version = newVersion };
        return true;
    }
}