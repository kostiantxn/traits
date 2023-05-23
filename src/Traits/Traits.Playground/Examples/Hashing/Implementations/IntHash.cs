namespace Traits.Playground.Examples.Hashing.Implementations;

internal sealed class IntHash : IHash<int>
{
    public int Of(int self) =>
        self;
}