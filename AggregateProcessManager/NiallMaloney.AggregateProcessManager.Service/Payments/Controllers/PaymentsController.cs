using MediatR;
using Microsoft.AspNetCore.Mvc;
using NiallMaloney.AggregateProcessManager.Service.Payments.Commands;
using NiallMaloney.AggregateProcessManager.Service.Payments.Controllers.Models;

namespace NiallMaloney.AggregateProcessManager.Service.Payments.Controllers;

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

    [HttpPost]
    public async Task<IActionResult> ReceivePayment(PaymentDefinition definition)
    {
        var paymentId = Ids.NewPaymentId();
        await _mediator.Send(new ReceivePayment(paymentId, definition.Iban, definition.Amount, definition.Reference));
        return Accepted(new PaymentReference(paymentId));
    }
}
