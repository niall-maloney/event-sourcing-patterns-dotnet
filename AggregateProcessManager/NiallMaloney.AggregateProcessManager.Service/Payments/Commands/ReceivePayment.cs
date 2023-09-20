using MediatR;

namespace NiallMaloney.AggregateProcessManager.Service.Payments.Commands;

public record ReceivePayment(string PaymentId, string Iban, decimal Amount, string Reference)
    : IRequest;
