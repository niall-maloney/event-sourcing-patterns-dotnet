using NiallMaloney.EventSourcing;

namespace NiallMaloney.TwoPhaseCommit.Service.Payments.Events;

[Event("two_phase_commit.payment_received")]
public record PaymentReceived(string PaymentId, string Iban, decimal Amount, string Reference)
    : IEvent;