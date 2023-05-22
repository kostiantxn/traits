namespace Traits.Playground.Examples.Sets;

[Trait]
internal interface ISemigroup<S>
{
    S Dot(S x, S y);
}