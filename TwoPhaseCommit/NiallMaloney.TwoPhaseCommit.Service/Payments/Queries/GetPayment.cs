using MediatR;
using NiallMaloney.TwoPhaseCommit.Cassandra;

namespace NiallMaloney.TwoPhaseCommit.Service.Payments.Queries;

public record GetPayment(string PaymentId) : IRequest<PaymentRow?>;
