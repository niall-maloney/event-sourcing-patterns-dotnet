using MediatR;
using NiallMaloney.AggregateProcessManager.Cassandra;

namespace NiallMaloney.AggregateProcessManager.Service.Expectations.Queries;

public record SearchExpectations(
    string? ExpectationId = null,
    string? Iban = null,
    decimal? Amount = null,
    string? Reference = null,
    string? Status = null) : IRequest<IEnumerable<ExpectationRow>>;
