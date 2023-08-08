using Cassandra;
using Cassandra.Data.Linq;
using Cassandra.Mapping;

namespace NiallMaloney.ProcessManager.Cassandra;

public class CassandraBookingRepository : IBookingsRepository
{
    private readonly Mapper _mapper;
    private readonly ISession _session;

    public CassandraBookingRepository()
    {
        var cluster = Cluster.Builder().AddContactPoint("localhost").WithPort(9042).Build();
        _session = cluster.Connect("process_manager");
        _mapper = new Mapper(_session);
        CreateTables();
    }

    public Task AddBooking(BookingRow booking) => _mapper.InsertAsync(booking);

    public Task UpdateBooking(BookingRow booking) => _mapper.UpdateAsync(booking);

    public Task<BookingRow?> GetBooking(string bookingId) =>
        _mapper.SingleOrDefaultAsync<BookingRow?>("SELECT * FROM bookings where bookingId=?", bookingId);

    public Task<IEnumerable<BookingRow>> SearchBookings(
        string? bookingId = null,
        string? ledger = null,
        string? status = null)
    {
        CqlQuery<BookingRow> bookings = new Table<BookingRow>(_session);

        if (!string.IsNullOrEmpty(bookingId))
        {
            bookings = bookings.Where(b => b.BookingId == bookingId);
        }

        if (!string.IsNullOrEmpty(ledger))
        {
            bookings = bookings.Where(b => b.Ledger == ledger).AllowFiltering();
        }

        if (!string.IsNullOrEmpty(status))
        {
            bookings = bookings.Where(b => b.Status == status).AllowFiltering();
        }

        return bookings.ExecuteAsync();
    }

    private void CreateTables()
    {
        //CREATE TABLE IF NOT EXISTS process_manager.bookings ( bookingId text PRIMARY KEY, ledger text, amount decimal, status text, version varint );
        _session.Execute(
            "CREATE TABLE IF NOT EXISTS bookings ( bookingId text PRIMARY KEY, ledger text, amount decimal, status text, version varint )");
    }
}