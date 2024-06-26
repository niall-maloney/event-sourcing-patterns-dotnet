using MediatR;
using Microsoft.AspNetCore.Mvc;
using NiallMaloney.TwoPhaseCommit.Service.Matching.Commands;
using NiallMaloney.TwoPhaseCommit.Service.Matching.Controllers.Models;
using NiallMaloney.TwoPhaseCommit.Service.Matching.Queries;

namespace NiallMaloney.TwoPhaseCommit.Service.Matching.Controllers;

[ApiController]
[Route("matching-managers")]
public class MatchingManagersController : ControllerBase
{
    private readonly ILogger<MatchingManagersController> _logger;
    private readonly IMediator _mediator;

    public MatchingManagersController(
        ILogger<MatchingManagersController> logger,
        IMediator mediator
    )
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet("{matchingId}")]
    public async Task<IActionResult> GetMatchingManager([FromRoute] string matchingId)
    {
        var row = await _mediator.Send(new GetManager(matchingId));
        if (row is null)
        {
            return NotFound();
        }

        return Ok(MatchingManager.Map(row));
    }

    [HttpGet]
    public async Task<IActionResult> SearchPayments(
        [FromQuery] string? matchingId = null,
        [FromQuery] string? paymentId = null,
        [FromQuery] string? expectationId = null,
        [FromQuery] string? iban = null,
        [FromQuery] decimal? amount = null,
        [FromQuery] string? reference = null,
        [FromQuery] string? status = null
    )
    {
        var rows = await _mediator.Send(
            new SearchManagers(matchingId, paymentId, expectationId, iban, amount, reference, status)
        );
        return Ok(MatchingManager.Map(rows));
    }

    [HttpPost]
    public async Task<IActionResult> BeingMatching([FromBody] MatchingDefinition definition)
    {
        var matchingId = Ids.NewMatchingId();
        await _mediator.Send(
            new BeginMatching(
                matchingId,
                definition.ExpectationId,
                definition.PaymentId,
                definition.Iban,
                definition.Amount,
                definition.Reference
            )
        );
        return Accepted(new MatchingReference(matchingId));
    }
}