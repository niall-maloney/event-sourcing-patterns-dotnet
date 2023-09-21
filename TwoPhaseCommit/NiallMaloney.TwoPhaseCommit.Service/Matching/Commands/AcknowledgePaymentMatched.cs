using MediatR;

namespace NiallMaloney.TwoPhaseCommit.Service.Matching.Commands;

public record AcknowledgePaymentMatched(string MatchingId, string PaymentId) : IRequest;
