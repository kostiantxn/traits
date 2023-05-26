namespace Traits.Common.Sets.Implementations;

public sealed class IntSemigroup : ISemigroup<int>
{
    public int Dot(int x, int y) =>
        x + y;
}