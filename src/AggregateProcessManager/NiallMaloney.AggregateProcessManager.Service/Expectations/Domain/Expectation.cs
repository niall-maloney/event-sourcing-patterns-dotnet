using NiallMaloney.AggregateProcessManager.Service.Expectations.Events;
using NiallMaloney.EventSourcing.Aggregates;

namespace NiallMaloney.AggregateProcessManager.Service.Expectations.Domain;

[Category("aggregate_process_manager.expectation")]
public class Expectation : Aggregate
{
    private decimal Amount { get; set; } = 0;
    private string Payment { get; set; } = string.Empty;
    private string Iban { get; set; } = string.Empty;
    private string Reference { get; set; } = string.Empty;

    private bool _hasBeenReceived = false;
    private bool _hasBeenReserved = false;
    private bool _hasBeenMatched = false;


    public Expectation()
    {
        When<ExpectationCreated>(Apply);
        When<ExpectationMatching>(Apply);
        When<ExpectationMatched>(Apply);
    }

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
}
