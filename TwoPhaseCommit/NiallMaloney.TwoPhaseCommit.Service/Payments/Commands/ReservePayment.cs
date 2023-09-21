using MediatR;

namespace NiallMaloney.TwoPhaseCommit.Service.Payments.Commands;

public record ReservePayment(string PaymentId, string MatchingId) : IRequest;
