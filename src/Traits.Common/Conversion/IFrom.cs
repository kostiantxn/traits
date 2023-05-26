namespace Traits.Common.Conversion;

/// <summary>
///     Defines a conversion between two types.
/// </summary>
/// <typeparam name="S">The type to convert into.</typeparam>
/// <typeparam name="T">The type to convert from.</typeparam>
/// <remarks>
///     The reciprocal of <see cref="IInto{S, T}"/>.
/// </remarks>
[Trait]
public interface IFrom<S, T>
{
    /// <summary>
    ///     Converts the value into <typeparamref name="S"/>.
    /// </summary>
    S Into(T value);
}