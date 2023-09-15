using MediatR;

namespace NiallMaloney.AggregateProcessManager.Service.Payments.Commands;

public record ReleasePayment(string PaymentId, string MatchingId) : IRequest;
