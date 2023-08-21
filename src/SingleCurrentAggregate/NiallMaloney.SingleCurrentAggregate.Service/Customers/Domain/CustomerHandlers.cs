using MediatR;
using NiallMaloney.EventSourcing.Aggregates;
using NiallMaloney.SingleCurrentAggregate.Service.Customers.Commands;

namespace NiallMaloney.SingleCurrentAggregate.Service.Customers.Domain;

public class CustomerHandlers : IRequestHandler<AddCustomer>
{
    private readonly AggregateRepository _aggregateRepository;

    public CustomerHandlers(AggregateRepository aggregateRepository)
    {
        _aggregateRepository = aggregateRepository;
    }

    public async Task Handle(AddCustomer request, CancellationToken cancellationToken)
    {
        var customer = await _aggregateRepository.LoadAggregate<Customer>(request.CustomerId);
        customer.Add();
        await _aggregateRepository.SaveAggregate(customer);
    }
}