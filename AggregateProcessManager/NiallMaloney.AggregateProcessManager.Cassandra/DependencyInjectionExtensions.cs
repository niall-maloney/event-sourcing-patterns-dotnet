using Cassandra.Mapping;
using Microsoft.Extensions.DependencyInjection;

namespace NiallMaloney.AggregateProcessManager.Cassandra;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddCassandraRepositories(this IServiceCollection services)
    {
        MappingConfiguration.Global.Define<CassandraMappings>();
        return services.AddSingleton<IPaymentsRepository, CassandraPaymentsRepository>()
            .AddSingleton<IExpectationsRepository, CassandraExpectationsRepository>();
    }
}
