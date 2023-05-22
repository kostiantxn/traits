namespace Traits.Playground.Examples.Sets;

[Trait]
internal interface IMonoid<[Semigroup] S>
{
    S Zero();
}