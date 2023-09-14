using MediatR;
using NiallMaloney.AggregateProcessManager.Service.Expectations.Commands;
using NiallMaloney.EventSourcing.Aggregates;

namespace NiallMaloney.AggregateProcessManager.Service.Expectations.Domain;

public class ExpectationHandlers : IRequestHandler<CreateExpectation>,
    IRequestHandler<ReserveExpectation>,
    IRequestHandler<MatchExpectation>
{
    private readonly AggregateRepository _repository;

    public ExpectationHandlers(AggregateRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(CreateExpectation request, CancellationToken cancellationToken)
    {
        var expectation = await _repository.LoadAggregate<Expectation>(request.ExpectationId);
        expectation.Receive(request.Iban, request.Amount, request.Reference);
        await _repository.SaveAggregate(expectation);
    }

    public async Task Handle(ReserveExpectation request, CancellationToken cancellationToken)
    {
        var expectation = await _repository.LoadAggregate<Expectation>(request.ExpectationId);
        expectation.Reserve(request.MatchingId);
        await _repository.SaveAggregate(expectation);
    }

    public async Task Handle(MatchExpectation request, CancellationToken cancellationToken)
    {
        var expectation = await _repository.LoadAggregate<Expectation>(request.ExpectationId);
        expectation.Match(request.MatchingId, request.ExpectationId);
        await _repository.SaveAggregate(expectation);
    }
}
