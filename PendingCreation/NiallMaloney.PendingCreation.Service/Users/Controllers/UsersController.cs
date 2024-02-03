using MediatR;
using Microsoft.AspNetCore.Mvc;
using NiallMaloney.PendingCreation.Service.Users.Commands;
using NiallMaloney.PendingCreation.Service.Users.Controllers.Models;
using NiallMaloney.PendingCreation.Service.Users.Queries;

namespace NiallMaloney.PendingCreation.Service.Users.Controllers;

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
    public async Task<IActionResult> SearchUsers()
    {
        var rows = await _mediator.Send(new SearchUsers());
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