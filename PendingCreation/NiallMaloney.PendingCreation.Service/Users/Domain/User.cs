using NiallMaloney.EventSourcing.Aggregates;
using NiallMaloney.PendingCreation.Service.Users.Events;

namespace NiallMaloney.PendingCreation.Service.Users.Domain;

[Category("pending_creation.user")]
public class User : Aggregate
{
    private bool _hasBeenAccepted;
    private bool _hasBeenRejected;

    private bool _hasBeenRequested;

    public User()
    {
        When<UserRequested>(Apply);
        When<UserAccepted>(Apply);
        When<UserRejected>(Apply);
    }

    public string Forename { get; private set; } = string.Empty;
    public string Surname { get; private set; } = string.Empty;
    public string EmailAddress { get; private set; } = string.Empty;

    public void Request(string emailAddress, string forename, string surname)
    {
        if (_hasBeenRequested || _hasBeenAccepted)
        {
            return;
        }

        RaiseEvent(new UserRequested(Id, emailAddress, forename, surname));
    }

    public void Accept()
    {
        if (_hasBeenAccepted)
        {
            return;
        }

        if (!_hasBeenRequested)
        {
            throw new Exception();
        }

        RaiseEvent(new UserAccepted(Id));
    }

    public void Reject(string reason)
    {
        if (_hasBeenRejected)
        {
            return;
        }

        if (_hasBeenAccepted)
        {
            throw new Exception();
        }

        if (!_hasBeenRequested)
        {
            throw new Exception();
        }

        RaiseEvent(new UserRejected(Id, reason));
    }

    private void Apply(UserRequested evnt)
    {
        _hasBeenRequested = true;
        EmailAddress = evnt.EmailAddress;
        Forename = evnt.Forename;
        Surname = evnt.Surname;
    }

    private void Apply(UserAccepted evnt)
    {
        _hasBeenAccepted = true;
    }

    private void Apply(UserRejected evnt)
    {
        _hasBeenRejected = true;
    }
}