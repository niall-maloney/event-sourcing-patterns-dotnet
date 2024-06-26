using MediatR;
using NiallMaloney.EventSourcing.Aggregates;
using NiallMaloney.FatEvents.Service.Users.Commands;

namespace NiallMaloney.FatEvents.Service.Users.Domain;

public class UserHandlers : IRequestHandler<AddUser>, IRequestHandler<AcceptUser>
{
    private readonly AggregateRepository _repository;

    public UserHandlers(AggregateRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(AcceptUser request, CancellationToken cancellationToken)
    {
        var user = await _repository.LoadAggregate<User>(request.UserId);
        user.Accept();
        await _repository.SaveAggregate(user);
    }

    public async Task Handle(AddUser request, CancellationToken cancellationToken)
    {
        var user = await _repository.LoadAggregate<User>(request.UserId);
        user.Request(request.EmailAddress, request.Forename, request.Surname);
        await _repository.SaveAggregate(user);
    }
}