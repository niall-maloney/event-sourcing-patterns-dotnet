using MediatR;

namespace NiallMaloney.TwoPhaseCommit.Service.Matching.Commands;

public record AcknowledgeExpectationReservationRejected(string MatchingId, string ExpectationId)
    : IRequest;