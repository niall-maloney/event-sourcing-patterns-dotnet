using NiallMaloney.EventSourcing;
using NiallMaloney.EventSourcing.Projections;
using NiallMaloney.EventSourcing.Subscriptions;
using NiallMaloney.TwoPhaseCommit.Cassandra;
using NiallMaloney.TwoPhaseCommit.Cassandra.Payments;
using NiallMaloney.TwoPhaseCommit.Service.Payments.Events;

namespace NiallMaloney.TwoPhaseCommit.Service.Payments.Projections;

[SubscriberName("PaymentsProjection")]
[Subscription("$ce-two_phase_commit.payment")]
public class PaymentsProjection : Projection
{
    private readonly IPaymentsRepository _repository;

    public PaymentsProjection(IPaymentsRepository repository)
    {
        _repository = repository;

        When<PaymentReceived>(Handle);
        When<PaymentMatching>(Handle);
        When<PaymentReleased>(Handle);
        When<PaymentMatched>(Handle);
        When<PaymentMatchRejected>(Handle);
    }

    private async Task Handle(PaymentReceived evnt, EventMetadata metadata)
    {
        var payment = await _repository.GetPayment(evnt.PaymentId);
        if (payment is not null)
        {
            return;
        }

        await _repository.AddPayment(
            new PaymentRow
            {
                PaymentId = evnt.PaymentId,
                Iban = evnt.Iban,
                Amount = evnt.Amount,
                Reference = evnt.Reference,
                Status = "Received",
                Version = metadata.StreamPosition
            }
        );
    }

    private async Task Handle(PaymentMatching evnt, EventMetadata metadata)
    {
        var payment = await _repository.GetPayment(evnt.PaymentId);
        if (payment is null || !TryUpdateVersion(payment, metadata.StreamPosition, out payment))
        {
            return;
        }

        payment = payment with { Status = "Reserved" };
        await _repository.UpdatePayment(payment);
    }

    private async Task Handle(PaymentReleased evnt, EventMetadata metadata)
    {
        var payment = await _repository.GetPayment(evnt.PaymentId);
        if (payment is null || !TryUpdateVersion(payment, metadata.StreamPosition, out payment))
        {
            return;
        }

        payment = payment with { Status = "Received" };
        await _repository.UpdatePayment(payment);
    }

    private async Task Handle(PaymentMatched evnt, EventMetadata metadata)
    {
        var payment = await _repository.GetPayment(evnt.PaymentId);
        if (payment is null || !TryUpdateVersion(payment, metadata.StreamPosition, out payment))
        {
            return;
        }

        payment = payment with { Status = "Matched" };
        await _repository.UpdatePayment(payment);
    }

    private async Task Handle(PaymentMatchRejected evnt, EventMetadata metadata)
    {
        var payment = await _repository.GetPayment(evnt.PaymentId);
        if (payment is null || !TryUpdateVersion(payment, metadata.StreamPosition, out payment))
        {
            return;
        }

        await _repository.UpdatePayment(payment);
    }

    private bool TryUpdateVersion(PaymentRow payment, ulong newVersion, out PaymentRow newPayment)
    {
        var expectedVersion = newVersion - 1;
        var actualVersion = payment.Version;
        if (actualVersion >= newVersion)
        {
            newPayment = payment;
            return false;
        }

        if (actualVersion != expectedVersion)
        {
            throw new InvalidOperationException(
                $"Version mismatch, expected {expectedVersion} actual {actualVersion}"
            );
        }

        newPayment = payment with { Version = newVersion };
        return true;
    }
}
