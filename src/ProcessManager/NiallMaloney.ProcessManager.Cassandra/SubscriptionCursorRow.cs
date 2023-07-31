namespace NiallMaloney.ProcessManager.Cassandra;

record SubscriptionCursorRow(string Subscription, ulong? Position);