using MediatR;

namespace NiallMaloney.TwoPhaseCommit.Service.Matching.Commands;

public record AcknowledgePaymentReserved(string MatchingId, string PaymentId) : IRequest;