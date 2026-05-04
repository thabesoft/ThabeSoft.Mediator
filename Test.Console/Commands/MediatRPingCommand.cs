using MediatR;

namespace Test.Console.Commands;

public readonly record struct MediatRPingCommand : IRequest<MediatRPongResponse> { }

public readonly record struct MediatRPongResponse(string Message);
