namespace Traits.Playground.Examples.Parsing;

[Trait]
internal interface IParse<S>
{
    /// <summary>
    ///     Parses a value from the specified string.
    /// </summary>
    S Value(string s);
}