namespace Traits.Playground.Examples.Conversion;

[Trait]
internal interface IFrom<S, T>
{
    /// <summary>
    ///     Converts the provided input into <typeparamref name="S"/>.
    /// </summary>
    S Into(T input);
}