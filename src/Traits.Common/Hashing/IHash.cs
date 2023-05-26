namespace Traits.Common.Hashing;

/// <summary>
///     Defines a hashable type.
/// </summary>
[Trait]
public interface IHash<S>
{
    /// <summary>
    ///     Computes the hash.
    /// </summary>
    /// <param name="self">The value to compute the hash of.</param>
    /// <returns>The computed hash.</returns>
    int Of(S self);
}