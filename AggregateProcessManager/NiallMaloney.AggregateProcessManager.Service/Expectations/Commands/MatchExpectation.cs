using MediatR;

namespace NiallMaloney.AggregateProcessManager.Service.Expectations.Commands;

public record MatchExpectation(string ExpectationId, string PaymentId, string MatchingId) : IRequest;