namespace NiallMaloney.Shared.Cassandra;

internal record SubscriptionCursorRow(string Subscription, ulong? Position);
