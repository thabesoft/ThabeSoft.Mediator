using ThabeSoft.Mediator;

namespace Test.Commands;


public record PingCommand : ICommand<PongResponse>;

public record PongResponse(string Message);
