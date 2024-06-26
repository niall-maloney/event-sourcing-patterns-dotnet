using MediatR;

namespace NiallMaloney.TwoPhaseCommit.Service.Matching.Commands;

public record AcknowledgePaymentReservationRejected(string MatchingId, string PaymentId) : IRequest;