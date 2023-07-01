namespace Traits.Common.Cloning;

/// <summary>
///     Defines operations to create copies of an object.
/// </summary>
[Trait]
public interface IClone<S>
{
    /// <summary>
    ///     Creates a copy of the provided value.
    /// </summary>
    /// <param name="self">The value to copy.</param>
    /// <returns>The copied value.</returns>
    S Copy(S self);

    /// <summary>
    ///     Copies from the source.
    /// </summary>
    /// <param name="self">The value to modify.</param>
    /// <param name="source">The source to copy from.</param>
    // ReSharper disable once RedundantAssignment
    void Copy(ref S self, S source) => self = Copy(source);
}