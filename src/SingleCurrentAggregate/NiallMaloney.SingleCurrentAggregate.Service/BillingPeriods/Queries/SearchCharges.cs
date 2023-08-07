using MediatR;
using NiallMaloney.SingleCurrentAggregate.Cassandra;

namespace NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Queries;

public record SearchCharges
    (string? ChargeId, string? BillingPeriodId = null, string? Status = null) : IRequest<IEnumerable<ChargeRow>>;