using MediatR;
using Microsoft.AspNetCore.Mvc;
using NiallMaloney.AggregateProcessManager.Service.Expectations.Commands;
using NiallMaloney.AggregateProcessManager.Service.Expectations.Controllers.Models;

namespace NiallMaloney.AggregateProcessManager.Service.Expectations.Controllers;

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

    [HttpPost]
    public async Task<IActionResult> CreateExpectation([FromBody] ExpectationDefinition definition)
    {
        var expectationId = Ids.NewExpectationId();
        await _mediator.Send(new CreateExpectation(expectationId, definition.Iban, definition.Amount,
            definition.Reference));
        return Accepted(new ExpectationReference(expectationId));
    }
}
