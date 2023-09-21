using MediatR;

namespace NiallMaloney.TwoPhaseCommit.Service.Matching.Commands;

public record AcknowledgePaymentReservationFailed(string MatchingId, string Reason) : IRequest;
