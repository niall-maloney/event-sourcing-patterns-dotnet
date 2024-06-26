using NiallMaloney.EventSourcing;
using NiallMaloney.EventSourcing.Subscriptions;
using NiallMaloney.FatEvents.Cassandra;
using NiallMaloney.FatEvents.Service.Notifications.Domain;
using NiallMaloney.FatEvents.Service.Notifications.Projections;
using NiallMaloney.FatEvents.Service.Notifications.Services;
using NiallMaloney.FatEvents.Service.Users.Domain;
using NiallMaloney.FatEvents.Service.Users.Projections;
using NiallMaloney.Shared.Cassandra;

var builder = WebApplication.CreateBuilder(args);
var executingAssembly = typeof(Program).Assembly;

var eventStoreSection = builder.Configuration.GetSection("EventStore:ConnectionString");
builder.Services.AddEventStore(
    new EventStoreClientOptions(eventStoreSection.Value),
    [executingAssembly]
);

builder.Services.AddCassandraCursorRepository(Configuration.Keyspace);
builder.Services.AddCassandraRepositories();

builder.Services.AddSubscriber<UsersProjection>();
builder.Services.AddSubscriber<UserProcessManager>();

builder.Services.AddTransient<INotificationService, NullNotificationService>();
builder.Services.AddSubscriber<NotificationsProjection>();
builder.Services.AddSubscriber<NotificationProcessManager>();

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