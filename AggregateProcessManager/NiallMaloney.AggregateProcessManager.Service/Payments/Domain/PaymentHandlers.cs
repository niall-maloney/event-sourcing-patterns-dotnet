using MediatR;
using NiallMaloney.AggregateProcessManager.Service.Payments.Commands;
using NiallMaloney.EventSourcing.Aggregates;

namespace NiallMaloney.AggregateProcessManager.Service.Payments.Domain;

public class PaymentHandlers : IRequestHandler<ReceivePayment>,
    IRequestHandler<ReservePayment>,
    IRequestHandler<MatchPayment>,
    IRequestHandler<ReleasePayment>
{
    private readonly AggregateRepository _repository;

    public PaymentHandlers(AggregateRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(ReceivePayment request, CancellationToken cancellationToken)
    {
        var payment = await _repository.LoadAggregate<Payment>(request.PaymentId);
        payment.Receive(request.Iban, request.Amount, request.Reference);
        await _repository.SaveAggregate(payment);
    }

    public async Task Handle(ReservePayment request, CancellationToken cancellationToken)
    {
        var payment = await _repository.LoadAggregate<Payment>(request.PaymentId);
        payment.Reserve(request.MatchingId);
        await _repository.SaveAggregate(payment);
    }

    public async Task Handle(MatchPayment request, CancellationToken cancellationToken)
    {
        var payment = await _repository.LoadAggregate<Payment>(request.PaymentId);
        payment.Match(request.MatchingId, request.ExpectationId);
        await _repository.SaveAggregate(payment);
    }

    public async Task Handle(ReleasePayment request, CancellationToken cancellationToken)
    {
        var payment = await _repository.LoadAggregate<Payment>(request.PaymentId);
        payment.Release(request.MatchingId);
        await _repository.SaveAggregate(payment);
    }
}
