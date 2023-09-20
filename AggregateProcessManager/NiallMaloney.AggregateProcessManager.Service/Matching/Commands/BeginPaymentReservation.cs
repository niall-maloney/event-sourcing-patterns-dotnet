using MediatR;

namespace NiallMaloney.AggregateProcessManager.Service.Matching.Commands;

public record BeginPaymentReservation(string MatchingId, string PaymentId) : IRequest;
