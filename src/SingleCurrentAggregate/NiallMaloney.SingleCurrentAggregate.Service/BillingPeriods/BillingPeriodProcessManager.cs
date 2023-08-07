using MediatR;
using NiallMaloney.EventSourcing;
using NiallMaloney.EventSourcing.Subscriptions;
using NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Commands;
using NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Events;

namespace NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods;

[SubscriberName("BillingPeriodProcessManager")]
[Subscription("$ce-single_current_aggregate.billing_period")]
public class BillingPeriodsProcessManager : SubscriberBase
{
    private readonly IMediator _mediator;

    public BillingPeriodsProcessManager(IMediator mediator)
    {
        _mediator = mediator;
        When<BillingPeriodClosed>(Handle);
    }

    private Task Handle(BillingPeriodClosed evnt, EventMetadata metadata) =>
        _mediator.Send(new OpenBillingPeriod(Ids.NewBillingPeriodId(evnt.BillingPeriodId)));
}