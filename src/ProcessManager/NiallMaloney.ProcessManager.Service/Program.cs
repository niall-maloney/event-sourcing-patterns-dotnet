using NiallMaloney.EventSourcing;
using NiallMaloney.EventSourcing.Subscriptions;
using NiallMaloney.ProcessManager.Service.Ledgers;
using NiallMaloney.ProcessManager.Service.Ledgers.Repositories;

var builder = WebApplication.CreateBuilder(args);
var executingAssembly = typeof(Program).Assembly;

var eventStoreSection = builder.Configuration.GetSection("EventStore:ConnectionString");
builder.Services.AddEventStore(new EventStoreClientOptions(eventStoreSection.Value), new[] { executingAssembly });
builder.Services.AddSingleton<ISubscriptionCursorRepository, InMemoryCursorRepository>();

builder.Services.AddSingleton<IBalanceRepository, InMemoryBalanceRepository>();
builder.Services.AddSubscriber<LedgersProcessManager>();

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
