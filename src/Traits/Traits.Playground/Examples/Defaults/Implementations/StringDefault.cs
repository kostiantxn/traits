namespace Traits.Playground.Examples.Defaults.Implementations;

internal sealed class StringDefault : IDefault<string>
{
    public string Of() => string.Empty;
}