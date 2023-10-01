using NiallMaloney.TwoPhaseCommit.Cassandra;
using NiallMaloney.TwoPhaseCommit.Service.Expectations.Projections;
using NiallMaloney.TwoPhaseCommit.Service.Matching.Domain;
using NiallMaloney.TwoPhaseCommit.Service.Payments.Projections;
using NiallMaloney.EventSourcing;
using NiallMaloney.EventSourcing.Subscriptions;
using NiallMaloney.Shared.Cassandra;
using NiallMaloney.TwoPhaseCommit.Service.Matching.Projections;

var builder = WebApplication.CreateBuilder(args);
var executingAssembly = typeof(Program).Assembly;

var eventStoreSection = builder.Configuration.GetSection("EventStore:ConnectionString");
builder.Services.AddEventStore(
    new EventStoreClientOptions(eventStoreSection.Value),
    new[] { executingAssembly }
);
builder.Services.AddCassandraCursorRepository(Configuration.Keyspace);
builder.Services.AddCassandraRepositories();

builder.Services.AddSubscriber<MatchingProcessManager>();
builder.Services.AddSubscriber<MatchingManagerProjection>();
builder.Services.AddSubscriber<PaymentsProjection>();
builder.Services.AddSubscriber<ExpectationsProjection>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(executingAssembly);
});

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) { }

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// for tests
public partial class Program { }
