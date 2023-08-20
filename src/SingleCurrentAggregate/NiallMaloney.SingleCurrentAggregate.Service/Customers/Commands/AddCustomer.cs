using MediatR;

namespace NiallMaloney.SingleCurrentAggregate.Service.Customers.Commands;

public record AddCustomer(string CustomerId) : IRequest;