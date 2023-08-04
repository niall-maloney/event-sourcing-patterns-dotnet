using MediatR;
using NiallMaloney.SingleCurrentAggregate.Cassandra;

namespace NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Queries;

public class BillingPeriodQueryHandlers : IRequestHandler<GetBillingPeriod, BillingPeriodRow>,
    IRequestHandler<SearchBillingPeriod, IEnumerable<BillingPeriodRow>>
{
    private readonly IBillingPeriodsRepository _repository;

    public BillingPeriodQueryHandlers(IBillingPeriodsRepository repository)
    {
        _repository = repository;
    }

    public Task<BillingPeriodRow> Handle(GetBillingPeriod request, CancellationToken cancellationToken) =>
        _repository.GetBillingPeriod(request.BillingPeriodId);

    public Task<IEnumerable<BillingPeriodRow>>
        Handle(SearchBillingPeriod request, CancellationToken cancellationToken) =>
        _repository.SearchBillingPeriods(request.BillingPeriodId, request.Status);
}
