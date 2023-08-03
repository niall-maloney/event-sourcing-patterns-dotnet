using Microsoft.Extensions.DependencyInjection;

namespace NiallMaloney.SingleCurrentAggregate.Cassandra;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddCassandraRepositories(this IServiceCollection services) =>
        services.AddSingleton<IBillingPeriodsRepository, CassandraBillingPeriodsRepository>();
}