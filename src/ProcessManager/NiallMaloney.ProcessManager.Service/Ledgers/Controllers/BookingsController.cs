using MediatR;
using Microsoft.AspNetCore.Mvc;
using NiallMaloney.ProcessManager.Service.Ledgers.Commands;
using NiallMaloney.ProcessManager.Service.Ledgers.Controllers.Models;

namespace NiallMaloney.ProcessManager.Service.Ledgers.Controllers;

[ApiController]
[Route("bookings")]
public class BookingsController : ControllerBase
{
    private readonly ILogger<BookingsController> _logger;
    private readonly IMediator _mediator;

    public BookingsController(ILogger<BookingsController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> RequestBooking(BookingDefinition definition)
    {
        var bookingId = Guid.NewGuid().ToString();
        await _mediator.Send(new RequestBooking(bookingId, definition.Ledger, definition.Amount));
        return Accepted(bookingId);
    }
}