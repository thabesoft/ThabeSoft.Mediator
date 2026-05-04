namespace ThabeSoft.Mediator.SourceGenerator;

/// <summary>
/// 处理器信息
/// </summary>
internal readonly record struct HandlerInfo
{
    public static readonly HandlerInfo Empty = default;


    /// <summary>
    /// 类型
    /// </summary>
    public readonly HandlerKind Kind;
    /// <summary>
    /// 处理器类型名称
    /// </summary>
    public readonly string HandlerTypeFullName;
    /// <summary>
    /// 消息类型名称
    /// </summary>
    public readonly string MessageTypeFullName;
    /// <summary>
    /// 消息返回值类型名称
    /// </summary>
    public readonly string ReturnTypeFullName;


    private HandlerInfo(HandlerKind kind, string handlerTypeFullName, string messageTypeFullName, string returnType = null)
    {
        Kind = kind;
        HandlerTypeFullName = handlerTypeFullName;
        MessageTypeFullName = messageTypeFullName;
        ReturnTypeFullName = returnType;
    }

    public static bool TryCreateEvent(string handlerTypeFullName, string messageTypeFullName, out HandlerInfo handlerInfo)
    {
        if (!AllNotNullOrWhiteSpace(handlerTypeFullName, messageTypeFullName))
        {
            handlerInfo = Empty;
            return false;
        }

        handlerInfo = new HandlerInfo(HandlerKind.Event, handlerTypeFullName, messageTypeFullName);
        return true;
    }
    public static bool TryCreateCommand(string handlerTypeFullName, string messageTypeFullName, out HandlerInfo handlerInfo)
    {
        if (!AllNotNullOrWhiteSpace(handlerTypeFullName, messageTypeFullName))
        {
            handlerInfo = Empty;
            return false;
        }

        handlerInfo = new HandlerInfo(HandlerKind.Command, handlerTypeFullName, messageTypeFullName);
        return true;
    }
    public static bool TryCreateCommand(string handlerTypeFullName, string messageTypeFullName, string result, out HandlerInfo handlerInfo)
    {
        if (!AllNotNullOrWhiteSpace(handlerTypeFullName, messageTypeFullName, result))
        {
            handlerInfo = Empty;
            return false;
        }

        handlerInfo = new HandlerInfo(HandlerKind.Command, handlerTypeFullName, messageTypeFullName, result);
        return true;
    }
    public static bool TryCreateQuery(string handlerTypeFullName, string messageTypeFullName, string result, out HandlerInfo handlerInfo)
    {
        if (!AllNotNullOrWhiteSpace(handlerTypeFullName, messageTypeFullName, result))
        {
            handlerInfo = Empty;
            return false;
        }

        handlerInfo = new HandlerInfo(HandlerKind.Query, handlerTypeFullName, messageTypeFullName, result);
        return true;
    }


    // 全部传入的字符串都不能为空或者空白字符
    private static bool AllNotNullOrWhiteSpace(params IEnumerable<string> items)
    {
        return items.All(x => !string.IsNullOrWhiteSpace(x));
    }
}