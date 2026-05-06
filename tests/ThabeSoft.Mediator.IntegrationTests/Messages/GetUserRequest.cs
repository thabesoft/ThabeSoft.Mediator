namespace ThabeSoft.Mediator.Tests.Messages;


public record GetUserRequest(int Id) : IRequest<GetUserResponse>;
public record GetUserResponse(int Id, string Name);
