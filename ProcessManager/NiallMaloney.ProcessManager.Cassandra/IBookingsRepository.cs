namespace NiallMaloney.ProcessManager.Cassandra;

public interface IBookingsRepository
{
    public Task AddBooking(BookingRow booking);
    public Task UpdateBooking(BookingRow booking);
    public Task<BookingRow?> GetBooking(string bookingId);

    public Task<IEnumerable<BookingRow>> SearchBookings(
        string? bookingId = null,
        string? ledger = null,
        string? status = null
    );
}
