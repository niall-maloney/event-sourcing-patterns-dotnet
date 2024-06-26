using MediatR;

namespace NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Commands;

public record CloseBillingPeriod(string BillingPeriodId) : IRequest;