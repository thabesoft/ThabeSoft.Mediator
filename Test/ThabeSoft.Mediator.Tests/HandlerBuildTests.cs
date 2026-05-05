using Microsoft.Extensions.DependencyInjection;
using Test.Models.Commands;
using Test.Models.Events;
using Test.Models.Queries;
using ThabeSoft.Mediator.DependencyInjection;

namespace ThabeSoft.Mediator.Tests;


/// <summary>
/// 处理器构建测试
/// </summary>
[TestClass]
public class HandlerBuildTests
{
    private HandlerDescriptorCollection _collection = default!;

    [TestInitialize]
    public void Setup()
    {
        _collection = new HandlerDescriptorCollection();
    }

    [TestMethod(DisplayName = "是否成功添加命令处理器")]
    public void AddCommand_ShouldAddCommandHandler()
    {
        // Act
        _collection.AddCommand<PingCommandHandler, PingCommand, PongResponse>()
            .Singleton();

        // Assert
        var descriptors = _collection.BuildToServiceDescriptors().ToList();
        Assert.HasCount(1, descriptors);

        var descriptor = descriptors[0];
        Assert.AreEqual(typeof(ICommandHandler<PingCommand, PongResponse>), descriptor.ServiceType);
        Assert.AreEqual(typeof(PingCommandHandler), descriptor.ImplementationType);
        Assert.AreEqual(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [TestMethod(DisplayName = "默认生命周期是否为作用域")]
    public void AddCommand_WithoutLifetime_ShouldUseDefaultScoped()
    {
        // Act
        _collection.AddCommand<PingCommandHandler, PingCommand, PongResponse>();

        // Assert
        var descriptors = _collection.BuildToServiceDescriptors().ToList();
        var descriptor = descriptors[0];
        Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [TestMethod(DisplayName = "是否成功添加查询处理器")]
    public void AddQuery_ShouldAddQueryHandler()
    {
        // Act
        _collection.AddQuery<GetUserQueryHandler, GetUserQuery, UserDto>()
            .Singleton();

        // Assert
        var descriptors = _collection.BuildToServiceDescriptors().ToList();
        Assert.HasCount(1, descriptors);

        var descriptor = descriptors[0];
        Assert.AreEqual(typeof(IQueryHandler<GetUserQuery, UserDto>), descriptor.ServiceType);
        Assert.AreEqual(typeof(GetUserQueryHandler), descriptor.ImplementationType);
    }

    [TestMethod(DisplayName = "是否成功添加事件处理器")]
    public void AddEvent_ShouldAddEventHandler()
    {
        // Act
        _collection.AddEvent<UserCreatedEventHandler, UserCreatedEvent>()
            .Scoped();

        // Assert
        var descriptors = _collection.BuildToServiceDescriptors().ToList();
        Assert.HasCount(1, descriptors);

        var descriptor = descriptors[0];
        Assert.AreEqual(typeof(IEventHandler<UserCreatedEvent>), descriptor.ServiceType);
        Assert.AreEqual(typeof(UserCreatedEventHandler), descriptor.ImplementationType);
    }

    [TestMethod(DisplayName = "排除处理器是否生效")]
    public void Except_ShouldExcludeHandler()
    {
        // Arrange
        _collection.AddCommand<PingCommandHandler, PingCommand, PongResponse>();
        _collection.AddQuery<GetUserQueryHandler, GetUserQuery, UserDto>();

        // Act
        _collection.Except(x => x.ImplementationType == typeof(PingCommandHandler));

        // Assert
        var descriptors = _collection.BuildToServiceDescriptors().ToList();
        Assert.HasCount(1, descriptors);
        Assert.AreEqual(typeof(GetUserQueryHandler), descriptors[0].ImplementationType);
    }

    [TestMethod(DisplayName = "更新所有带结果命令的生命周期为单例")]
    public void ConfigureWhere_ShouldModifyMatchedHandlers()
    {
        // Arrange
        _collection.AddCommand<PingCommandHandler, PingCommand, PongResponse>();

        // Act
        _collection.UpdateAll(
            x => x.Kind == HandlerKind.CommandWithResult,
            x => x.Singleton()
        );

        // Assert
        var descriptors = _collection.BuildToServiceDescriptors().ToList();
        var commandDescriptor = descriptors.First(d => d.ServiceType == typeof(ICommandHandler<PingCommand, PongResponse>));
        Assert.AreEqual(ServiceLifetime.Singleton, commandDescriptor.Lifetime);
    }

    [TestMethod(DisplayName = "查询所有命令改为单例")]
    public void Batch_ShouldApplyOperationsToAllMatchedHandlers()
    {
        // Arrange
        _collection.AddCommand<PingCommandHandler, PingCommand, PongResponse>();
        _collection.AddCommand<FuckCommandHandler, FuckCommand>();

        // Act
        _collection.FindAllByCommand()
            .Singleton();

        // Assert
        var descriptors = _collection.BuildToServiceDescriptors().ToList();
        foreach (var descriptor in descriptors)
        {
            Assert.AreEqual(ServiceLifetime.Singleton, descriptor.Lifetime);
        }
    }

    [TestMethod(DisplayName = "根据消息类型批量更改生命周期")]
    public void ChainOperations_ShouldWorkInSequence()
    {
        // Arrange
        _collection.AddCommand<PingCommandHandler, PingCommand, PongResponse>();
        _collection.AddQuery<GetUserQueryHandler, GetUserQuery, UserDto>();

        // Act
        _collection
            .FindAllByCommand()
                .Singleton()
                .Back()
            .FindAllByQuery()
                .Scoped()
                .Back();

        // Assert
        var descriptors = _collection.BuildToServiceDescriptors().ToList();
        var commandDesc = descriptors.First(d => d.ServiceType == typeof(ICommandHandler<PingCommand, PongResponse>));
        var queryDesc = descriptors.First(d => d.ServiceType == typeof(IQueryHandler<GetUserQuery, UserDto>));

        Assert.AreEqual(ServiceLifetime.Singleton, commandDesc.Lifetime);
        Assert.AreEqual(ServiceLifetime.Scoped, queryDesc.Lifetime);
    }

    [TestMethod(DisplayName = "根据命令查询处理器")]
    public void FindAllByCommand_ShouldFindSpecificCommand()
    {
        // Arrange
        _collection.AddCommand<PingCommandHandler, PingCommand, PongResponse>();
        _collection.AddQuery<GetUserQueryHandler, GetUserQuery, UserDto>();

        // Act
        var batch = _collection.FindAllByCommand<PingCommand, PongResponse>();

        // 验证 batch 中的描述符
        var descriptors = _collection.BuildToServiceDescriptors().ToList();
        Assert.IsTrue(descriptors.Any(d => d.ServiceType == typeof(ICommandHandler<PingCommand, PongResponse>)));
    }

    [TestMethod(DisplayName = "多次更改生命周期以最后为准")]
    public void MultipleLifetimeSettings_LastOneWins()
    {
        // Act
        _collection.AddCommand<PingCommandHandler, PingCommand, PongResponse>()
            .Singleton()
            .Scoped()
            .Transient();

        // Assert
        var descriptors = _collection.BuildToServiceDescriptors().ToList();
        Assert.AreEqual(ServiceLifetime.Transient, descriptors[0].Lifetime);
    }

    [TestMethod(DisplayName = "添加后排除数量比对")]
    public void Except_AfterAdd_ShouldRemoveHandler()
    {
        // Act
        _collection.AddCommand<PingCommandHandler, PingCommand, PongResponse>()
            .Except();

        // Assert
        var descriptors = _collection.BuildToServiceDescriptors().ToList();
        Assert.HasCount(0, descriptors);
    }

    [TestMethod(DisplayName = "添加数量比对")]
    public void BuildToServiceDescriptors_ShouldReturnReadOnlyCollection()
    {
        // Arrange
        _collection.AddCommand<PingCommandHandler, PingCommand, PongResponse>();

        // Act
        var result = _collection.BuildToServiceDescriptors();

        // Assert
        Assert.IsInstanceOfType<IReadOnlyCollection<ServiceDescriptor>>(result);
        Assert.HasCount(1, result);
    }
}