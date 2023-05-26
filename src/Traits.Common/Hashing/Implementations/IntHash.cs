namespace Traits.Common.Hashing.Implementations;

public sealed class IntHash : IHash<int>
{
    public int Of(int self) =>
        self;
}