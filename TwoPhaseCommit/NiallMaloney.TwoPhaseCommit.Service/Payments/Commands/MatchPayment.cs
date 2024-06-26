using MediatR;

namespace NiallMaloney.TwoPhaseCommit.Service.Payments.Commands;

public record MatchPayment(string PaymentId, string ExpectationId, string MatchingId) : IRequest;