using MediatR;

namespace NiallMaloney.TwoPhaseCommit.Service.Expectations.Commands;

public record MatchExpectation(string ExpectationId, string PaymentId, string MatchingId)
    : IRequest;
