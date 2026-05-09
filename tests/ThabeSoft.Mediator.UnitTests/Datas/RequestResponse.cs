namespace ThabeSoft.Mediator.UnitTests.Datas;


public record RequestResponse(int PingId) : IRequest<Response>;
public record Response(int PingId);