using MediatR;
using Microsoft.AspNetCore.Mvc;
using NiallMaloney.FatEvents.Service.Users.Commands;
using NiallMaloney.FatEvents.Service.Users.Controllers.Models;
using NiallMaloney.FatEvents.Service.Users.Queries;

namespace NiallMaloney.FatEvents.Service.Users.Controllers;

[ApiController]
[Route("users")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUser([FromRoute] string userId)
    {
        var row = await _mediator.Send(new GetUser(userId));
        if (row is null)
        {
            return NotFound();
        }

        return Ok(Models.User.Map(row));
    }

    [HttpGet]
    public async Task<IActionResult> SearchUsers(
        [FromQuery] string? emailAddress = null,
        [FromQuery] string? status = null)
    {
        var rows = await _mediator.Send(new SearchUsers(emailAddress, status));
        return Ok(Models.User.Map(rows));
    }

    [HttpPost]
    public async Task<IActionResult> AddUser(UserDefinition definition)
    {
        var userId = Ids.NewUserId();
        await _mediator.Send(new AddUser(userId, definition.EmailAddress, definition.Forename, definition.Surname));
        return Accepted(new UserReference(userId));
    }
}