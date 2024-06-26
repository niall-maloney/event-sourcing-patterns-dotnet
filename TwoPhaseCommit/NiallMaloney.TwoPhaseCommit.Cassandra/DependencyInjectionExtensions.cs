using Cassandra.Mapping;
using Microsoft.Extensions.DependencyInjection;
using NiallMaloney.TwoPhaseCommit.Cassandra.Expectations;
using NiallMaloney.TwoPhaseCommit.Cassandra.Matching;
using NiallMaloney.TwoPhaseCommit.Cassandra.Payments;

namespace NiallMaloney.TwoPhaseCommit.Cassandra;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddCassandraRepositories(this IServiceCollection services)
    {
        MappingConfiguration.Global.Define<CassandraPaymentsMappings>();
        MappingConfiguration.Global.Define<CassandraExpectationsMapping>();
        MappingConfiguration.Global.Define<CassandraMatchingMappings>();
        return services
            .AddSingleton<IPaymentsRepository, CassandraPaymentsRepository>()
            .AddSingleton<IExpectationsRepository, CassandraExpectationsRepository>()
            .AddSingleton<IMatchingManagerRepository, CassandraMatchingManagerRepository>();
    }
}