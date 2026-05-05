namespace ThabeSoft.Mediator.Benchmark.Models.ThabeSoft;


public record GetUserQuery(int Id) : IQuery<UserDto>;

public record UserDto(int Id, string Name);


public class GetUserQueryHandler : IQueryHandler<GetUserQuery, UserDto>
{
    public ValueTask<UserDto> HandleAsync(GetUserQuery query, CancellationToken ct)
    {
        return ValueTask.FromResult(new UserDto(query.Id, $"User{query.Id}"));
    }
}
