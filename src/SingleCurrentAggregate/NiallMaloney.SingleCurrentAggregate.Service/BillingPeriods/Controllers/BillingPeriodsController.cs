using MediatR;
using Microsoft.AspNetCore.Mvc;
using NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Commands;
using NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Queries;

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

    [HttpGet("{billingPeriodId}")]
    public async Task<IActionResult> GetBillingPeriod([FromRoute] string billingPeriodId)
    {
        var billingPeriod = await _mediator.Send(new GetBillingPeriod(billingPeriodId));
        if (billingPeriod is null) return NotFound();
        return Ok(billingPeriod);
    }

    [HttpGet]
    public async Task<IActionResult> SearchBillingPeriods(
        [FromQuery] string? billingPeriodId = null,
        [FromQuery] string? status = null)
    {
        var billingPeriods = await _mediator.Send(new SearchBillingPeriod(billingPeriodId, status));
        return Ok(billingPeriods);
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
        var billingPeriod = await _mediator.Send(new GetBillingPeriod(billingPeriodId));
        if (billingPeriod is null) return NotFound();
        await _mediator.Send(new CloseBillingPeriod(billingPeriodId));
        return Accepted(billingPeriodId);
    }
}
