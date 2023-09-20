using MediatR;
using NiallMaloney.AggregateProcessManager.Cassandra;

namespace NiallMaloney.AggregateProcessManager.Service.Payments.Queries;

public record SearchPayments(
    string? PaymentId = null,
    string? Iban = null,
    decimal? Amount = null,
    string? Reference = null,
    string? Status = null
) : IRequest<IEnumerable<PaymentRow>>;
