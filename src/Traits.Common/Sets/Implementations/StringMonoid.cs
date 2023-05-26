namespace Traits.Common.Sets.Implementations;

public sealed class StringMonoid : IMonoid<string>
{
    public string Zero() =>
        string.Empty;
}