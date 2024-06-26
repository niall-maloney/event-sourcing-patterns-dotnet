using System.Collections.Frozen;

namespace NiallMaloney.FatEvents.Service.Notifications.Domain;

public static class NotificationTypes
{
    public const string UserAccepted = "UserAccepted";

    public static FrozenSet<string> All = new HashSet<string> { UserAccepted }.ToFrozenSet();
}