using Microsoft.AspNetCore.Mvc;
using NiallMaloney.ProcessManager.Service.Ledgers.Repositories;

namespace NiallMaloney.ProcessManager.Service.Ledgers.Controllers;

[ApiController]
[Route("balances")]
public class BalancesController : ControllerBase
{
    private readonly ILogger<BalancesController> _logger;
    private readonly IBalanceRepository _repository;

    public BalancesController(ILogger<BalancesController> logger, IBalanceRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    [HttpGet("{ledger}")]
    public async Task<IActionResult> GetBalance(string ledger)
    {
        var balance = await _repository.GetBalance(ledger);
        if (balance is null)
        {
            return NotFound();
        }

        return Ok(balance);
    }
}
