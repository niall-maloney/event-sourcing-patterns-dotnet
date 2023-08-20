using MediatR;
using NiallMaloney.EventSourcing;
using NiallMaloney.EventSourcing.Subscriptions;
using NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Commands;
using NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Events;
using NiallMaloney.SingleCurrentAggregate.Service.Customers.Events;

namespace NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods;

[SubscriberName("BillingPeriodProcessManager")]
[Subscription("$ce-single_current_aggregate.customer")]
[Subscription("$ce-single_current_aggregate.billing_period")]
public class BillingPeriodsProcessManager : SubscriberBase
{
    private readonly IMediator _mediator;

    public BillingPeriodsProcessManager(IMediator mediator)
    {
        _mediator = mediator;
        When<BillingPeriodClosed>(Handle);
        When<CustomerAdded>(Handle);
    }

    private Task Handle(CustomerAdded evnt, EventMetadata metadata) =>
        OpenBillingPeriod(evnt.CustomerId, evnt.CustomerId);

    private Task Handle(BillingPeriodClosed evnt, EventMetadata metadata) =>
        OpenBillingPeriod(evnt.BillingPeriodId, evnt.CustomerId);

    private Task OpenBillingPeriod(string idempotencyKey, string customerId) =>
        _mediator.Send(new OpenBillingPeriod(Ids.NewBillingPeriodId(idempotencyKey), customerId));
}
