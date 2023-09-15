using MediatR;

namespace NiallMaloney.AggregateProcessManager.Service.Matching.Commands;

public record ApplyPaymentMatch(string MatchingId, string PaymentId) : IRequest;
