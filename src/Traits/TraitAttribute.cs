using JetBrains.Annotations;

namespace Traits;

/// <summary>
///     Defines a trait.
///     <br/>
///     <para>
///         Interfaces marked with the <c>TraitAttribute</c> are processed by the <c>TraitGenerator</c>,
///         which generates a static facade class for a trait. The facade contains all the methods
///         defined in the trait interface, and should be used for calling <em>implementations</em>
///         of the trait.
///     </para>
///     <para>
///         The <c>TraitGenerator</c> also generates a trait constraint attribute for the trait.
///         This attribute can be used to restrict generic type parameters and require substituted
///         types to implement the trait.
///     </para>
/// </summary>
/// <remarks>
///     Trait interfaces must contain at least one generic type parameter, the <em>self</em> parameter,
///     which is used to denote the type that implements the trait.
/// </remarks>
/// <example>
///     Define a trait for hashable types:
///     <br/>
///     <code>
///         [Trait]
///         interface IHash&lt;S&gt;
///         {
///             int Of(S self);
///         }
///     </code>
/// 
///     <para>
///         The static <c>Hash</c> class, as well as the <c>HashAttribute</c> trait constraint,
///         will be generated for this interface. The <c>Hash</c> class can be used to compute
///         the hash of types which implement the trait.
///     </para>
/// 
///     <para>
///         The trait can then be implemented for different types as follows:
///         <br/>
///         <code>
///             record struct Point(double X, double Y);
///             <br/>
///             sealed class PointHash : IHash&lt;Point&gt;
///             {
///                 public int Of(Point self) =>
///                     HashCode.Combine(self.X, self.Y);
///             }
///         </code>
///         <br/>
///         This implementation will be automatically resolved in the <c>Hash</c> class
///         whenever an object of type <c>Point</c> is passed into one of its methods.
///         For example, consider the following call:
///         <br/>
///         <code>
///             var hash = Hash.Of(new Point(X: 1, Y: 2));
///         </code>
///         <br/>
///         In this snippet, the <c>Hash.Of</c> method actually calls the <c>PointHash.Of</c>
///         method since a <c>Point</c> object is passed into it.
///     </para>
///     <para>
///         The generated <c>HashAttribute</c> can also be used to restrict generic type parameters
///         and require them to implement the <c>IHash&lt;S&gt;</c> trait. The following will
///         compile only of the generic type parameter <c>T</c> implements the trait:
///         <br/>
///         <code>
///             int Bucket&lt;[Hash] T&gt;(T element, int size) =>
///                 Hash.Of(element) % size;
///         </code>
///     </para>
/// </example>
[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Interface)]
public class TraitAttribute : Attribute
{
}