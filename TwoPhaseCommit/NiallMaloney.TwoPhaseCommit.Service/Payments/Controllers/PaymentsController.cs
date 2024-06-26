using MediatR;
using Microsoft.AspNetCore.Mvc;
using NiallMaloney.TwoPhaseCommit.Service.Payments.Commands;
using NiallMaloney.TwoPhaseCommit.Service.Payments.Controllers.Models;
using NiallMaloney.TwoPhaseCommit.Service.Payments.Queries;

namespace NiallMaloney.TwoPhaseCommit.Service.Payments.Controllers;

[ApiController]
[Route("payments")]
public class PaymentsController : ControllerBase
{
    private readonly ILogger<PaymentsController> _logger;
    private readonly IMediator _mediator;

    public PaymentsController(ILogger<PaymentsController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet("{paymentId}")]
    public async Task<IActionResult> GetPayment([FromRoute] string paymentId)
    {
        var row = await _mediator.Send(new GetPayment(paymentId));
        if (row is null)
        {
            return NotFound();
        }

        return Ok(Payment.Map(row));
    }

    [HttpGet]
    public async Task<IActionResult> SearchPayments(
        [FromQuery] string? paymentId = null,
        [FromQuery] string? iban = null,
        [FromQuery] decimal? amount = null,
        [FromQuery] string? reference = null,
        [FromQuery] string? status = null
    )
    {
        var rows = await _mediator.Send(
            new SearchPayments(paymentId, iban, amount, reference, status)
        );
        return Ok(Payment.Map(rows));
    }

    [HttpPost]
    public async Task<IActionResult> ReceivePayment(PaymentDefinition definition)
    {
        var paymentId = Ids.NewPaymentId();
        await _mediator.Send(
            new ReceivePayment(paymentId, definition.Iban, definition.Amount, definition.Reference)
        );
        return Accepted(new PaymentReference(paymentId));
    }
}