using MediatR;

namespace NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Commands;

public record AddCharge(string ChargeId, string CustomerId, decimal Amount) : IRequest;
