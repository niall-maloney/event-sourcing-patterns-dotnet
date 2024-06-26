using MediatR;
using NiallMaloney.PendingCreation.Cassandra.Users;

namespace NiallMaloney.PendingCreation.Service.Users.Queries;

public class UserQueryHandlers : IRequestHandler<GetUser, UserRow?>, IRequestHandler<SearchUsers, IEnumerable<UserRow>>
{
    private readonly IUsersRepository _repository;

    public UserQueryHandlers(IUsersRepository repository)
    {
        _repository = repository;
    }

    public Task<UserRow?> Handle(GetUser query, CancellationToken cancellationToken)
    {
        return _repository.GetUser(query.UserId);
    }

    public Task<IEnumerable<UserRow>> Handle(SearchUsers query, CancellationToken cancellationToken)
    {
        return _repository.SearchUsers(query.EmailAddress, query.Status);
    }
}