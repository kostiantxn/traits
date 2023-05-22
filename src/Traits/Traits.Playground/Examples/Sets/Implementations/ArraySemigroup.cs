namespace Traits.Playground.Examples.Sets.Implementations;

internal sealed class ArraySemigroup : ISemigroup<object[]>
{
    public object[] Dot(object[] x, object[] y) =>
        x.Concat(y).ToArray();
}