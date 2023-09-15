using MediatR;

namespace NiallMaloney.AggregateProcessManager.Service.Matching.Commands;

public record AcknowledgeExpectationReservationRejected(string MatchingId, string ExpectationId) : IRequest;