using MediatR;

namespace NiallMaloney.TwoPhaseCommit.Service.Payments.Commands;

public record ReceivePayment(string PaymentId, string Iban, decimal Amount, string Reference)
    : IRequest;