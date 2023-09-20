using MediatR;
using NiallMaloney.AggregateProcessManager.Service.Expectations.Commands;
using NiallMaloney.AggregateProcessManager.Service.Expectations.Events;
using NiallMaloney.AggregateProcessManager.Service.Matching.Commands;
using NiallMaloney.AggregateProcessManager.Service.Matching.Events;
using NiallMaloney.AggregateProcessManager.Service.Payments.Commands;
using NiallMaloney.AggregateProcessManager.Service.Payments.Events;
using NiallMaloney.EventSourcing.Subscriptions;

namespace NiallMaloney.AggregateProcessManager.Service.Matching.Domain;

[SubscriberName("MatchingProcessManager")]
[Subscription("$ce-aggregate_process_manager.matching_manager")]
[Subscription("$ce-aggregate_process_manager.expectation")]
[Subscription("$ce-aggregate_process_manager.payment")]
public class MatchingProcessManager : SubscriberBase
{
    public MatchingProcessManager(IMediator mediator)
    {
        //matching manager events
        When<MatchingStarted>((evnt, metadata) => Task.CompletedTask);
        When<PaymentReserving>(
            async (evnt, metadata) =>
                await mediator.Send(new ReservePayment(evnt.PaymentId, evnt.MatchingId))
        );
        When<PaymentReserved>((evnt, metadata) => Task.CompletedTask);
        When<PaymentReservationRejected>((evnt, metadata) => Task.CompletedTask);
        When<ExpectationReserving>(
            async (evnt, metadata) =>
                await mediator.Send(new ReserveExpectation(evnt.ExpectationId, evnt.MatchingId))
        );
        When<ExpectationReserved>((evnt, metadata) => Task.CompletedTask);
        When<ExpectationReservationRejected>(
            async (evnt, metadata) =>
                await mediator.Send(new ReleasePayment(evnt.PaymentId, evnt.MatchingId))
        );
        When<PaymentMatchApplying>(
            async (evnt, metadata) =>
                await mediator.Send(
                    new MatchPayment(evnt.PaymentId, evnt.ExpectationId, evnt.MatchingId)
                )
        );
        When<PaymentMatchApplied>((evnt, metadata) => Task.CompletedTask);
        When<ExpectationMatchApplying>(
            async (evnt, metadata) =>
                await mediator.Send(
                    new MatchExpectation(evnt.ExpectationId, evnt.PaymentId, evnt.MatchingId)
                )
        );
        When<ExpectationMatchApplied>((evnt, metadata) => Task.CompletedTask);
        When<MatchingCompleted>((evnt, metadata) => Task.CompletedTask);

        //payment events
        When<PaymentReceived>((evnt, metadata) => Task.CompletedTask);
        When<PaymentMatching>(
            async (evnt, metadata) =>
                await mediator.Send(new AcknowledgePaymentReserved(evnt.MatchingId, evnt.PaymentId))
        );
        When<PaymentMatched>(
            async (evnt, metadata) =>
                await mediator.Send(new AcknowledgePaymentMatched(evnt.MatchingId, evnt.PaymentId))
        );
        When<PaymentMatchRejected>(
            async (evnt, metadata) =>
                await mediator.Send(
                    new AcknowledgePaymentReservationRejected(evnt.MatchingId, evnt.PaymentId)
                )
        );

        //expectation events
        When<ExpectationCreated>((evnt, metadata) => Task.CompletedTask);
        When<ExpectationMatching>(
            async (evnt, metadata) =>
                await mediator.Send(
                    new AcknowledgeExpectationReserved(evnt.MatchingId, evnt.ExpectationId)
                )
        );
        When<ExpectationMatched>(
            async (evnt, metadata) =>
                await mediator.Send(
                    new AcknowledgeExpectationMatched(evnt.MatchingId, evnt.ExpectationId)
                )
        );
        When<ExpectationMatchRejected>(
            async (evnt, metadata) =>
                await mediator.Send(
                    new AcknowledgeExpectationReservationRejected(
                        evnt.MatchingId,
                        evnt.ExpectationId
                    )
                )
        );
    }
}
