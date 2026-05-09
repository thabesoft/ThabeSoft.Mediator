using Microsoft.Extensions.DependencyInjection;
using ThabeSoft.Mediator.DependencyInjection;
using ThabeSoft.Mediator.UnitTests.Datas;

namespace ThabeSoft.Mediator.UnitTests;


/// <summary>
/// 处理器构建测试
/// </summary>
[TestClass]
public class HandlerBuildTests
{
    public static Type RequestHandlerServiceType { get; } = typeof(IRequestHandler<Request>);
    public static Type ResultRequestHandlerServiceType { get; } = typeof(IRequestHandler<RequestResponse, Response>);
    public static Type NotificationHandlerServiceType { get; } = typeof(INotificationHandler<Notification>);


    public static (HandlerKind Kind, Type HandlerServiceType)[] HandlerServiceType { get; } =
    [
        (HandlerKind.Request, RequestHandlerServiceType),
        (HandlerKind.RequestResponse, ResultRequestHandlerServiceType),
        (HandlerKind.Notification, NotificationHandlerServiceType)
    ];

    #region -- 添加处理器 --


    [TestMethod(DisplayName = "添加请求处理器")]
    public void AddRequestHandler_TRequest()
    {
        var collection = new DescriptorCollection();
        collection.AddRequestHandler<RequestHandler, Request>();

        var descriptors = collection.BuildToServiceDescriptors();
        Assert.HasCount(1, descriptors);

        var descriptor = descriptors.First();
        Assert.AreEqual(typeof(IRequestHandler<Request>), descriptor.ServiceType);
        Assert.AreEqual(typeof(RequestHandler), descriptor.ImplementationType);
        Assert.AreEqual(collection.DefaultLifetime, descriptor.Lifetime);
    }

    [TestMethod(DisplayName = "添加请求-响应处理器")]
    public void AddRequestHandler_TRequest_TResponse()
    {
        var collection = new DescriptorCollection();
        collection.AddRequestHandler<RequestResponseHandler, RequestResponse, Response>();

        var descriptors = collection.BuildToServiceDescriptors();
        Assert.HasCount(1, descriptors);

        var descriptor = descriptors.First();
        Assert.AreEqual(typeof(IRequestHandler<RequestResponse, Response>), descriptor.ServiceType);
        Assert.AreEqual(typeof(RequestResponseHandler), descriptor.ImplementationType);
        Assert.AreEqual(collection.DefaultLifetime, descriptor.Lifetime);
    }

    [TestMethod(DisplayName = "添加通知处理器")]
    public void AddNotificationHandler_TNotification()
    {
        var collection = new DescriptorCollection();
        collection.AddNotificationHandler<NotificationHandler, Notification>();

        var descriptors = collection.BuildToServiceDescriptors();
        Assert.HasCount(1, descriptors);

        var descriptor = descriptors.First();
        Assert.AreEqual(typeof(INotificationHandler<Notification>), descriptor.ServiceType);
        Assert.AreEqual(typeof(NotificationHandler), descriptor.ImplementationType);
        Assert.AreEqual(collection.DefaultLifetime, descriptor.Lifetime);
    }

    #endregion

    #region -- 添加管道行为 --


    [TestMethod(DisplayName = "添加请求管道行为")]
    public void AddRequestBehavior_TRequest()
    {
        var collection = new DescriptorCollection();
        collection.AddRequestBehavior<RequestPipelineBehavior, Request>();

        var descriptors = collection.BuildToServiceDescriptors();
        Assert.HasCount(1, descriptors);

        var descriptor = descriptors.First();
        Assert.AreEqual(typeof(IRequestPipelineBehavior<Request>), descriptor.ServiceType);
        Assert.AreEqual(typeof(RequestPipelineBehavior), descriptor.ImplementationType);
        Assert.AreEqual(collection.DefaultLifetime, descriptor.Lifetime);
    }

    [TestMethod(DisplayName = "添加请求-响应管道行为")]
    public void AddRequestBehavior_TRequest_TResponse()
    {
        var collection = new DescriptorCollection();
        collection.AddRequestBehavior<RequestResponsePipelineBehavior, RequestResponse, Response>();

        var descriptors = collection.BuildToServiceDescriptors();
        Assert.HasCount(1, descriptors);

        var descriptor = descriptors.First();
        Assert.AreEqual(typeof(IRequestPipelineBehavior<RequestResponse, Response>), descriptor.ServiceType);
        Assert.AreEqual(typeof(RequestResponsePipelineBehavior), descriptor.ImplementationType);
        Assert.AreEqual(collection.DefaultLifetime, descriptor.Lifetime);
    }

    

    [TestMethod(DisplayName = "添加通知管道行为")]
    public void AddRequestBehavior_TNotification()
    {
        var collection = new DescriptorCollection();
        collection.AddNotificationBehavior<NotificationPipelineBehavior, Notification>();

        var descriptors = collection.BuildToServiceDescriptors();
        Assert.HasCount(1, descriptors);

        var descriptor = descriptors.First();
        Assert.AreEqual(typeof(INotificationPipelineBehavior<Notification>), descriptor.ServiceType);
        Assert.AreEqual(typeof(NotificationPipelineBehavior), descriptor.ImplementationType);
        Assert.AreEqual(collection.DefaultLifetime, descriptor.Lifetime);
    }

    #endregion


    #region -- 基础设置 --

