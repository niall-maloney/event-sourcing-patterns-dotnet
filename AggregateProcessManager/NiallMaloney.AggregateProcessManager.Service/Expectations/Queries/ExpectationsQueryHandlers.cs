using MediatR;
using NiallMaloney.AggregateProcessManager.Cassandra;

namespace NiallMaloney.AggregateProcessManager.Service.Expectations.Queries;

public class ExpectationsQueryHandlers : IRequestHandler<GetExpectation, ExpectationRow?>,
    IRequestHandler<SearchExpectations, IEnumerable<ExpectationRow>>
{
    private readonly IExpectationsRepository _repository;

    public ExpectationsQueryHandlers(IExpectationsRepository repository)
    {
        _repository = repository;
    }

    public Task<ExpectationRow?> Handle(GetExpectation request, CancellationToken cancellationToken) =>
        _repository.GetExpectation(request.ExpectationId);

    public Task<IEnumerable<ExpectationRow>> Handle(SearchExpectations request, CancellationToken cancellationToken) =>
        _repository.SearchExpectations(request.ExpectationId, request.Iban, request.Amount, request.Reference,
            request.Status);
}
