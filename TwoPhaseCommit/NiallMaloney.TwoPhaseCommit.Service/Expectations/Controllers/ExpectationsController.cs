using MediatR;
using Microsoft.AspNetCore.Mvc;
using NiallMaloney.TwoPhaseCommit.Service.Expectations.Commands;
using NiallMaloney.TwoPhaseCommit.Service.Expectations.Controllers.Models;
using NiallMaloney.TwoPhaseCommit.Service.Expectations.Queries;

namespace NiallMaloney.TwoPhaseCommit.Service.Expectations.Controllers;

[ApiController]
[Route("expectations")]
public class ExpectationsController : ControllerBase
{
    private readonly ILogger<ExpectationsController> _logger;
    private readonly IMediator _mediator;

    public ExpectationsController(ILogger<ExpectationsController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet("{expectationId}")]
    public async Task<IActionResult> GetExpectation([FromRoute] string expectationId)
    {
        var row = await _mediator.Send(new GetExpectation(expectationId));
        if (row is null)
        {
            return NotFound();
        }

        return Ok(Expectation.Map(row));
    }

    [HttpGet]
    public async Task<IActionResult> SearchExpectations(
        [FromQuery] string? expectationId = null,
        [FromQuery] string? iban = null,
        [FromQuery] decimal? amount = null,
        [FromQuery] string? reference = null,
        [FromQuery] string? status = null
    )
    {
        var rows = await _mediator.Send(
            new SearchExpectations(expectationId, iban, amount, reference, status)
        );
        return Ok(Expectation.Map(rows));
    }

    [HttpPost]
    public async Task<IActionResult> CreateExpectation([FromBody] ExpectationDefinition definition)
    {
        var expectationId = Ids.NewExpectationId();
        await _mediator.Send(
            new CreateExpectation(
                expectationId,
                definition.Iban,
                definition.Amount,
                definition.Reference
            )
        );
        return Accepted(new ExpectationReference(expectationId));
    }
}