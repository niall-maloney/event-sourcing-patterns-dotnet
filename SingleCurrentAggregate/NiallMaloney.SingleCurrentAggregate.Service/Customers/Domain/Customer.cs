using NiallMaloney.EventSourcing.Aggregates;
using NiallMaloney.SingleCurrentAggregate.Service.Customers.Events;

namespace NiallMaloney.SingleCurrentAggregate.Service.Customers.Domain;

[Category("single_current_aggregate.customer")]
public class Customer : Aggregate
{
    private bool _isAdded;

    public Customer()
    {
        When<CustomerAdded>(Apply);
    }

    public void Add()
    {
        if (_isAdded)
        {
            return;
        }
        RaiseEvent(new CustomerAdded(Id));
    }

    private void Apply(CustomerAdded evnt)
    {
        _isAdded = true;
    }
}
