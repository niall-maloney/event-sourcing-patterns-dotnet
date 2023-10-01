using Microsoft.Extensions.DependencyInjection;
using NiallMaloney.EventSourcing.Subscriptions;

namespace NiallMaloney.Shared.Cassandra;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddCassandraCursorRepository(
        this IServiceCollection services,
        string keyspace
    ) =>
        services.AddSingleton<ISubscriptionCursorRepository>(
            new CassandraSubscriptionCursorRepository(keyspace)
        );
}
