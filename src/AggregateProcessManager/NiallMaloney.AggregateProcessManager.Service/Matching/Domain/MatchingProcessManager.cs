using MediatR;
using NiallMaloney.AggregateProcessManager.Service.Expectations.Commands;
using NiallMaloney.AggregateProcessManager.Service.Expectations.Events;
using NiallMaloney.AggregateProcessManager.Service.Matching.Commands;
using NiallMaloney.AggregateProcessManager.Service.Matching.Events;
using NiallMaloney.AggregateProcessManager.Service.Payments.Commands;
using NiallMaloney.AggregateProcessManager.Service.Payments.Events;
using NiallMaloney.EventSourcing;
using NiallMaloney.EventSourcing.Subscriptions;
using PaymentReserved = NiallMaloney.AggregateProcessManager.Service.Payments.Events.PaymentReserved;

namespace NiallMaloney.AggregateProcessManager.Service.Matching.Domain;

[SubscriberName("MatchingProcessManager")]
[Subscription("$ce-aggregate_process_manager.matching_manager")]
[Subscription("$ce-aggregate_process_manager.expectation")]
[Subscription("$ce-aggregate_process_manager.payment")]
public class MatchingProcessManager : SubscriberBase
{
    private readonly IMediator _mediator;

    public MatchingProcessManager(IMediator mediator)
    {
        _mediator = mediator;

        //matching manager events
        When<MatchingStarted>(Handle);
        When<ExpectationReserving>(Handle);
        When<ExpectationReserved>(Handle);
        When<PaymentReserving>(Handle);
        When<AggregateProcessManager.Service.Matching.Events.PaymentReserved>(Handle);
        When<ExpectationMatchApplying>(Handle);
        When<ExpectationMatchApplied>(Handle);
        When<PaymentMatchApplying>(Handle);
        When<PaymentMatchApplied>(Handle);
        When<MatchingCompleted>(Handle);

        //expectation events
        When<ExpectationCreated>(Handle);
        When<ExpectationMatching>(Handle);
        When<ExpectationMatched>(Handle);

        //payment events
        When<PaymentReceived>(Handle);
        When<PaymentReserved>(Handle);
        When<PaymentMatched>(Handle);
    }

    private Task Handle(ExpectationCreated evnt, EventMetadata metadata) => Task.CompletedTask;

    private Task Handle(PaymentReceived evnt, EventMetadata metadata) => Task.CompletedTask;

    private Task Handle(MatchingStarted evnt, EventMetadata metadata) => Task.CompletedTask;

    private async Task Handle(PaymentReserving evnt, EventMetadata metadata) =>
        await _mediator.Send(new ReservePayment(evnt.PaymentId, evnt.MatchingId));

    private async Task Handle(PaymentReserved evnt, EventMetadata metadata) =>
        await _mediator.Send(new AcknowledgePaymentReserved(evnt.MatchingId, evnt.PaymentId));

    private Task Handle(AggregateProcessManager.Service.Matching.Events.PaymentReserved evnt, EventMetadata metadata) => Task.CompletedTask;

    private async Task Handle(ExpectationReserving evnt, EventMetadata metadata) =>
        await _mediator.Send(new ReserveExpectation(evnt.ExpectationId, evnt.MatchingId));

    private async Task Handle(ExpectationMatching evnt, EventMetadata metadata) =>
        await _mediator.Send(new AcknowledgeExpectationReserved(evnt.MatchingId, evnt.ExpectationId));

    private Task Handle(ExpectationReserved evnt, EventMetadata metadata) => Task.CompletedTask;

    private async Task Handle(PaymentMatchApplying evnt, EventMetadata metadata) =>
        await _mediator.Send(new MatchPayment(evnt.PaymentId, evnt.ExpectationId, evnt.MatchingId));

    private async Task Handle(PaymentMatched evnt, EventMetadata metadata) =>
        await _mediator.Send(new AcknowledgePaymentMatched(evnt.MatchingId, evnt.PaymentId));

    private Task Handle(PaymentMatchApplied evnt, EventMetadata metadata) => Task.CompletedTask;

    private async Task Handle(ExpectationMatchApplying evnt, EventMetadata metadata) =>
        await _mediator.Send(new MatchExpectation(evnt.ExpectationId, evnt.PaymentId, evnt.MatchingId));

    private async Task Handle(ExpectationMatched evnt, EventMetadata metadata) =>
        await _mediator.Send(new AcknowledgeExpectationMatched(evnt.MatchingId, evnt.ExpectationId));

    private Task Handle(ExpectationMatchApplied evnt, EventMetadata metadata) => Task.CompletedTask;

    private Task Handle(MatchingCompleted evnt, EventMetadata metadata) => Task.CompletedTask;
}
