using static System.AttributeTargets;

namespace Traits;

/// <summary>
///     Inherited by concrete trait constraint attributes.
/// </summary>
/// <example>
///     Consider a simple trait:
///     <code>
///         [Trait]
///         interface IHash&lt;T&gt;
///         {
///             int Of(T value);
///         }
///     </code>
///     <para/>
///     The following trait constraint attribute will be generated for this trait:
///     <code>
///         [ConstraintAttribute(typeof(IHash&lt;&gt;)]
///         class HashAttribute : ConstraintAttribute
///         {
///         }
///     </code>
/// </example>
// [AttributeUsage(Parameter | GenericParameter | Field | Property | ReturnValue, AllowMultiple = true)]
[AttributeUsage(GenericParameter, AllowMultiple = true)]
public abstract class ConstraintAttribute : Attribute
{
}