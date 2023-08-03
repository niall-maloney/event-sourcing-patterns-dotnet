using MediatR;
using NiallMaloney.EventSourcing.Aggregates;
using NiallMaloney.SingleCurrentAggregate.Cassandra;
using NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Commands;

namespace NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Domain;

public class BillingPeriodHandlers : IRequestHandler<OpenBillingPeriod>,
    IRequestHandler<CloseBillingPeriod>,
    IRequestHandler<AddCharge>,
    IRequestHandler<RemoveCharge>
{
    private readonly AggregateRepository _aggregateRepository;
    private readonly IBillingPeriodsRepository _billingPeriodsRepository;

    public BillingPeriodHandlers(
        AggregateRepository aggregateRepository,
        IBillingPeriodsRepository billingPeriodsRepository)
    {
        _aggregateRepository = aggregateRepository;
        _billingPeriodsRepository = billingPeriodsRepository;
    }

    public async Task Handle(AddCharge request, CancellationToken cancellationToken)
    {
        var billingPeriodId = await GetCurrentBillingPeriodId();
        var billingPeriod = await _aggregateRepository.LoadAggregate<BillingPeriod>(billingPeriodId);
        billingPeriod.AddCharge(request.ChargeId, request.Amount);
        await _aggregateRepository.SaveAggregate(billingPeriod);
    }

    public async Task Handle(CloseBillingPeriod request, CancellationToken cancellationToken)
    {
        var billingPeriod = await _aggregateRepository.LoadAggregate<BillingPeriod>(request.BillingPeriodId);
        billingPeriod.Close();
        await _aggregateRepository.SaveAggregate(billingPeriod);
    }

    public async Task Handle(OpenBillingPeriod request, CancellationToken cancellationToken)
    {
        var billingPeriod = await _aggregateRepository.LoadAggregate<BillingPeriod>(request.BillingPeriodId);
        billingPeriod.Open();
        await _aggregateRepository.SaveAggregate(billingPeriod);
    }

    public async Task Handle(RemoveCharge request, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    private async Task<string> GetCurrentBillingPeriodId()
    {
        var rows = (await _billingPeriodsRepository.SearchBillingPeriods(status: "Open")).ToArray();
        return rows.Length switch
        {
            > 1 => throw new InvalidOperationException("Multiple open billing periods"),
            < 1 => throw new InvalidOperationException("No open billing period"),
            _ => rows.Single().BillingPeriodId
        };
    }
}
