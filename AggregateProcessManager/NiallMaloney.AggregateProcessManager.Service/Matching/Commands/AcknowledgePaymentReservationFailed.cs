using MediatR;

namespace NiallMaloney.AggregateProcessManager.Service.Matching.Commands;

public record AcknowledgePaymentReservationFailed(string MatchingId, string Reason) : IRequest;
