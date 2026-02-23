using Microsoft.Extensions.DependencyInjection;
using NiallMaloney.EventSourcing;
using NiallMaloney.EventSourcing.DeadLetters;
using NiallMaloney.EventSourcing.Subscriptions;

namespace NiallMaloney.Shared.Cassandra;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddCassandraCursorRepository(
        this IServiceCollection services,
        string keyspace
    )
    {
        return services.AddSingleton<ISubscriptionCursorRepository>(
            new CassandraSubscriptionCursorRepository(keyspace)
        );
    }

    public static IServiceCollection AddCassandraDeadLetterRepository(
        this IServiceCollection services, string keyspace
    )
    {
        return services.AddDeadLetters(sp =>
            new CassandraDeadLetterRepository(keyspace, sp.GetRequiredService<EventSerializer>()));
    }
}
