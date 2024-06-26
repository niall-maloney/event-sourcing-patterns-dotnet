namespace NiallMaloney.TwoPhaseCommit.Cassandra.Expectations;

public interface IExpectationsRepository
{
    public Task AddExpectation(ExpectationRow expectation);
    public Task UpdateExpectation(ExpectationRow expectation);
    public Task<ExpectationRow?> GetExpectation(string expectationId);

    public Task<IEnumerable<ExpectationRow>> SearchExpectations(
        string? expectationId = null,
        string? iban = null,
        decimal? amount = null,
        string? reference = null,
        string? status = null
    );
}