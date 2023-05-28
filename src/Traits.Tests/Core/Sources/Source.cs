namespace Traits.Tests.Core.Sources;

internal record Source(string Text, string? Path = null)
{
    public static implicit operator Source(string text) =>
        new(text);
}
