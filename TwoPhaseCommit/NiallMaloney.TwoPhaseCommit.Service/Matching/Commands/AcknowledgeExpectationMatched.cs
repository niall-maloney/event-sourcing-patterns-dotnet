using MediatR;

namespace NiallMaloney.TwoPhaseCommit.Service.Matching.Commands;

public record AcknowledgeExpectationMatched(string MatchingId, string ExpectationId) : IRequest;