using System.Threading;
using System.Threading.Tasks;


namespace ThabeSoft.Mediator
{
    /// <summary>
    /// 命令处理器
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public interface ICommandHandler<TCommand, TResult>
        where TCommand : ICommand<TResult>
    {
        ValueTask<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// 命令处理器
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    public interface ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        ValueTask HandleAsync(TCommand command, CancellationToken cancellationToken = default);
    }
}