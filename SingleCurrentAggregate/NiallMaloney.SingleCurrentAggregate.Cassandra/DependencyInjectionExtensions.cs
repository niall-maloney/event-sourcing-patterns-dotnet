using Cassandra.Mapping;
using Microsoft.Extensions.DependencyInjection;

namespace NiallMaloney.SingleCurrentAggregate.Cassandra;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddCassandraRepositories(this IServiceCollection services)
    {
        MappingConfiguration.Global.Define<CassandraMappings>();
        return services.AddSingleton<IBillingPeriodsRepository, CassandraBillingPeriodsRepository>()
            .AddSingleton<IChargesRepository, CassandraChargesRepository>();
    }
}