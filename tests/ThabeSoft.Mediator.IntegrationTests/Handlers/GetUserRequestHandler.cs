using ThabeSoft.Mediator.Tests.Messages;

namespace ThabeSoft.Mediator.IntegrationTests.Handlers;

public class GetUserRequestHandler : IRequestHandler<GetUserRequest, GetUserResponse>
{
    public ValueTask<GetUserResponse> HandleAsync(GetUserRequest request, CancellationToken ct)
    {
        return ValueTask.FromResult(new GetUserResponse(request.Id, $"User{request.Id}"));
    }
}