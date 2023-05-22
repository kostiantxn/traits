namespace Traits.Playground.Examples.Sets.Implementations;

internal sealed class StringMonoid : IMonoid<string>
{
    public string Zero() =>
        string.Empty;
}