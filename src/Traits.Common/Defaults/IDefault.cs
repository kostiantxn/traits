namespace Traits.Common.Defaults;

/// <summary>
///     Provides a reasonable default value for a type.
/// </summary>
[Trait]
public interface IDefault<S>
{
    /// <summary>
    ///     Returns a default value for the type.
    /// </summary>
    /// <returns>The default value.</returns>
    S Of();
}