using NiallMaloney.EventSourcing.Aggregates;
using NiallMaloney.FatEvents.Service.Users.Events;

namespace NiallMaloney.FatEvents.Service.Users.Domain;

[Category("fat_events.user")]
public class User : Aggregate
{
    private bool _hasBeenAccepted;
    private bool _hasBeenRequested;

    public User()
    {
        When<UserRequested>(Apply);
        When<UserAccepted>(Apply);
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

        RaiseEvent(new UserAccepted(Id, EmailAddress, Forename, Surname));
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
}