using NiallMaloney.EventSourcing;
using NiallMaloney.EventSourcing.Projections;
using NiallMaloney.EventSourcing.Subscriptions;
using NiallMaloney.TwoPhaseCommit.Cassandra;
using NiallMaloney.TwoPhaseCommit.Service.Expectations.Events;

namespace NiallMaloney.TwoPhaseCommit.Service.Expectations.Projections;

[SubscriberName("ExpectationsProjection")]
[Subscription("$ce-aggregate_process_manager.expectation")]
public class ExpectationsProjection : Projection
{
    private readonly IExpectationsRepository _repository;

    public ExpectationsProjection(IExpectationsRepository repository)
    {
        _repository = repository;

        When<ExpectationCreated>(Handle);
        When<ExpectationMatching>(Handle);
        When<ExpectationMatched>(Handle);
        When<ExpectationMatchRejected>(Handle);
    }

    private async Task Handle(ExpectationCreated evnt, EventMetadata metadata)
    {
        var expectation = await _repository.GetExpectation(evnt.ExpectationId);
        if (expectation is not null)
        {
            return;
        }

        await _repository.AddExpectation(
            new ExpectationRow
            {
                ExpectationId = evnt.ExpectationId,
                Iban = evnt.Iban,
                Amount = evnt.Amount,
                Reference = evnt.Reference,
                Status = "Created",
                Version = metadata.StreamPosition
            }
        );
    }

    private async Task Handle(ExpectationMatching evnt, EventMetadata metadata)
    {
        var expectation = await _repository.GetExpectation(evnt.ExpectationId);
        if (
            expectation is null
            || !TryUpdateVersion(expectation, metadata.StreamPosition, out expectation)
        )
        {
            return;
        }

        expectation = expectation with { Status = "Reserved" };
        await _repository.UpdateExpectation(expectation);
    }

    private async Task Handle(ExpectationMatched evnt, EventMetadata metadata)
    {
        var expectation = await _repository.GetExpectation(evnt.ExpectationId);
        if (
            expectation is null
            || !TryUpdateVersion(expectation, metadata.StreamPosition, out expectation)
        )
        {
            return;
        }

        expectation = expectation with { Status = "Matched" };
        await _repository.UpdateExpectation(expectation);
    }

    private async Task Handle(ExpectationMatchRejected evnt, EventMetadata metadata)
    {
        var expectation = await _repository.GetExpectation(evnt.ExpectationId);
        if (
            expectation is null
            || !TryUpdateVersion(expectation, metadata.StreamPosition, out expectation)
        )
        {
            return;
        }

        await _repository.UpdateExpectation(expectation);
    }

    private bool TryUpdateVersion(
        ExpectationRow expectation,
        ulong newVersion,
        out ExpectationRow newExpectation
    )
    {
        var expectedVersion = newVersion - 1;
        var actualVersion = expectation.Version;
        if (actualVersion >= newVersion)
        {
            newExpectation = expectation;
            return false;
        }
        if (actualVersion != expectedVersion)
        {
            throw new InvalidOperationException(
                $"Version mismatch, expected {expectedVersion} actual {actualVersion}"
            );
        }
        newExpectation = expectation with { Version = newVersion };
        return true;
    }
}
