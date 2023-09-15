using MediatR;

namespace NiallMaloney.AggregateProcessManager.Service.Payments.Commands;

public record ReservePayment(string PaymentId, string MatchingId) : IRequest;
