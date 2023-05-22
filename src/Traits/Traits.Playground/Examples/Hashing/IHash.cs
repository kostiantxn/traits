namespace Traits.Playground.Examples.Hashing;

[Trait]
internal interface IHash<S>
{
    int Of(S self);
}