using MediatR;

namespace NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Commands;

public record OpenBillingPeriod(string BillingPeriodId, string CustomerId) : IRequest;