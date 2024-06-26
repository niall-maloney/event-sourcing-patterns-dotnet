using NiallMaloney.EventSourcing.Aggregates;
using NiallMaloney.TwoPhaseCommit.Service.Payments.Events;

namespace NiallMaloney.TwoPhaseCommit.Service.Payments.Domain;

[Category("two_phase_commit.payment")]
public class Payment : Aggregate
{
    private bool _hasBeenMatched;

    private bool _hasBeenReceived;
    private bool _hasBeenReserved;

    public Payment()
    {
        When<PaymentReceived>(Apply);
        When<PaymentMatching>(Apply);
        When<PaymentReleased>(Apply);
        When<PaymentMatched>(Apply);
        When<PaymentMatchRejected>(Apply);
    }

    private decimal Amount { get; set; }
    private string ExpectationId { get; set; } = string.Empty;
    private string Iban { get; set; } = string.Empty;
    private string MatchingId { get; set; } = string.Empty;
    private string Reference { get; set; } = string.Empty;

    public void Receive(string iban, decimal amount, string reference)
    {
        if (_hasBeenReceived)
        {
            return;
        }

        RaiseEvent(new PaymentReceived(Id, iban, amount, reference));
    }

    public void Reserve(string matchingId)
    {
        if (_hasBeenReserved || _hasBeenMatched)
        {
            RaiseEvent(new PaymentMatchRejected(Id, matchingId));
        }
        else
        {
            RaiseEvent(new PaymentMatching(Id, matchingId));
        }
    }

    public void Release(string matchingId)
    {
        if (_hasBeenReserved)
        {
            RaiseEvent(new PaymentReleased(Id, matchingId));
        }
    }

    public void Match(string matchingId, string expectationId)
    {
        if (matchingId != MatchingId)
        {
            throw new InvalidOperationException();
        }

        if (!_hasBeenReserved)
        {
            throw new InvalidOperationException();
        }

        if (_hasBeenMatched)
        {
            return;
        }

        RaiseEvent(new PaymentMatched(Id, expectationId, matchingId));
    }

    private void Apply(PaymentReceived evnt)
    {
        _hasBeenReceived = true;
        Amount = evnt.Amount;
        Iban = evnt.Iban;
        Reference = evnt.Reference;
    }

    private void Apply(PaymentMatching evnt)
    {
        _hasBeenReserved = true;
        MatchingId = evnt.MatchingId;
    }

    private void Apply(PaymentReleased evnt)
    {
        _hasBeenReserved = false;
        MatchingId = string.Empty;
    }

    private void Apply(PaymentMatched evnt)
    {
        _hasBeenMatched = true;
        ExpectationId = evnt.ExpectationId;
    }

    private void Apply(PaymentMatchRejected evnt)
    {
    }
}