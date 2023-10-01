using MediatR;
using NiallMaloney.SingleCurrentAggregate.Cassandra;

namespace NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Queries;

public class BillingPeriodsQueryHandlers
    : IRequestHandler<GetBillingPeriod, BillingPeriodRow?>,
        IRequestHandler<SearchBillingPeriods, IEnumerable<BillingPeriodRow>>
{
    private readonly IBillingPeriodsRepository _repository;

    public BillingPeriodsQueryHandlers(IBillingPeriodsRepository repository)
    {
        _repository = repository;
    }

    public Task<BillingPeriodRow?> Handle(
        GetBillingPeriod request,
        CancellationToken cancellationToken
    ) => _repository.GetBillingPeriod(request.BillingPeriodId);

    public Task<IEnumerable<BillingPeriodRow>> Handle(
        SearchBillingPeriods request,
        CancellationToken cancellationToken
    ) =>
        _repository.SearchBillingPeriods(
            request.BillingPeriodId,
            request.CustomerId,
            request.Status
        );
}
