using MediatR;

namespace NiallMaloney.AggregateProcessManager.Service.Matching.Commands;

public record AcknowledgePaymentMatched(string MatchingId, string PaymentId) : IRequest;
