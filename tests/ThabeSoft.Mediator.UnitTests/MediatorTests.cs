using Microsoft.Extensions.DependencyInjection;
using Moq;
using ThabeSoft.Mediator.DependencyInjection;
using ThabeSoft.Mediator.UnitTests.Datas;

namespace ThabeSoft.Mediator.UnitTests;


[TestClass]
public class MediatorTests
{
    #region -- SendAsync<TRequest, TResponse> --

    [TestMethod(DisplayName = "请求-响应")]
    public async Task SendAsync_Request_Response()
    {
        ServiceCollection services = new();
        services.AddMediator();
        services.ConfigureMediator(x =>
        {
            x.AddRequestBehavior<RequestPipelineBehavior, Request>();
            x.AddRequestBehavior<RequestResponsePipelineBehavior, RequestResponse, Response>();
            x.AddNotificationBehavior<NotificationPipelineBehavior, Notification>();

            x.AddRequestHandler<RequestHandler, Request>();
            x.AddRequestHandler<RequestResponseHandler, RequestResponse, Response>();
            x.AddNotificationHandler<NotificationHandler, Notification>();
        });
        services.ConfigureMediator(x => x.AddNotificationBehavior<NotificationPipelineBehavior, Notification>());
        services.ConfigureMediator(x => x.PipelineBehavior().Except());

        var mockHandler = new Mock<IRequestHandler<RequestResponse, Response>>();
        var expectedResponse = new Response(123);

        mockHandler
            .Setup(x => x.HandleAsync(It.IsAny<RequestResponse>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider
            .Setup(x => x.GetService(typeof(IRequestHandler<RequestResponse, Response>)))
            .Returns(mockHandler.Object);

        var mediator = new Mediator(serviceProvider.Object);
        var request = new RequestResponse(123);

        var response = await mediator.SendAsync<RequestResponse, Response>(request, TestContext.CancellationToken);

        Assert.AreEqual(expectedResponse.PingId, response.PingId);
        mockHandler.Verify(x => x.HandleAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod(DisplayName = "请求-响应-取消令牌")]
    public async Task SendAsync_Request_Response_WhenCancelled_ShouldThrow()
    {
        var mockHandler = new Mock<IRequestHandler<RequestResponse, Response>>();
        mockHandler
            .Setup(x => x.HandleAsync(It.IsAny<RequestResponse>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider
            .Setup(x => x.GetService(typeof(IRequestHandler<RequestResponse, Response>)))
            .Returns(mockHandler.Object);

        var mediator = new Mediator(serviceProvider.Object);
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            async () => await mediator.SendAsync<RequestResponse, Response>(new RequestResponse(0), cts.Token)
        );
    }

    [TestMethod(DisplayName = "请求-处理器不存在")]
    public async Task SendAsync_Request_Response_WhenHandlerNotFound_ShouldThrow()
    {
        var serviceProvider = new Mock<IServiceProvider>();
        var mediator = new Mediator(serviceProvider.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await mediator.SendAsync<RequestResponse, Response>(new RequestResponse(0), TestContext.CancellationToken)
        );
    }

    [TestMethod(DisplayName = "请求-响应-null参数")]
    public async Task SendAsync_Request_Response_WithNullRequest_ShouldThrow()
    {
        var mediator = new Mediator(Mock.Of<IServiceProvider>());

        await Assert.ThrowsAsync<ArgumentNullException>(
            async () => await mediator.SendAsync<RequestResponse, Response>(null!, TestContext.CancellationToken)
        );
    }

    [TestMethod(DisplayName = "请求-响应-null参数")]
    public async Task SendAsync_TRequest_TResponse_WithNullRequest_ShouldThrow()
    {
        var mediator = new Mediator(Mock.Of<IServiceProvider>());

        await Assert.ThrowsAsync<ArgumentNullException>(
            async () => await mediator.SendAsync<RequestResponse, Response>(null!, TestContext.CancellationToken)
        );
    }

    #endregion


    #region -- SendAsync<TRequest> --

    [TestMethod(DisplayName = "请求")]
    public async Task SendAsync_Request()
    {
        var mockHandler = new Mock<IRequestHandler<Request>>();

        mockHandler
            .Setup(x => x.HandleAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()));

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider
            .Setup(x => x.GetService(typeof(IRequestHandler<Request>)))
            .Returns(mockHandler.Object);

        var mediator = new Mediator(serviceProvider.Object);
        var request = new Request();

        await mediator.SendAsync(request, TestContext.CancellationToken);

        mockHandler.Verify(x => x.HandleAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod(DisplayName = "请求-null参数")]
    public async Task SendAsync_Request_WithNullRequest_ShouldThrow()
    {
        var mediator = new Mediator(Mock.Of<IServiceProvider>());

        await Assert.ThrowsAsync<ArgumentNullException>(
            async () => await mediator.SendAsync<Request>(null!, TestContext.CancellationToken)
        );
    }

    [TestMethod(DisplayName = "请求-处理器不存在")]
    public async Task SendAsync_Request_WhenHandlerNotFound_ShouldThrow()
    {
        var serviceProvider = new Mock<IServiceProvider>();
        var mediator = new Mediator(serviceProvider.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await mediator.SendAsync(new Request(), TestContext.CancellationToken)
        );
    }

    [TestMethod(DisplayName = "请求-取消令牌")]
    public async Task SendAsync_Request_WhenCancelled_ShouldThrow()
    {
        var mockHandler = new Mock<IRequestHandler<Request>>();
        mockHandler
            .Setup(x => x.HandleAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider
            .Setup(x => x.GetService(typeof(IRequestHandler<Request>)))
            .Returns(mockHandler.Object);

        var mediator = new Mediator(serviceProvider.Object);
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            async () => await mediator.SendAsync(new Request(), cts.Token)
        );
    }

    #endregion


    #region -- PublishAsync<TNotification> --

    [TestMethod(DisplayName = "通知")]
    public async Task PublishAsync_Notification()
    {
        var mockHandler1 = new Mock<INotificationHandler<Notification>>();
        mockHandler1
            .Setup(x => x.HandleAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()));

        var mockHandler2 = new Mock<INotificationHandler<Notification>>();
        mockHandler2
            .Setup(x => x.HandleAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()));


        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider
            .Setup(x => x.GetService(typeof(IEnumerable<INotificationHandler<Notification>>)))
            .Returns(new INotificationHandler<Notification>[] { mockHandler1.Object, mockHandler2.Object });

        
        var mediator = new Mediator(serviceProvider.Object);
        var notification = new Notification();

        
        await mediator.PublishAsync(notification, TestContext.CancellationToken);


        mockHandler1.Verify(x => x.HandleAsync(notification, It.IsAny<CancellationToken>()), Times.Once);
        mockHandler2.Verify(x => x.HandleAsync(notification, It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod(DisplayName = "通知-null参数")]
    public async Task PublishAsync_Notification_WithNullNotification_ShouldThrow()
    {
        DescriptorCollection handlerDescriptor = new();

        var mediator = new Mediator(Mock.Of<IServiceProvider>());

        await Assert.ThrowsAsync<ArgumentNullException>(
            async () => await mediator.PublishAsync<Notification>(null!, TestContext.CancellationToken)
        );
    }

    #endregion

    public TestContext TestContext { get; set; }
}