namespace ThabeSoft.Mediator.SourceGenerators;

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
    public readonly string HandlerTypeName; 
    /// <summary>
    /// 消息类型名称
    /// </summary>
    public readonly string MessageTypeName;
    /// <summary>
    /// 消息返回值类型名称
    /// </summary>
    public readonly string ReturnTypeName;


    private HandlerInfo(HandlerKind kind, string handlerTypeName, string messageTypeName, string returnType = null)
    {
        Kind = kind;
        HandlerTypeName = handlerTypeName;
        MessageTypeName = messageTypeName;
        ReturnTypeName = returnType;
    }
    
    public static bool TryCreateEvent(string handlerTypeName, string messageTypeName, out HandlerInfo handlerInfo)
    {
        if (!AllNotNullOrWhiteSpace(handlerTypeName, messageTypeName))
        {
            handlerInfo = Empty;
            return false;
        }

        handlerInfo = new HandlerInfo(HandlerKind.Event, handlerTypeName, messageTypeName);
        return true;
    }
    public static bool TryCreateCommand(string handlerTypeName, string messageTypeName, out HandlerInfo handlerInfo)
    {
        if (!AllNotNullOrWhiteSpace(handlerTypeName, messageTypeName))
        {
            handlerInfo = Empty;
            return false;
        }

        handlerInfo = new HandlerInfo(HandlerKind.Command, handlerTypeName, messageTypeName);
        return true;
    }
    public static bool TryCreateCommand(string handlerTypeName, string messageTypeName, string result, out HandlerInfo handlerInfo)
    {
        if (!AllNotNullOrWhiteSpace(handlerTypeName, messageTypeName, result))
        {
            handlerInfo = Empty;
            return false;
        }

        handlerInfo = new HandlerInfo(HandlerKind.Command, handlerTypeName, messageTypeName, result);
        return true;
    }
    public static bool TryCreateQuery(string handlerTypeName, string messageTypeName, string result, out HandlerInfo handlerInfo)
    {
        if (!AllNotNullOrWhiteSpace(handlerTypeName, messageTypeName, result))
        {
            handlerInfo = Empty;
            return false;
        }

        handlerInfo = new HandlerInfo(HandlerKind.Query, handlerTypeName, messageTypeName, result);
        return true;
    }


    // 全部传入的字符串都不能为空或者空白字符
    private static bool AllNotNullOrWhiteSpace(params IEnumerable<string> items)
    {
        return items.All(x => !string.IsNullOrWhiteSpace(x));
    }
}
