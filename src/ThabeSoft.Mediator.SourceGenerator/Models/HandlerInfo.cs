namespace ThabeSoft.Mediator.SourceGenerator.Models;

/// <summary>
/// 处理器信息
/// </summary>
internal sealed record class HandlerInfo
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
    /// 输入类型名称
    /// </summary>
    public readonly string InputTypeFullName;
    /// <summary>
    /// 输出类型名称
    /// </summary>
    public readonly string OutputTypeFullName;


    private HandlerInfo(HandlerKind kind, string handlerTypeFullName, string messageTypeFullName, string returnType = null)
    {
        Kind = kind;
        HandlerTypeFullName = handlerTypeFullName;
        InputTypeFullName = messageTypeFullName;
        OutputTypeFullName = returnType;
    }

    
    public static bool TryCreateRequest(string handlerTypeFullName, string requestTypeFullName, out HandlerInfo handlerInfo)
    {
        if (!AllNotNullOrWhiteSpace(handlerTypeFullName, requestTypeFullName))
        {
            handlerInfo = Empty;
            return false;
        }

        handlerInfo = new HandlerInfo(HandlerKind.Request, handlerTypeFullName, requestTypeFullName);
        return true;
    }
    public static bool TryCreateRequestResponse(string handlerTypeFullName, string requestTypeFullName, string responseTypeFullName, out HandlerInfo handlerInfo)
    {
        if (!AllNotNullOrWhiteSpace(handlerTypeFullName, requestTypeFullName, responseTypeFullName))
        {
            handlerInfo = Empty;
            return false;
        }

        handlerInfo = new HandlerInfo(HandlerKind.RequestResponse, handlerTypeFullName, requestTypeFullName, responseTypeFullName);
        return true;
    }
    public static bool TryCreateNotification(string handlerTypeFullName, string messageTypeFullName, out HandlerInfo handlerInfo)
    {
        if (!AllNotNullOrWhiteSpace(handlerTypeFullName, messageTypeFullName))
        {
            handlerInfo = Empty;
            return false;
        }

        handlerInfo = new HandlerInfo(HandlerKind.Notification, handlerTypeFullName, messageTypeFullName);
        return true;
    }


    // 全部传入的字符串都不能为空或者空白字符
    private static bool AllNotNullOrWhiteSpace(params IEnumerable<string> items)
    {
        return items.All(x => !string.IsNullOrWhiteSpace(x));
    }


    public override string ToString()
    {
        if(OutputTypeFullName is null)
        {
            return $"{HandlerTypeFullName} ({Kind})<{InputTypeFullName}>";
        }

        return $"{HandlerTypeFullName} ({Kind})<{InputTypeFullName}, {OutputTypeFullName}>";
    }
}