using MediatR;

namespace NiallMaloney.TwoPhaseCommit.Service.Matching.Commands;

public record ApplyPaymentMatch(string MatchingId, string PaymentId) : IRequest;
