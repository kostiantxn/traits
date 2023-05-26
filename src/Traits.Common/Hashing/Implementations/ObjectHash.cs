namespace Traits.Common.Hashing.Implementations;

public sealed class ObjectHash : IHash<object>
{
    public int Of(object self) =>
        self.GetHashCode();
}