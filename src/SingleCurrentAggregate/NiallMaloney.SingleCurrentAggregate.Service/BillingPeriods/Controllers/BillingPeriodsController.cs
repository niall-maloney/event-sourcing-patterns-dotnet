using MediatR;
using Microsoft.AspNetCore.Mvc;
using NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Commands;

namespace NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Controllers;

[ApiController]
[Route("billing-periods")]
public class BillingPeriodsController : ControllerBase
{
    private readonly ILogger<BillingPeriodsController> _logger;
    private readonly IMediator _mediator;

    public BillingPeriodsController(ILogger<BillingPeriodsController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> OpenBillingPeriod()
    {
        var billingPeriodId = Ids.NewBillingPeriodId();
        await _mediator.Send(new OpenBillingPeriod(billingPeriodId));
        return Accepted(billingPeriodId);
    }

    [HttpPost("{billingPeriodId}/actions/close")]
    public async Task<IActionResult> CloseBillingPeriod([FromRoute] string billingPeriodId)
    {
        //todo: look up in projection
        await _mediator.Send(new CloseBillingPeriod(billingPeriodId));
        return Accepted(billingPeriodId);
    }
}
