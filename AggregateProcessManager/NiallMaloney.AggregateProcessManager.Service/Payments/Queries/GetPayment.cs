using MediatR;
using NiallMaloney.AggregateProcessManager.Cassandra;

namespace NiallMaloney.AggregateProcessManager.Service.Payments.Queries;

public record GetPayment(string PaymentId) : IRequest<PaymentRow?>;
