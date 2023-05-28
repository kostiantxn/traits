namespace Traits.Benchmarks.Subjects;

public sealed class ObjectHash : IHash<object>
{
    public int Of(object self) =>
        self.GetHashCode();
}