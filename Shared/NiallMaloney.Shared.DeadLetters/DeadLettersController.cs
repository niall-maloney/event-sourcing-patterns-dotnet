using Microsoft.AspNetCore.Mvc;
using NiallMaloney.EventSourcing.DeadLetters;

namespace NiallMaloney.Shared.DeadLetters;

[ApiController]
[Route("dead-letters")]
public class DeadLettersController : ControllerBase
{
    private readonly DeadLetterManager _manager;

    public DeadLettersController(DeadLetterManager manager)
    {
        _manager = manager;
    }

    [HttpGet("{subscriberName}")]
    public async Task<IActionResult> GetDeadLetters([FromRoute] string subscriberName)
    {
        var deadLetters = await _manager.GetAllDeadLettersForSubscriber(subscriberName);
        return Ok(deadLetters);
    }

    [HttpPost("/actions/redeliver-all")]
    public async Task<IActionResult> RedeliverAllDeadLettersForSubscriber(
        [FromQuery] string subscriberName)
    {
        await _manager.RedeliverAllDeadLettersForSubscriber(subscriberName);
        return Ok();
    }

    [HttpPost("{deadLetterId}/actions/redeliver")]
    public async Task<IActionResult> RedeliverDeadLetter([FromRoute] string deadLetterId)
    {
        await _manager.RedeliverDeadLetter(deadLetterId);
        return Ok();
    }
}
