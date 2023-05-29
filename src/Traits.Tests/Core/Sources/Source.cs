namespace Traits.Tests.Core.Sources;

internal record Source(string Analyze, string Generate, string? Path = null)
{
    public static implicit operator Source(string text) =>
        new(Analyze: text, Generate: text);
}
