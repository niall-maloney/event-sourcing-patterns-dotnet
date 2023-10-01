using MediatR;
using NiallMaloney.TwoPhaseCommit.Cassandra;

namespace NiallMaloney.TwoPhaseCommit.Service.Matching.Queries;

public record SearchManagers(
    string? MatchingId = null,
    string? PaymentId = null,
    string? ExpectationId = null,
    string? Iban = null,
    decimal? Amount = null,
    string? Reference = null,
    string? Status = null
) : IRequest<IEnumerable<MatchingManagerRow>>;
