using MediatR;
using NiallMaloney.TwoPhaseCommit.Cassandra;
using NiallMaloney.TwoPhaseCommit.Cassandra.Expectations;

namespace NiallMaloney.TwoPhaseCommit.Service.Expectations.Queries;

public record GetExpectation(string ExpectationId) : IRequest<ExpectationRow?>;
