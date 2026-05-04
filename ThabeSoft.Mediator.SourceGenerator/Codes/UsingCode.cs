namespace ThabeSoft.Mediator.SourceGenerators.Codes;

internal static class UsingCode
{
    /// <summary>
    /// 从名字空间构建 Using 代码
    /// </summary>
    /// <param name="namespaces"></param>
    /// <returns></returns>
    public static string FromNamespaces(IEnumerable<string> namespaces)
    {
        return string.Join("\n", namespaces.Select(x => $"using {x};"));
    }
}
