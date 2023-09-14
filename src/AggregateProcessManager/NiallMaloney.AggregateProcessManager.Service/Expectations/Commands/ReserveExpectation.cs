using MediatR;

namespace NiallMaloney.AggregateProcessManager.Service.Expectations.Commands;

public record ReserveExpectation(string ExpectationId, string MatchingId) : IRequest;
