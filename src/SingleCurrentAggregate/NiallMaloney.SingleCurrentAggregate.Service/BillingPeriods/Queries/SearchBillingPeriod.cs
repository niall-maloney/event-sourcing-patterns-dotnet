using MediatR;
using NiallMaloney.SingleCurrentAggregate.Cassandra;

namespace NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Queries;

public record SearchBillingPeriod
    (string? BillingPeriodId = null, string? Status = null) : IRequest<IEnumerable<BillingPeriodRow>>;