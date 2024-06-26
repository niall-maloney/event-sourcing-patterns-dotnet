namespace NiallMaloney.SingleCurrentAggregate.Cassandra;

public interface IChargesRepository
{
    public Task<ChargeRow> GetCharge(string chargeId);

    public Task<IEnumerable<ChargeRow>> SearchCharges(
        string? chargeId = null,
        string? billingPeriodId = null,
        string? status = null
    );

    public Task AddCharge(ChargeRow charge);
    public Task UpdateCharge(ChargeRow charge);
}