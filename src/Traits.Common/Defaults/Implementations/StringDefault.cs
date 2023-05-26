namespace Traits.Common.Defaults.Implementations;

public sealed class StringDefault : IDefault<string>
{
    public string Of() => string.Empty;
}