using MediatR;
using NiallMaloney.TwoPhaseCommit.Cassandra;
using NiallMaloney.TwoPhaseCommit.Cassandra.Payments;

namespace NiallMaloney.TwoPhaseCommit.Service.Payments.Queries;

public record SearchPayments(
    string? PaymentId = null,
    string? Iban = null,
    decimal? Amount = null,
    string? Reference = null,
    string? Status = null
) : IRequest<IEnumerable<PaymentRow>>;
