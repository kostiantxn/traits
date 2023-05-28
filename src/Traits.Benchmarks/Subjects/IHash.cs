namespace Traits.Benchmarks.Subjects;

[Trait]
public interface IHash<S>
{
    int Of(S self);
}