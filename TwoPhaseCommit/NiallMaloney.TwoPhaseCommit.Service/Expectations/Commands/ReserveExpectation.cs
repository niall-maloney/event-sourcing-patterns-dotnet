using MediatR;

namespace NiallMaloney.TwoPhaseCommit.Service.Expectations.Commands;

public record ReserveExpectation(string ExpectationId, string MatchingId) : IRequest;
