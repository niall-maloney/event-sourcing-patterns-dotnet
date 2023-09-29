using MediatR;
using NiallMaloney.TwoPhaseCommit.Cassandra;

namespace NiallMaloney.TwoPhaseCommit.Service.Matching.Queries;

public class MatchingManagersQueryHandlers : IRequestHandler<GetManager, MatchingManagerRow?>,
    IRequestHandler<SearchManagers, IEnumerable<MatchingManagerRow>>
{
    private readonly IMatchingManagerRepository _repository;

    public MatchingManagersQueryHandlers(IMatchingManagerRepository repository)
    {
        _repository = repository;
    }

    public Task<MatchingManagerRow?> Handle(GetManager request, CancellationToken cancellationToken) =>
        _repository.GetManager(request.MatchingId);

    public Task<IEnumerable<MatchingManagerRow>> Handle(
        SearchManagers request,
        CancellationToken cancellationToken
    ) =>
        _repository.SearchManagers(
            request.MatchingId,
            request.PaymentId,
            request.ExpectationId,
            request.Status
        );
}
