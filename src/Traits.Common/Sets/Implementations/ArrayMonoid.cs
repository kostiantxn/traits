namespace Traits.Common.Sets.Implementations;

public sealed class ArrayMonoid : IMonoid<object[]>
{
    public object[] Zero() =>
        Array.Empty<object>();
}