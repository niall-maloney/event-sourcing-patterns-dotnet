using NiallMaloney.EventSourcing.Aggregates;
using NiallMaloney.TwoPhaseCommit.Service.Matching.Events;

namespace NiallMaloney.TwoPhaseCommit.Service.Matching.Domain;

[Category("two_phase_commit.matching_manager")]
public class MatchingManager : Aggregate
{
    private decimal? _amount;
    private Expectation? _expectation;
    private bool _hasFinished;
    private bool _hasStarted;
    private Payment? _payment;
    private bool _wasSuccessful;

    public MatchingManager()
    {
        When<MatchingStarted>(Apply);
        When<PaymentReserving>(Apply);
        When<PaymentReserved>(Apply);
        When<PaymentReservationRejected>(Apply);
        When<ExpectationReserving>(Apply);
        When<ExpectationReserved>(Apply);
        When<ExpectationReservationRejected>(Apply);
        When<PaymentMatchApplying>(Apply);
        When<PaymentMatchApplied>(Apply);
        When<ExpectationMatchApplying>(Apply);
        When<ExpectationMatchApplied>(Apply);
        When<MatchingCompleted>(Apply);
        When<MatchingFailed>(Apply);
    }

    public Expectation Expectation => _expectation ?? throw new InvalidOperationException();
    public Payment Payment => _payment ?? throw new InvalidOperationException();

    public void Begin(
        string expectationId,
        string paymentId,
        string iban,
        decimal amount,
        string reference
    )
    {
        RaiseEvent(new MatchingStarted(Id, expectationId, paymentId, iban, amount, reference));
    }

    public void ReservePayment()
    {
        RaiseEvent(new PaymentReserving(Id, _payment!.Id));
    }

    public void AcknowledgePaymentReserved()
    {
        RaiseEvent(new PaymentReserved(Id, _payment!.Id));
    }

    public void AcknowledgePaymentReservationRejected()
    {
        RaiseEvent(new PaymentReservationRejected(Id, _payment!.Id));
    }

    public void ReserveExpectation()
    {
        RaiseEvent(new ExpectationReserving(Id, _expectation!.Id));
    }

    public void AcknowledgeExpectationReserved()
    {
        RaiseEvent(new ExpectationReserved(Id, _expectation!.Id));
    }

    public void AcknowledgeExpectationReservationRejected()
    {
        RaiseEvent(new ExpectationReservationRejected(Id, _expectation!.Id, _payment!.Id));
    }

    public void ApplyPaymentMatch()
    {
        RaiseEvent(new PaymentMatchApplying(Id, _payment!.Id, _expectation!.Id));
    }

    public void AcknowledgePaymentApplied()
    {
        RaiseEvent(new PaymentMatchApplied(Id, _payment!.Id, _expectation!.Id));
    }

    public void ApplyExpectationMatch()
    {
        RaiseEvent(new ExpectationMatchApplying(Id, _expectation!.Id, _payment!.Id));
    }

    public void AcknowledgeExpectationApplied()
    {
        RaiseEvent(new ExpectationMatchApplied(Id, _expectation!.Id, _payment!.Id));
    }

    public void Complete()
    {
        RaiseEvent(new MatchingCompleted(Id));
    }

    public void Fail(string reason)
    {
        RaiseEvent(new MatchingFailed(Id, reason));
    }

    private void Apply(MatchingStarted evnt)
    {
        _hasStarted = true;
        _payment = new Payment(evnt.PaymentId, evnt.Iban, evnt.Amount, evnt.Reference);
        _expectation = new Expectation(evnt.ExpectationId, evnt.Iban, evnt.Amount, evnt.Reference);
    }

    private void Apply(ExpectationReserving evnt)
    {
    }

    private void Apply(ExpectationReserved evnt)
    {
        _expectation = _expectation! with { IsReserved = true };
    }

    private void Apply(ExpectationReservationRejected evnt)
    {
        _expectation = null;
    }

    private void Apply(PaymentReserving evnt)
    {
    }

    private void Apply(PaymentReserved evnt)
    {
        _payment = _payment! with { IsReserved = true };
    }

    private void Apply(PaymentReservationRejected evnt)
    {
        _payment = null;
    }

    private void Apply(ExpectationMatchApplying evnt)
    {
    }

    private void Apply(ExpectationMatchApplied evnt)
    {
        _expectation = _expectation! with { IsMatched = true };
    }

    private void Apply(PaymentMatchApplying evnt)
    {
    }

    private void Apply(PaymentMatchApplied evnt)
    {
        _payment = _payment! with { IsMatched = true };
    }

    private void Apply(MatchingCompleted evnt)
    {
        _hasFinished = true;
        _wasSuccessful = true;
    }

    private void Apply(MatchingFailed evnt)
    {
        _hasFinished = true;
        _wasSuccessful = false;
    }
}