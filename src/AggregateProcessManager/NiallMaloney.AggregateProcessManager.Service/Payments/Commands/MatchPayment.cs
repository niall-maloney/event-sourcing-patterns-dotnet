using MediatR;

namespace NiallMaloney.AggregateProcessManager.Service.Payments.Commands;

public record MatchPayment(string PaymentId, string ExpectationId, string MatchingId) : IRequest;
