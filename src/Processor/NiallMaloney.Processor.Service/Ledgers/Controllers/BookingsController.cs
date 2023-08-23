using MediatR;
using Microsoft.AspNetCore.Mvc;
using NiallMaloney.Processor.Service.Ledgers.Commands;
using NiallMaloney.Processor.Service.Ledgers.Controllers.Models;
using NiallMaloney.Processor.Service.Ledgers.Queries;

namespace NiallMaloney.Processor.Service.Ledgers.Controllers;

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
    public async Task<IActionResult> RequestBooking([FromBody] BookingDefinition definition)
    {
        var bookingId = Guid.NewGuid().ToString();
        await _mediator.Send(new RequestBooking(bookingId, definition.Ledger, definition.Amount));
        return Accepted(new BookingReference(bookingId));
    }

    [HttpGet("{bookingId}")]
    public async Task<IActionResult> GetBooking([FromRoute] string bookingId)
    {
        var booking = await _mediator.Send(new GetBooking(bookingId));
        if (booking is null)
        {
            return NotFound();
        }
        return Ok(Booking.Map(booking));
    }

    [HttpGet]
    public async Task<IActionResult> SearchBookings(
        [FromQuery] string? bookingId = null,
        [FromQuery] string? ledger = null,
        [FromQuery] string? status = null)
    {
        var bookings = await _mediator.Send(new SearchBookings(bookingId, ledger, status));
        return Ok(Booking.Map(bookings));
    }
}
