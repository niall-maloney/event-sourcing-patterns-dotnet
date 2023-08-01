using MediatR;

namespace NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Commands;

public record AddCharge(string ChargeId, decimal Amount) : IRequest;
