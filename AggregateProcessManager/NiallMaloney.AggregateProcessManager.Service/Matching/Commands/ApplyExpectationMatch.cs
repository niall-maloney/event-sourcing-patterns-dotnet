using MediatR;

namespace NiallMaloney.AggregateProcessManager.Service.Matching.Commands;

public record ApplyExpectationMatch(
    string MatchingId,
    string ExpectationId) : IRequest;