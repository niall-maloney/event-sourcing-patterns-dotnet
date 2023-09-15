using MediatR;
using NiallMaloney.SingleCurrentAggregate.Cassandra;

namespace NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Queries;

public class ChargesQueryHandlers : IRequestHandler<GetCharge, ChargeRow?>,
    IRequestHandler<SearchCharges, IEnumerable<ChargeRow>>
{
    private readonly IChargesRepository _repository;

    public ChargesQueryHandlers(IChargesRepository repository)
    {
        _repository = repository;
    }

    public async Task<ChargeRow?> Handle(GetCharge request, CancellationToken cancellationToken) =>
        await _repository.GetCharge(request.ChargeId);

    public async Task<IEnumerable<ChargeRow>> Handle(SearchCharges request, CancellationToken cancellationToken) =>
        (await _repository.SearchCharges(request.ChargeId, request.BillingPeriodId, request.Status)).ToList();
}