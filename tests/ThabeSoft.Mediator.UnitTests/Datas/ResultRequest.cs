namespace ThabeSoft.Mediator.UnitTests.Requests;

public record ResultRequest(int PingId) : IRequest<Response>;
public record Response(int PingId);
