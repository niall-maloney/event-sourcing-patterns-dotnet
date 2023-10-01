using NiallMaloney.EventSourcing;
using NiallMaloney.EventSourcing.Subscriptions;
using NiallMaloney.ProcessManager.Cassandra;
using NiallMaloney.ProcessManager.Service.Ledgers;
using NiallMaloney.ProcessManager.Service.Ledgers.Projections;
using NiallMaloney.Shared.Cassandra;

var builder = WebApplication.CreateBuilder(args);
var executingAssembly = typeof(NiallMaloney.ProcessManager.Service.Program).Assembly;

var eventStoreSection = builder.Configuration.GetSection("EventStore:ConnectionString");
builder.Services.AddEventStore(
    new EventStoreClientOptions(eventStoreSection.Value),
    new[] { executingAssembly }
);
builder.Services.AddCassandraCursorRepository(Configuration.Keyspace);

builder.Services.AddCassandraRepositories();
builder.Services.AddSubscriber<BookingsProjection>();
builder.Services.AddSubscriber<LedgersProcessManager>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(executingAssembly);
});

builder.Services.AddControllers();

var app = builder.Build();
if (app.Environment.IsDevelopment()) { }

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

// for tests
namespace NiallMaloney.ProcessManager.Service
{
    public partial class Program { }
}
