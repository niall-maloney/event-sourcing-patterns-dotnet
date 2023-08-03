using Microsoft.AspNetCore.Mvc;
using NiallMaloney.ProcessManager.Cassandra;

namespace NiallMaloney.ProcessManager.Service.Ledgers.Controllers;

[ApiController]
[Route("balances")]
public class BalancesController : ControllerBase
{
    private readonly ILogger<BalancesController> _logger;
    private readonly ILedgersRepository _repository;

    public BalancesController(ILogger<BalancesController> logger, ILedgersRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetLedger()
    {
        var ls = await _repository.GetLedgers();
        return Ok(ls);
    }

    [HttpGet("{ledger}")]
    public async Task<IActionResult> GetLedger(string ledger)
    {
        var l = await _repository.GetLedger(ledger);
        if (l is null) return NotFound();

        return Ok(l);
    }
}