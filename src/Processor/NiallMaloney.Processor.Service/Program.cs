using NiallMaloney.EventSourcing;
using NiallMaloney.EventSourcing.Subscriptions;
using NiallMaloney.Processor.Cassandra;
using NiallMaloney.Processor.Service.Ledgers;
using NiallMaloney.Processor.Service.Ledgers.Projections;
using NiallMaloney.Shared.Cassandra;

var builder = WebApplication.CreateBuilder(args);
var executingAssembly = typeof(Program).Assembly;

var eventStoreSection = builder.Configuration.GetSection("EventStore:ConnectionString");
builder.Services.AddEventStore(new EventStoreClientOptions(eventStoreSection.Value), new[] { executingAssembly });
builder.Services.AddCassandraCursorRepository("processor");

builder.Services.AddCassandraRepositories();
builder.Services.AddSubscriber<BookingsProjection>();
builder.Services.AddSubscriber<LedgersProcessor>();

builder.Services.AddMediatR(cfg => { cfg.RegisterServicesFromAssembly(executingAssembly); });

builder.Services.AddControllers();

var app = builder.Build();
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
