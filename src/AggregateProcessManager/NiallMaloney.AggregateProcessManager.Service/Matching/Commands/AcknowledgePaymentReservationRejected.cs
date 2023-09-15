using MediatR;

namespace NiallMaloney.AggregateProcessManager.Service.Matching.Commands;

public record AcknowledgePaymentReservationRejected(string MatchingId, string PaymentId) : IRequest;
