using Polly;

namespace NiallMaloney.Shared.TestUtils;

public abstract class TestHelpers
{
    public static async Task<T> RetryUntil<T>(
        Func<Task<T>> action,
        Func<T, bool> retryUntilPredicate,
        int retryCount = 50,
        int sleepDurationInMilliseconds = 100
    )
    {
        return await Policy
            .HandleResult<T>(r => !retryUntilPredicate.Invoke(r))
            .WaitAndRetryAsync(
                retryCount,
                _ => TimeSpan.FromMilliseconds(sleepDurationInMilliseconds)
            )
            .ExecuteAsync(action);
    }
}