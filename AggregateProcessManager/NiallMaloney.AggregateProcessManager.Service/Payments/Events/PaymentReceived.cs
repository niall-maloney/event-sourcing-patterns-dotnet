using NiallMaloney.EventSourcing;

namespace NiallMaloney.AggregateProcessManager.Service.Payments.Events;

[Event("aggregate_process_manager.payment_received")]
public record PaymentReceived(string PaymentId, string Iban, decimal Amount, string Reference) : IEvent;
