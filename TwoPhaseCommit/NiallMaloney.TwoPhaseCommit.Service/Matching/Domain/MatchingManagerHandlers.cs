using MediatR;
using NiallMaloney.EventSourcing.Aggregates;
using NiallMaloney.TwoPhaseCommit.Service.Matching.Commands;

namespace NiallMaloney.TwoPhaseCommit.Service.Matching.Domain;

public class MatchingManagerHandlers
    : IRequestHandler<BeginMatching>,
        IRequestHandler<AcknowledgePaymentReserved>,
        IRequestHandler<AcknowledgePaymentReservationRejected>,
        IRequestHandler<AcknowledgeExpectationReserved>,
        IRequestHandler<AcknowledgeExpectationReservationRejected>,
        IRequestHandler<AcknowledgePaymentMatched>,
        IRequestHandler<AcknowledgeExpectationMatched>,
        IRequestHandler<AcknowledgePaymentReservationFailed>
{
    private readonly AggregateRepository _repository;

    public MatchingManagerHandlers(AggregateRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(
        AcknowledgeExpectationMatched request,
        CancellationToken cancellationToken
    )
    {
        var manager = await _repository.LoadAggregate<MatchingManager>(request.MatchingId);
        manager.AcknowledgeExpectationApplied();
        manager.Complete();
        await _repository.SaveAggregate(manager);
    }

    public async Task Handle(
        AcknowledgeExpectationReservationRejected request,
        CancellationToken cancellationToken
    )
    {
        var manager = await _repository.LoadAggregate<MatchingManager>(request.MatchingId);
        manager.AcknowledgeExpectationReservationRejected();
        manager.Fail($"Expectation ({request.ExpectationId}) reservation was rejected");
        await _repository.SaveAggregate(manager);
    }

    public async Task Handle(
        AcknowledgeExpectationReserved request,
        CancellationToken cancellationToken
    )
    {
        var manager = await _repository.LoadAggregate<MatchingManager>(request.MatchingId);
        manager.AcknowledgeExpectationReserved();
        manager.ApplyPaymentMatch();
        await _repository.SaveAggregate(manager);
    }

    public async Task Handle(AcknowledgePaymentMatched request, CancellationToken cancellationToken)
    {
        var manager = await _repository.LoadAggregate<MatchingManager>(request.MatchingId);
        manager.AcknowledgePaymentApplied();
        manager.ApplyExpectationMatch();
        await _repository.SaveAggregate(manager);
    }

    public async Task Handle(
        AcknowledgePaymentReservationFailed request,
        CancellationToken cancellationToken
    )
    {
        var manager = await _repository.LoadAggregate<MatchingManager>(request.MatchingId);
        manager.Fail(request.Reason);
        await _repository.SaveAggregate(manager);
    }

    public async Task Handle(
        AcknowledgePaymentReservationRejected request,
        CancellationToken cancellationToken
    )
    {
        var manager = await _repository.LoadAggregate<MatchingManager>(request.MatchingId);
        manager.AcknowledgePaymentReservationRejected();
        manager.Fail($"Payment ({request.PaymentId}) reservation was rejected");
        await _repository.SaveAggregate(manager);
    }

    public async Task Handle(
        AcknowledgePaymentReserved request,
        CancellationToken cancellationToken
    )
    {
        var manager = await _repository.LoadAggregate<MatchingManager>(request.MatchingId);
        manager.AcknowledgePaymentReserved();
        manager.ReserveExpectation();
        await _repository.SaveAggregate(manager);
    }

    public async Task Handle(BeginMatching request, CancellationToken cancellationToken)
    {
        var manager = await _repository.LoadAggregate<MatchingManager>(request.MatchingId);
        manager.Begin(
            request.ExpectationId,
            request.PaymentId,
            request.Iban,
            request.Amount,
            request.Reference
        );
        manager.ReservePayment();
        await _repository.SaveAggregate(manager);
    }
}