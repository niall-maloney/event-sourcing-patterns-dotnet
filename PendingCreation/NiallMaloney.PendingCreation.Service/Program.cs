using NiallMaloney.EventSourcing;
using NiallMaloney.EventSourcing.Subscriptions;
using NiallMaloney.PendingCreation.Cassandra;
using NiallMaloney.PendingCreation.Service.Users.Domain;
using NiallMaloney.PendingCreation.Service.Users.Projections;
using NiallMaloney.Shared.Cassandra;

var builder = WebApplication.CreateBuilder(args);
var executingAssembly = typeof(Program).Assembly;

var eventStoreSection = builder.Configuration.GetSection("EventStore:ConnectionString");
builder.Services.AddEventStore(
    new EventStoreClientOptions(eventStoreSection.Value),
    new[] { executingAssembly }
);

builder.Services.AddCassandraCursorRepository(Configuration.Keyspace);
builder.Services.AddCassandraRepositories();

builder.Services.AddSubscriber<UsersProjection>();
builder.Services.AddSubscriber<UserProcessManager>();

builder.Services.AddMediatR(cfg => { cfg.RegisterServicesFromAssembly(executingAssembly); });

builder.Services.AddControllers();

var app = builder.Build();
if (app.Environment.IsDevelopment()) { }

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

// for tests
public partial class Program { }
