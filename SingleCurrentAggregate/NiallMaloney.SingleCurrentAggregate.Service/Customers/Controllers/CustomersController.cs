using MediatR;
using Microsoft.AspNetCore.Mvc;
using NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods;
using NiallMaloney.SingleCurrentAggregate.Service.Customers.Commands;
using NiallMaloney.SingleCurrentAggregate.Service.Customers.Controllers.Models;

namespace NiallMaloney.SingleCurrentAggregate.Service.Customers.Controllers;

[ApiController]
[Route("/customers")]
public class CustomersController : ControllerBase
{
    private readonly ILogger<CustomersController> _logger;
    private readonly IMediator _mediator;

    public CustomersController(ILogger<CustomersController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> AddCustomer(CustomerDefinition definition)
    {
        var customerId = Ids.NewCustomerId();
        await _mediator.Send(new AddCustomer(customerId));
        return Accepted(new CustomerReference(customerId));
    }
}
