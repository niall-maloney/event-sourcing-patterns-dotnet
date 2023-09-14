using NiallMaloney.EventSourcing;
using NiallMaloney.EventSourcing.Projections;
using NiallMaloney.EventSourcing.Subscriptions;
using NiallMaloney.ProcessManager.Cassandra;
using NiallMaloney.ProcessManager.Service.Ledgers.Events;

namespace NiallMaloney.ProcessManager.Service.Ledgers.Projections;

[SubscriberName("BookingsProjection")]
[Subscription("$ce-processor.booking")]
public class BookingsProjection : Projection
{
    private readonly IBookingsRepository _repository;

    public BookingsProjection(IBookingsRepository repository)
    {
        _repository = repository;
        When<BookingRequested>(Handle);
        When<BookingCommitted>(Handle);
        When<BookingRejected>(Handle);
    }

    private async Task Handle(BookingRequested evnt, EventMetadata metadata)
    {
        var booking = await _repository.GetBooking(evnt.BookingId);
        if (booking is not null)
        {
            return;
        }
        await _repository.AddBooking(new BookingRow
        {
            BookingId = evnt.BookingId,
            Ledger = evnt.Ledger,
            Amount = evnt.Amount,
            Status = "Pending",
            Version = metadata.StreamPosition
        });
    }

    private async Task Handle(BookingCommitted evnt, EventMetadata metadata)
    {
        var booking = await _repository.GetBooking(evnt.BookingId);
        if (booking is null || !TryUpdateVersion(booking, metadata.StreamPosition, out booking))
        {
            return;
        }
        booking = booking with
        {
            Status = "Committed"
        };
        await _repository.UpdateBooking(booking);
    }

    private async Task Handle(BookingRejected evnt, EventMetadata metadata)
    {
        var booking = await _repository.GetBooking(evnt.BookingId);
        if (booking is null || !TryUpdateVersion(booking, metadata.StreamPosition, out booking))
        {
            return;
        }
        booking = booking with
        {
            Status = "Rejected"
        };
        await _repository.UpdateBooking(booking);
    }

    private bool TryUpdateVersion(
        BookingRow booking,
        ulong newVersion,
        out BookingRow newBooking)
    {
        var expectedVersion = newVersion - 1;
        var actualVersion = booking.Version;
        if (actualVersion >= newVersion)
        {
            newBooking = booking;
            return false;
        }
        if (actualVersion != expectedVersion)
        {
            throw new InvalidOperationException($"Version mismatch, expected {expectedVersion} actual {actualVersion}");
        }
        newBooking = booking with
        {
            Version = newVersion
        };
        return true;
    }
}
