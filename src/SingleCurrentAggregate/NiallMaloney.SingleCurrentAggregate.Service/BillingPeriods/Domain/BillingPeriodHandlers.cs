using MediatR;
using NiallMaloney.EventSourcing.Aggregates;
using NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Commands;

namespace NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Domain;

public class BillingPeriodHandlers : IRequestHandler<OpenBillingPeriod>,IRequestHandler<CloseBillingPeriod>, IRequestHandler<AddCharge>, IRequestHandler<RemoveCharge>
{
    private readonly AggregateRepository _repository;

    public BillingPeriodHandlers(AggregateRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(OpenBillingPeriod request, CancellationToken cancellationToken)
    {
        var billingPeriod = await _repository.LoadAggregate<BillingPeriod>(request.BillingPeriodId);
        billingPeriod.Open();
        await _repository.SaveAggregate(billingPeriod);
    }

    public async Task Handle(CloseBillingPeriod request, CancellationToken cancellationToken)
    {
        var billingPeriod = await _repository.LoadAggregate<BillingPeriod>(request.BillingPeriodId);
        billingPeriod.Close();
        await _repository.SaveAggregate(billingPeriod);
    }

    public async Task Handle(AddCharge request, CancellationToken cancellationToken)
    {
        var billingPeriodId = await GetCurrentBillingPeriodId();
        var billingPeriod = await _repository.LoadAggregate<BillingPeriod>(billingPeriodId);
        billingPeriod.AddCharge(request.ChargeId, request.Amount);
        await _repository.SaveAggregate(billingPeriod);
    }


    private async Task<string> GetCurrentBillingPeriodId()
    {
        return string.Empty;
    }

    public async Task Handle(RemoveCharge request, CancellationToken cancellationToken)
    {
        var billingPeriodId = await GetCurrentBillingPeriodId();
        var billingPeriod = await _repository.LoadAggregate<BillingPeriod>(billingPeriodId);
        billingPeriod.RemoveCharge(request.ChargeId);
        await _repository.SaveAggregate(billingPeriod);    }
}