    [DataRow(ServiceLifetime.Scoped, DisplayName = "作用域")]
    [DataRow(ServiceLifetime.Singleton, DisplayName = "单例")]
    [DataRow(ServiceLifetime.Transient, DisplayName = "瞬态")]
    [TestMethod(DisplayName = "更改默认生命周期")]
    public void Default(ServiceLifetime serviceLifetime)
    {
        var collection = new DescriptorCollection();
        collection.Default(serviceLifetime);

        Assert.AreEqual(serviceLifetime, collection.DefaultLifetime);
    }

    [DataRow(HandlerKind.Request, ServiceLifetime.Scoped, DisplayName = "作用域请求")]
    [DataRow(HandlerKind.Request, ServiceLifetime.Singleton, DisplayName = "单例请求")]
    [DataRow(HandlerKind.Request, ServiceLifetime.Transient, DisplayName = "瞬态请求")]
    [DataRow(HandlerKind.RequestResponse, ServiceLifetime.Scoped, DisplayName = "作用域请求-响应")]
    [DataRow(HandlerKind.RequestResponse, ServiceLifetime.Singleton, DisplayName = "单例请求-响应")]
    [DataRow(HandlerKind.RequestResponse, ServiceLifetime.Transient, DisplayName = "瞬态请求-响应")]
    [DataRow(HandlerKind.Notification, ServiceLifetime.Scoped, DisplayName = "作用域通知")]
    [DataRow(HandlerKind.Notification, ServiceLifetime.Singleton, DisplayName = "单例通知")]
    [DataRow(HandlerKind.Notification, ServiceLifetime.Transient, DisplayName = "瞬态通知")]
    [TestMethod(DisplayName = "根据条件更新")]
    public void UpdateAll(HandlerKind kind, ServiceLifetime serviceLifetime)
    {
        var collection = new DescriptorCollection();
        collection.AddRequestHandler<RequestHandler, Request>();
        collection.AddRequestHandler<RequestResponseHandler, RequestResponse, Response>();
        collection.AddNotificationHandler<NotificationHandler, Notification>();


        collection.UpdateAll(x => x.HandlerKind == kind, x => x.SetLifetime(serviceLifetime));

        // Assert
        var descriptors = collection.BuildToServiceDescriptors().ToList();

        foreach(var i in HandlerServiceType.Where(x => x.Kind == kind))
        {
            var service_descriptor = descriptors.FirstOrDefault(x => x.ServiceType == i.HandlerServiceType);
            Assert.IsNotNull(service_descriptor);
            Assert.AreEqual(serviceLifetime, service_descriptor.Lifetime);
        }
    }



    [DataRow(HandlerKind.Request, DisplayName = "请求")]
    [DataRow(HandlerKind.RequestResponse, DisplayName = "请求-响应")]
    [DataRow(HandlerKind.Notification, DisplayName = "通知")]
    [TestMethod(DisplayName = "根据条件排除")]
    public void ExceptAll(HandlerKind kind)
    {
        var collection = new DescriptorCollection();
        collection.AddRequestHandler<RequestHandler, Request>();
        collection.AddRequestHandler<RequestResponseHandler, RequestResponse, Response>();
        collection.AddNotificationHandler<NotificationHandler, Notification>();


        collection.ExceptAll(x => x.HandlerKind == kind);

        // Assert
        var descriptors = collection.BuildToServiceDescriptors().ToList();
        Assert.HasCount(2, descriptors);

        var except_service_types = HandlerServiceType.Where(x => x.Kind != kind).Select(x => x.HandlerServiceType);
        Assert.IsTrue(descriptors.All(d => except_service_types.Contains(d.ServiceType)));
    }

    [DataRow(HandlerKind.Request, ServiceLifetime.Scoped, DisplayName = "作用域请求")]
    [DataRow(HandlerKind.Request, ServiceLifetime.Singleton, DisplayName = "单例请求")]
    [DataRow(HandlerKind.Request, ServiceLifetime.Transient, DisplayName = "瞬态请求")]
    [DataRow(HandlerKind.RequestResponse, ServiceLifetime.Scoped, DisplayName = "作用域请求-响应")]
    [DataRow(HandlerKind.RequestResponse, ServiceLifetime.Singleton, DisplayName = "单例请求-响应")]
    [DataRow(HandlerKind.RequestResponse, ServiceLifetime.Transient, DisplayName = "瞬态请求-响应")]
    [DataRow(HandlerKind.Notification, ServiceLifetime.Scoped, DisplayName = "作用域通知")]
    [DataRow(HandlerKind.Notification, ServiceLifetime.Singleton, DisplayName = "单例通知")]
    [DataRow(HandlerKind.Notification, ServiceLifetime.Transient, DisplayName ="瞬态通知")]
    [TestMethod(DisplayName = "根据条件查询")]
    public void FindAll(HandlerKind kind, ServiceLifetime serviceLifetime)
    {
        var collection = new DescriptorCollection();
        collection.AddRequestHandler<RequestHandler, Request>();
        collection.AddRequestHandler<RequestResponseHandler, RequestResponse, Response>();
        collection.AddNotificationHandler<NotificationHandler, Notification>();


        collection.Batch(x => x.HandlerKind == kind)
            .SetLifetime(serviceLifetime);

        // Assert
        var descriptors = collection.BuildToServiceDescriptors();
        foreach (var i in HandlerServiceType.Where(x => x.Kind == kind))
        {
            var service_descriptor = descriptors.FirstOrDefault(x => x.ServiceType == i.HandlerServiceType);
            Assert.IsNotNull(service_descriptor);
            Assert.AreEqual(serviceLifetime, service_descriptor.Lifetime);
        }
    }

    #endregion
}