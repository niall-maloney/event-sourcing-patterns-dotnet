using MediatR;

namespace NiallMaloney.TwoPhaseCommit.Service.Matching.Commands;

public record ApplyExpectationMatch(string MatchingId, string ExpectationId) : IRequest;