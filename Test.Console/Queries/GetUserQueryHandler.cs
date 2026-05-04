using ThabeSoft.Mediator;

namespace Test.Console.Queries;

public class GetUserQueryHandler : IQueryHandler<GetUserQuery, UserDto>
{
    public Task<UserDto> HandleAsync(GetUserQuery query, CancellationToken ct)
    {
        return Task.FromResult(new UserDto(query.Id, $"User{query.Id}"));
    }
}
