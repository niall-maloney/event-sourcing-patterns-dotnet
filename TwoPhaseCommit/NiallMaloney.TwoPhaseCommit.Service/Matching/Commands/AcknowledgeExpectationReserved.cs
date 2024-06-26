using MediatR;

namespace NiallMaloney.TwoPhaseCommit.Service.Matching.Commands;

public record AcknowledgeExpectationReserved(string MatchingId, string ExpectationId) : IRequest;