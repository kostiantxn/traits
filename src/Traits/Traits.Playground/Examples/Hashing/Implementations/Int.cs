namespace Traits.Playground.Examples.Hashing.Implementations;

internal sealed class Int : IHash<int>
{
    public int Of(int self) =>
        self;
}