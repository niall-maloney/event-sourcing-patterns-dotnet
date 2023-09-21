using MediatR;

namespace NiallMaloney.TwoPhaseCommit.Service.Matching.Commands;

public record BeginMatching(
    string MatchingId,
    string ExpectationId,
    string PaymentId,
    string Iban,
    decimal Amount,
    string Reference
) : IRequest;
