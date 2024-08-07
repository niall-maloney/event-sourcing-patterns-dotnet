using MediatR;

namespace NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Commands;

public record RemoveCharge(string ChargeId) : IRequest;