using Microsoft.AspNetCore.Mvc.Testing;
using Polly;
using Xunit;

namespace NiallMaloney.Shared.TestUtils;

public class TestBase<TEntryPoint> : IClassFixture<WebApplicationFactory<TEntryPoint>> where TEntryPoint : class
{
    public TestBase(WebApplicationFactory<TEntryPoint> app)
    {
        Client = app.CreateClient();
    }

    protected HttpClient Client { get; }


    protected static async Task<T> RetryUntil<T>(
        Func<Task<T>> action,
        Func<T, bool> retryUntilPredicate,
        int retryCount = 50,
        int sleepDurationInMilliseconds = 100
    ) =>
        await Policy
            .HandleResult<T>(r => !retryUntilPredicate.Invoke(r))
            .WaitAndRetryAsync(
                retryCount,
                _ => TimeSpan.FromMilliseconds(sleepDurationInMilliseconds)
            )
            .ExecuteAsync(action);
}
