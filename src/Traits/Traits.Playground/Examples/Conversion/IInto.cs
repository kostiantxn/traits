namespace Traits.Playground.Examples.Conversion;

[Trait]
internal interface IInto<S, T>
{
    /// <summary>
    ///     Converts the value into <typeparamref name="T"/>.
    /// </summary>
    T From(S self);
}