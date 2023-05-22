namespace Traits.Playground.Examples.Sets.Implementations;

internal sealed class ArrayMonoid : IMonoid<object[]>
{
    public object[] Zero() =>
        Array.Empty<object>();
}