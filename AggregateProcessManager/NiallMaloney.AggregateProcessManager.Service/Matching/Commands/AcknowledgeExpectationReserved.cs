using MediatR;

namespace NiallMaloney.AggregateProcessManager.Service.Matching.Commands;

public record AcknowledgeExpectationReserved(string MatchingId, string ExpectationId) : IRequest;