using MediatR;

namespace NiallMaloney.AggregateProcessManager.Service.Matching.Commands;

public record AcknowledgeExpectationMatched(string MatchingId, string ExpectationId) : IRequest;
