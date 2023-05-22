namespace Traits.Playground.Examples.Sets.Implementations;

internal sealed class IntMonoid : IMonoid<int>
{
    public int Zero() =>
        0;
}