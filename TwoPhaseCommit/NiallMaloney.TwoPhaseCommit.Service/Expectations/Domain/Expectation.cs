using NiallMaloney.EventSourcing.Aggregates;
using NiallMaloney.TwoPhaseCommit.Service.Expectations.Events;

namespace NiallMaloney.TwoPhaseCommit.Service.Expectations.Domain;

[Category("two_phase_commit.expectation")]
public class Expectation : Aggregate
{
    private bool _hasBeenMatched;

    private bool _hasBeenReceived;
    private bool _hasBeenReserved;

    public Expectation()
    {
        When<ExpectationCreated>(Apply);
        When<ExpectationMatching>(Apply);
        When<ExpectationMatched>(Apply);
        When<ExpectationMatchRejected>(Apply);
    }

    private decimal Amount { get; set; }
    private string Payment { get; set; } = string.Empty;
    private string Iban { get; set; } = string.Empty;
    private string Reference { get; set; } = string.Empty;

    public void Receive(string iban, decimal amount, string reference)
    {
        if (_hasBeenReceived)
        {
            return;
        }

        RaiseEvent(new ExpectationCreated(Id, iban, amount, reference));
    }

    public void Reserve(string matchingId)
    {
        if (_hasBeenReserved || _hasBeenMatched)
        {
            RaiseEvent(new ExpectationMatchRejected(Id, matchingId));
        }
        else
        {
            RaiseEvent(new ExpectationMatching(Id, matchingId));
        }
    }

    public void Match(string matchingId, string paymentId)
    {
        if (_hasBeenMatched)
        {
            return;
        }

        if (!_hasBeenReserved)
        {
            throw new InvalidOperationException();
        }

        //todo: assert matchingId

        RaiseEvent(new ExpectationMatched(Id, paymentId, matchingId));
    }

    private void Apply(ExpectationCreated evnt)
    {
        _hasBeenReceived = true;
        Amount = evnt.Amount;
        Iban = evnt.Iban;
        Reference = evnt.Reference;
    }

    private void Apply(ExpectationMatching evnt)
    {
        _hasBeenReserved = true;
    }

    private void Apply(ExpectationMatched evnt)
    {
        _hasBeenMatched = true;
        Payment = evnt.PaymentId;
    }

    private void Apply(ExpectationMatchRejected evnt)
    {
    }
}