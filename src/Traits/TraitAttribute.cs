using JetBrains.Annotations;
using static System.AttributeTargets;

namespace Traits;

/// <summary>
///     Defines a trait.
/// </summary>
/// <example>
///     <code>
///         [Trait]
///         interface IHash&lt;T&gt;
///         {
///             int Of(T value);
///         }
///     </code>
/// </example>
[MeansImplicitUse]
[AttributeUsage(Interface)]
public class TraitAttribute : Attribute
{
}