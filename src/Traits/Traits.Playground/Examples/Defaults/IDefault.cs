namespace Traits.Playground.Examples.Defaults;

/// <summary>
///     A trait for providing a reasonable default value for a type.
/// </summary>
[Trait]
internal interface IDefault<S>
{
    /// <summary>
    ///     Returns a default value for the type.
    /// </summary>
    /// <returns>The default value.</returns>
    S Of();
}