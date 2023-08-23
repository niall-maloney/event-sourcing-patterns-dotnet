using Cassandra.Mapping;
using Microsoft.Extensions.DependencyInjection;

namespace NiallMaloney.Processor.Cassandra;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddCassandraRepositories(this IServiceCollection services)
    {
        MappingConfiguration.Global.Define<CassandraMappings>();
        return services.AddSingleton<ILedgersRepository, CassandraLedgersRepository>()
            .AddSingleton<IBookingsRepository, CassandraBookingRepository>();
    }
}
