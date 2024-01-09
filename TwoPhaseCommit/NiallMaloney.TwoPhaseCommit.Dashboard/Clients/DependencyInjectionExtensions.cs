namespace NiallMaloney.TwoPhaseCommit.Dashboard.Clients;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddMatchingClient(this IServiceCollection services, IConfiguration configuration)
    {
        var useStub = configuration.GetValue<bool>("UseStub");

        if (useStub)
        {
            services.AddTransient<IMatchingClient, StubMatchingClient>();
        }
        else
        {
            services.AddHttpClient<IMatchingClient, MatchingClient>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5038");
            });
        }

        return services;
    }
}
