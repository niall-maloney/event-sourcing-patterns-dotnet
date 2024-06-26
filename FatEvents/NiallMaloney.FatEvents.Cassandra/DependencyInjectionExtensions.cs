using Cassandra.Mapping;
using Microsoft.Extensions.DependencyInjection;
using NiallMaloney.FatEvents.Cassandra.Notifications;
using NiallMaloney.FatEvents.Cassandra.Users;

namespace NiallMaloney.FatEvents.Cassandra;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddCassandraRepositories(this IServiceCollection services)
    {
        MappingConfiguration.Global.Define<CassandraUsersMappings>();
        MappingConfiguration.Global.Define<CassandraNotificationsMappings>();
        return services
            .AddSingleton<IUsersRepository, CassandraUsersRepository>()
            .AddSingleton<INotificationsRepository, CassandraNotificationsRepository>();
    }
}