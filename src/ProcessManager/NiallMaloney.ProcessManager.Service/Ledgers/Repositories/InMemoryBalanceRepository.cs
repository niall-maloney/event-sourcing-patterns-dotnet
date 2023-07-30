using NiallMaloney.ProcessManager.Service.Ledgers.Models;

namespace NiallMaloney.ProcessManager.Service.Ledgers.Repositories;

public class InMemoryBalanceRepository : IBalanceRepository
{
    private readonly Dictionary<string, decimal> _ledgers = new();

    public Task<Balance?> GetBalance(string ledger)
    {
        return Task.FromResult(_ledgers.Where(l => l.Key == ledger)
            .Select(l => new Balance(l.Key, l.Value)).SingleOrDefault());
    }

    public Task<(bool, decimal)> UpdateBalance(string ledger, decimal amount)
    {
        if (!_ledgers.TryGetValue(ledger, out var balance))
        {
            balance = 0;
            _ledgers.Add(ledger, balance);
        }

        if (balance + amount < 0)
        {
            return Task.FromResult((false, _ledgers[ledger]));
        }

        _ledgers[ledger] += amount;

        return Task.FromResult((true, _ledgers[ledger]));
    }
}
