using MediatR;
using Microsoft.AspNetCore.Mvc;
using NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Commands;
using NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Controllers.Models;
using NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Queries;

namespace NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Controllers;

[ApiController]
[Route("charges")]
public class ChargesController : ControllerBase
{
    private readonly ILogger<ChargesController> _logger;
    private readonly IMediator _mediator;

    public ChargesController(ILogger<ChargesController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet("{chargeId}")]
    public async Task<IActionResult> GetCharge([FromRoute] string chargeId)
    {
        var row = await _mediator.Send(new GetCharge(chargeId));
        if (row is null)
        {
            return NotFound();
        }
        return Ok(Charge.Map(row));
    }

    [HttpGet]
    public async Task<IActionResult> SearchCharges(
        [FromQuery] string? chargeId = null,
        [FromQuery] string? billingPeriodId = null,
        [FromQuery] string? status = null)
    {
        var rows = await _mediator.Send(new SearchCharges(chargeId, billingPeriodId, status));
        return Ok(Charge.Map(rows));
    }

    [HttpPost]
    public async Task<IActionResult> CreateCharge(ChargeDefinition definition)
    {
        var chargeId = Ids.NewChargeId();
        await _mediator.Send(new AddCharge(chargeId, definition.Amount));
        return Accepted(chargeId);
    }

    [HttpPost("{chargeId}/actions/remove")]
    public async Task<IActionResult> CreateCharge(string chargeId)
    {
        await _mediator.Send(new RemoveCharge(chargeId));
        return Accepted(chargeId);
    }
}