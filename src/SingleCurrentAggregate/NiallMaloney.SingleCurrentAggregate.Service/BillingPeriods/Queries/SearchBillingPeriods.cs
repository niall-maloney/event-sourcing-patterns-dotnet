using MediatR;
using NiallMaloney.SingleCurrentAggregate.Cassandra;

namespace NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Queries;

public record SearchBillingPeriods
(
    string? BillingPeriodId = null,
    string? CustomerId = null,
    string? Status = null) : IRequest<IEnumerable<BillingPeriodRow>>;
