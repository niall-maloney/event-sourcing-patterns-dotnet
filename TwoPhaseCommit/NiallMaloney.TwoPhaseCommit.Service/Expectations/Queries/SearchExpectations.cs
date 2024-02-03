using MediatR;
using NiallMaloney.TwoPhaseCommit.Cassandra;
using NiallMaloney.TwoPhaseCommit.Cassandra.Expectations;

namespace NiallMaloney.TwoPhaseCommit.Service.Expectations.Queries;

public record SearchExpectations(
    string? ExpectationId = null,
    string? Iban = null,
    decimal? Amount = null,
    string? Reference = null,
    string? Status = null
) : IRequest<IEnumerable<ExpectationRow>>;
