using MediatR;
using NiallMaloney.AggregateProcessManager.Cassandra;

namespace NiallMaloney.AggregateProcessManager.Service.Payments.Queries;

public class PaymentsQueryHandlers : IRequestHandler<GetPayment, PaymentRow?>,
    IRequestHandler<SearchPayments, IEnumerable<PaymentRow>>
{
    private readonly IPaymentsRepository _repository;

    public PaymentsQueryHandlers(IPaymentsRepository repository)
    {
        _repository = repository;
    }

    public Task<PaymentRow?> Handle(GetPayment request, CancellationToken cancellationToken) =>
        _repository.GetPayment(request.PaymentId);

    public Task<IEnumerable<PaymentRow>> Handle(SearchPayments request, CancellationToken cancellationToken) =>
        _repository.SearchPayments(request.PaymentId, request.Iban, request.Amount, request.Reference,
            request.Status);
}
