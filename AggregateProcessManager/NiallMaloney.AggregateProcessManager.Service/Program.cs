using NiallMaloney.AggregateProcessManager.Service.Matching.Domain;
using NiallMaloney.EventSourcing;
using NiallMaloney.EventSourcing.Subscriptions;
using NiallMaloney.Shared.Cassandra;

var builder = WebApplication.CreateBuilder(args);
var executingAssembly = typeof(Program).Assembly;

var eventStoreSection = builder.Configuration.GetSection("EventStore:ConnectionString");
builder.Services.AddEventStore(new EventStoreClientOptions(eventStoreSection.Value), new[] { executingAssembly });
builder.Services.AddCassandraCursorRepository("aggregate_process_manager");
// builder.Services.AddCassandraRepositories();

builder.Services.AddSubscriber<MatchingProcessManager>();

builder.Services.AddMediatR(cfg => { cfg.RegisterServicesFromAssembly(executingAssembly); });

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// for tests
public partial class Program
{
}
