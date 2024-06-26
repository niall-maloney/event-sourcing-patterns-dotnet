using MediatR;

namespace NiallMaloney.TwoPhaseCommit.Service.Payments.Commands;

public record ReleasePayment(string PaymentId, string MatchingId) : IRequest;