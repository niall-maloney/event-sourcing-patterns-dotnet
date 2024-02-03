using NiallMaloney.EventSourcing;
using NiallMaloney.EventSourcing.Projections;
using NiallMaloney.EventSourcing.Subscriptions;
using NiallMaloney.TwoPhaseCommit.Cassandra;
using NiallMaloney.TwoPhaseCommit.Cassandra.Matching;
using NiallMaloney.TwoPhaseCommit.Service.Matching.Events;

namespace NiallMaloney.TwoPhaseCommit.Service.Matching.Projections;

[SubscriberName("MatchingManagerProjection")]
[Subscription("$ce-two_phase_commit.matching_manager")]
public class MatchingManagerProjection : Projection
{
    private readonly IMatchingManagerRepository _repository;

    public MatchingManagerProjection(IMatchingManagerRepository repository)
    {
        _repository = repository;

        When<MatchingStarted>(Handle);
        When<PaymentReserving>(Handle);
        When<PaymentReserved>(Handle);
        When<PaymentReservationRejected>(Handle);
        When<ExpectationReserving>(Handle);
        When<ExpectationReserved>(Handle);
        When<ExpectationReservationRejected>(Handle);
        When<PaymentMatchApplying>(Handle);
        When<PaymentMatchApplied>(Handle);
        When<ExpectationMatchApplying>(Handle);
        When<ExpectationMatchApplied>(Handle);
        When<MatchingCompleted>(Handle);
        When<MatchingFailed>(Handle);
    }

    private async Task Handle(MatchingStarted evnt, EventMetadata metadata)
    {
        var manager = await _repository.GetManager(evnt.MatchingId);
        if (manager is not null)
        {
            return;
        }

        await _repository.AddManager(new MatchingManagerRow
            {
                MatchingId = evnt.MatchingId,
                PaymentId = evnt.PaymentId,
                ExpectationId = evnt.ExpectationId,
                Iban = evnt.Iban,
                Amount = evnt.Amount,
                Reference = evnt.Reference,
                Status = "Pending",
                Version = metadata.StreamPosition
            }
        );
    }

    private async Task Handle(PaymentReserving evnt, EventMetadata metadata)
    {
        var manager = await _repository.GetManager(evnt.MatchingId);
        if (manager is null || !TryUpdateVersion(manager, metadata.StreamPosition, out manager))
        {
            return;
        }

        manager = manager with { Status = "Payment Reserving" };
        await _repository.UpdateManager(manager);
    }

    private async Task Handle(PaymentReserved evnt, EventMetadata metadata)
    {
        var manager = await _repository.GetManager(evnt.MatchingId);
        if (manager is null || !TryUpdateVersion(manager, metadata.StreamPosition, out manager))
        {
            return;
        }

        manager = manager with { Status = "Payment Reserved" };
        await _repository.UpdateManager(manager);
    }

    private async Task Handle(PaymentReservationRejected evnt, EventMetadata metadata)
    {
        var manager = await _repository.GetManager(evnt.MatchingId);
        if (manager is null || !TryUpdateVersion(manager, metadata.StreamPosition, out manager))
        {
            return;
        }

        await _repository.UpdateManager(manager);
    }

    private async Task Handle(ExpectationReserving evnt, EventMetadata metadata)
    {
        var manager = await _repository.GetManager(evnt.MatchingId);
        if (manager is null || !TryUpdateVersion(manager, metadata.StreamPosition, out manager))
        {
            return;
        }

        manager = manager with { Status = "Expectation Reserving" };
        await _repository.UpdateManager(manager);
    }

    private async Task Handle(ExpectationReserved evnt, EventMetadata metadata)
    {
        var manager = await _repository.GetManager(evnt.MatchingId);
        if (manager is null || !TryUpdateVersion(manager, metadata.StreamPosition, out manager))
        {
            return;
        }

        manager = manager with { Status = "Expectation Reserved" };
        await _repository.UpdateManager(manager);
    }

    private async Task Handle(ExpectationReservationRejected evnt, EventMetadata metadata)
    {
        var manager = await _repository.GetManager(evnt.MatchingId);
        if (manager is null || !TryUpdateVersion(manager, metadata.StreamPosition, out manager))
        {
            return;
        }

        await _repository.UpdateManager(manager);
    }

    private async Task Handle(PaymentMatchApplying evnt, EventMetadata metadata)
    {
        var manager = await _repository.GetManager(evnt.MatchingId);
        if (manager is null || !TryUpdateVersion(manager, metadata.StreamPosition, out manager))
        {
            return;
        }

        manager = manager with { Status = "Payment Applying" };
        await _repository.UpdateManager(manager);
    }

    private async Task Handle(PaymentMatchApplied evnt, EventMetadata metadata)
    {
        var manager = await _repository.GetManager(evnt.MatchingId);
        if (manager is null || !TryUpdateVersion(manager, metadata.StreamPosition, out manager))
        {
            return;
        }

        manager = manager with { Status = "Payment Applied" };
        await _repository.UpdateManager(manager);
    }

    private async Task Handle(ExpectationMatchApplying evnt, EventMetadata metadata)
    {
        var manager = await _repository.GetManager(evnt.MatchingId);
        if (manager is null || !TryUpdateVersion(manager, metadata.StreamPosition, out manager))
        {
            return;
        }

        manager = manager with { Status = "Expectation Applying" };
        await _repository.UpdateManager(manager);
    }

    private async Task Handle(ExpectationMatchApplied evnt, EventMetadata metadata)
    {
        var manager = await _repository.GetManager(evnt.MatchingId);
        if (manager is null || !TryUpdateVersion(manager, metadata.StreamPosition, out manager))
        {
            return;
        }

        manager = manager with { Status = "Expectation Applied" };
        await _repository.UpdateManager(manager);
    }

    private async Task Handle(MatchingCompleted evnt, EventMetadata metadata)
    {
        var manager = await _repository.GetManager(evnt.MatchingId);
        if (manager is null || !TryUpdateVersion(manager, metadata.StreamPosition, out manager))
        {
            return;
        }

        manager = manager with { Status = "Completed" };
        await _repository.UpdateManager(manager);
    }

    private async Task Handle(MatchingFailed evnt, EventMetadata metadata)
    {
        var manager = await _repository.GetManager(evnt.MatchingId);
        if (manager is null || !TryUpdateVersion(manager, metadata.StreamPosition, out manager))
        {
            return;
        }

        manager = manager with { Status = "Failed" };
        await _repository.UpdateManager(manager);
    }

    private bool TryUpdateVersion(MatchingManagerRow manager, ulong newVersion, out MatchingManagerRow newManager)
    {
        var expectedVersion = newVersion - 1;
        var actualVersion = manager.Version;
        if (actualVersion >= newVersion)
        {
            newManager = manager;
            return false;
        }
        if (actualVersion != expectedVersion)
        {
            throw new InvalidOperationException(
                $"Version mismatch, expected {expectedVersion} actual {actualVersion}"
            );
        }
        newManager = manager with { Version = newVersion };
        return true;
    }
}
