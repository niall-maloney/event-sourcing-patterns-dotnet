using MediatR;
using NiallMaloney.EventSourcing.Aggregates;
using NiallMaloney.SingleCurrentAggregate.Cassandra;
using NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Commands;

namespace NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Domain;

public class BillingPeriodHandlers
    : IRequestHandler<OpenBillingPeriod>,
        IRequestHandler<CloseBillingPeriod>,
        IRequestHandler<AddCharge>,
        IRequestHandler<RemoveCharge>
{
    private readonly AggregateRepository _aggregateRepository;
    private readonly IBillingPeriodsRepository _billingPeriodsRepository;
    private readonly IChargesRepository _chargesRepository;

    public BillingPeriodHandlers(
        AggregateRepository aggregateRepository,
        IBillingPeriodsRepository billingPeriodsRepository,
        IChargesRepository chargesRepository
    )
    {
        _aggregateRepository = aggregateRepository;
        _billingPeriodsRepository = billingPeriodsRepository;
        _chargesRepository = chargesRepository;
    }

    public async Task Handle(AddCharge request, CancellationToken cancellationToken)
    {
        var billingPeriodId = await GetCurrentBillingPeriodId(request.CustomerId);
        var billingPeriod = await _aggregateRepository.LoadAggregate<BillingPeriod>(
            billingPeriodId
        );
        billingPeriod.AddCharge(request.ChargeId, request.Amount);
        await _aggregateRepository.SaveAggregate(billingPeriod);
    }

    public async Task Handle(CloseBillingPeriod request, CancellationToken cancellationToken)
    {
        var billingPeriod = await _aggregateRepository.LoadAggregate<BillingPeriod>(
            request.BillingPeriodId
        );
        billingPeriod.Close();
        await _aggregateRepository.SaveAggregate(billingPeriod);
    }

    public async Task Handle(OpenBillingPeriod request, CancellationToken cancellationToken)
    {
        var billingPeriod = await _aggregateRepository.LoadAggregate<BillingPeriod>(
            request.BillingPeriodId
        );
        billingPeriod.Open(request.CustomerId);
        await _aggregateRepository.SaveAggregate(billingPeriod);
    }

    public async Task Handle(RemoveCharge request, CancellationToken cancellationToken)
    {
        var charge = await _chargesRepository.GetCharge(request.ChargeId);
        if (charge is null)
        {
            throw new InvalidOperationException("Charge not found in projection.");
        }
        var billingPeriod = await _aggregateRepository.LoadAggregate<BillingPeriod>(
            charge.BillingPeriodId!
        );
        billingPeriod.RemoveCharge(request.ChargeId);
        await _aggregateRepository.SaveAggregate(billingPeriod);
    }

    private async Task<string> GetCurrentBillingPeriodId(string customerId)
    {
        var rows = (
            await _billingPeriodsRepository.SearchBillingPeriods(
                customerId: customerId,
                status: "Open"
            )
        ).ToArray();
        return rows.Length switch
        {
            > 1 => throw new InvalidOperationException("Multiple open billing periods"),
            < 1 => throw new InvalidOperationException("No open billing period"),
            _ => rows.Single().BillingPeriodId!
        };
    }
}
