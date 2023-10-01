using MediatR;
using NiallMaloney.SingleCurrentAggregate.Cassandra;

namespace NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Queries;

public record GetBillingPeriod(string BillingPeriodId) : IRequest<BillingPeriodRow>;
