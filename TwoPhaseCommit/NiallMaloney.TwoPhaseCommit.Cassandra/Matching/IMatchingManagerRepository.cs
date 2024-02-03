namespace NiallMaloney.TwoPhaseCommit.Cassandra.Matching;

public interface IMatchingManagerRepository
{
    public Task AddManager(MatchingManagerRow manager);
    public Task UpdateManager(MatchingManagerRow manager);
    public Task<MatchingManagerRow?> GetManager(string matchingId);

    public Task<IEnumerable<MatchingManagerRow>> SearchManagers(
        string? matchingId = null,
        string? paymentId = null,
        string? expectationId = null,
        string? status = null
    );
}
