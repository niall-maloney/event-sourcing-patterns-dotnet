using MediatR;
using NiallMaloney.TwoPhaseCommit.Cassandra;

namespace NiallMaloney.TwoPhaseCommit.Service.Expectations.Queries;

public record GetExpectation(string ExpectationId) : IRequest<ExpectationRow?>;
