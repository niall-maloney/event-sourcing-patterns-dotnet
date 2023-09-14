using MediatR;

namespace NiallMaloney.AggregateProcessManager.Service.Matching.Commands;

public record AcknowledgePaymentReserved(string MatchingId, string PaymentId) : IRequest;
