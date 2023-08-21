namespace NiallMaloney.SingleCurrentAggregate.Cassandra;

public interface IBillingPeriodsRepository
{
    public Task<BillingPeriodRow?> GetBillingPeriod(string billingPeriodId);

    public Task<IEnumerable<BillingPeriodRow>> SearchBillingPeriods(
        string? billingPeriodId = null,
        string? customerId = null,
        string? status = null);

    public Task AddBillingPeriod(BillingPeriodRow billingPeriod);
    public Task UpdateBillingPeriod(BillingPeriodRow billingPeriod);
}