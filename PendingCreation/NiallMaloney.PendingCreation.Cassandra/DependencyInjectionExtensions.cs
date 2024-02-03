using Cassandra.Mapping;
using Microsoft.Extensions.DependencyInjection;
using NiallMaloney.PendingCreation.Cassandra.Users;

namespace NiallMaloney.PendingCreation.Cassandra;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddCassandraRepositories(this IServiceCollection services)
    {
        MappingConfiguration.Global.Define<CassandraUsersMappings>();
        return services
            .AddSingleton<IUsersRepository, CassandraUsersRepository>()
            .AddSingleton<IDuplicateUserTrackingRepository, CassandraDuplicateUserTrackingRepository>();
    }
}
