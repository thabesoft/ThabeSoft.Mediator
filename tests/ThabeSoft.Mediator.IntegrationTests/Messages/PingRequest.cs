namespace ThabeSoft.Mediator.Tests.Messages;


public record PingRequest(int PingId) : IRequest<PongResponse>;
public record PongResponse(int PingId, string Message);
