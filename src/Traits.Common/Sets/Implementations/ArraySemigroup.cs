namespace Traits.Common.Sets.Implementations;

public sealed class ArraySemigroup : ISemigroup<object[]>
{
    public object[] Dot(object[] x, object[] y) =>
        x.Concat(y).ToArray();
}