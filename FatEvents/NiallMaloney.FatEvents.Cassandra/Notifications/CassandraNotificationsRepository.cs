using Cassandra;
using Cassandra.Data.Linq;
using Cassandra.Mapping;

namespace NiallMaloney.FatEvents.Cassandra.Notifications;

public class CassandraNotificationsRepository : INotificationsRepository
{
    private readonly Mapper _mapper;
    private readonly ISession _session;

    public CassandraNotificationsRepository()
    {
        var cluster = Cluster.Builder().AddContactPoint("localhost").WithPort(9042).Build();
        _session = cluster.Connect(Configuration.Keyspace);
        _mapper = new Mapper(_session);
        CreateTables();
    }

    public Task AddNotification(NotificationRow notification)
    {
        return _mapper.InsertAsync(notification);
    }

    public Task UpdateNotification(NotificationRow notification)
    {
        return _mapper.UpdateAsync(notification);
    }

    public Task<NotificationRow?> GetNotification(string notificationId)
    {
        return _mapper.SingleOrDefaultAsync<NotificationRow?>(
            "SELECT * FROM notifications where notificationId=?",
            notificationId
        );
    }

    public Task<IEnumerable<NotificationRow>> SearchNotifications(string? type, string? status)
    {
        CqlQuery<NotificationRow> notifications = new Table<NotificationRow>(_session);

        if (!string.IsNullOrEmpty(type))
        {
            notifications = notifications.Where(b => b.Type == type).AllowFiltering();
        }

        if (!string.IsNullOrEmpty(status))
        {
            notifications = notifications.Where(b => b.Status == status).AllowFiltering();
        }

        return notifications.ExecuteAsync();
    }

    private void CreateTables()
    {
        //CREATE TABLE IF NOT EXISTS notifications ( notificationId text PRIMARY KEY, type text, status text, version varint )
        _session.Execute(
            "CREATE TABLE IF NOT EXISTS notifications ( notificationId text PRIMARY KEY, type text, status text, version varint )"
        );
    }
}