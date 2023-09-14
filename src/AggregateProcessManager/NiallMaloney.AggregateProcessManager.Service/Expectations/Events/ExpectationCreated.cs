using NiallMaloney.EventSourcing;

namespace NiallMaloney.AggregateProcessManager.Service.Expectations.Events;

[Event("aggregate_process_manager.expectation_created")]
public record ExpectationCreated(string ExpectationId, string Iban, decimal Amount, string Reference) : IEvent;
