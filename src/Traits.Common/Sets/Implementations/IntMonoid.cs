namespace Traits.Common.Sets.Implementations;

public sealed class IntMonoid : IMonoid<int>
{
    public int Zero() =>
        0;
}