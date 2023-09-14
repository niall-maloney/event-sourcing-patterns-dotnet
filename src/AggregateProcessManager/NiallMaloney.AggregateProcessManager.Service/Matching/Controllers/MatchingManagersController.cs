using MediatR;
using Microsoft.AspNetCore.Mvc;
using NiallMaloney.AggregateProcessManager.Service.Matching.Commands;
using NiallMaloney.AggregateProcessManager.Service.Matching.Controllers.Models;

namespace NiallMaloney.AggregateProcessManager.Service.Matching.Controllers;

[ApiController]
[Route("matching-managers")]
public class MatchingManagersController : ControllerBase
{
    private readonly ILogger<MatchingManagersController> _logger;
    private readonly IMediator _mediator;

    public MatchingManagersController(ILogger<MatchingManagersController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> BeingMatching([FromBody] MatchingDefinition definition)
    {
        var matchingId = Ids.NewMatchingId();
        await _mediator.Send(new BeginMatching(matchingId, definition.ExpectationId, definition.PaymentId,
            definition.Iban, definition.Amount, definition.Reference));
        return Accepted(new MatchingReference(matchingId));
    }
}
