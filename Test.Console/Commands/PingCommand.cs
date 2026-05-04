using ThabeSoft.Mediator;

namespace Test.Console.Commands;


public readonly record struct PingCommand : ICommand<PongResponse>;

public readonly record struct PongResponse(string Message);
