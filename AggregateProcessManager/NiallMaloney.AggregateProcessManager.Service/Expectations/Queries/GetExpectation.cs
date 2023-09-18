using MediatR;
using NiallMaloney.AggregateProcessManager.Cassandra;

namespace NiallMaloney.AggregateProcessManager.Service.Expectations.Queries;

public record GetExpectation(string ExpectationId) : IRequest<ExpectationRow?>;
