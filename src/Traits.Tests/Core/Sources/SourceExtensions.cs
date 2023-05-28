namespace Traits.Tests.Core.Sources;

internal static class SourceExtensions
{
    public static Source Path(this string source, string? path) =>
        new(source, path);
}
