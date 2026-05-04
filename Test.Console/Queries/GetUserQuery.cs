using ThabeSoft.Mediator;

namespace Test.Console.Queries;


public record GetUserQuery(int Id) : IQuery<UserDto>;

public record UserDto(int Id, string Name);