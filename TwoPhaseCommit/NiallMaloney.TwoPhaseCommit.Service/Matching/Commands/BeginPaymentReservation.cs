using MediatR;

namespace NiallMaloney.TwoPhaseCommit.Service.Matching.Commands;

public record BeginPaymentReservation(string MatchingId, string PaymentId) : IRequest;