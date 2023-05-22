namespace Traits.Playground.Examples.Sets.Implementations;

internal sealed class IntSemigroup : ISemigroup<int>
{
    public int Dot(int x, int y) =>
        x + y;
}