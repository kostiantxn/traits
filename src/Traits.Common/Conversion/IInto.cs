namespace Traits.Common.Conversion;

/// <summary>
///     Defines a conversion between two types.
/// </summary>
/// <typeparam name="S">The type to convert from.</typeparam>
/// <typeparam name="T">The type to convert into.</typeparam>
/// <remarks>
///     The reciprocal of <see cref="IFrom{S, T}"/>.
/// </remarks>
[Trait]
public interface IInto<S, T>
{
    /// <summary>
    ///     Converts the value into <typeparamref name="T"/>.
    /// </summary>
    T From(S self);
}