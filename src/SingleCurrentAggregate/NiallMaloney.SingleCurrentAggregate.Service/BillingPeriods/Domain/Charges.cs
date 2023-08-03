namespace NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Domain;

public class Charges
{
    private readonly Dictionary<string, decimal> _charges = new();

    public bool Contains(string chargeId) => _charges.ContainsKey(chargeId);

    public decimal GetAmount(string chargeId) => _charges[chargeId];

    public void Add(string chargeId, decimal amount)
    {
        if (!_charges.TryAdd(chargeId, amount)) throw new InvalidOperationException("Charge already added.");
    }

    public void Remove(string chargeId)
    {
        if (!_charges.Remove(chargeId)) throw new InvalidOperationException("Charge already removed.");
    }
}