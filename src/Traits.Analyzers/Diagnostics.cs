using Microsoft.CodeAnalysis;

namespace Traits.Analyzers;

/// <summary>
///     A collection of diagnostics that <see cref="TraitAnalyzer"/> emits.
/// </summary>
public static class Diagnostics
{
    /// <summary>
    ///     Diagnostics related to trait definitions.
    /// </summary>
    /// <example>
    ///     A typical trait definition:
    ///     <code>
    ///         [Trait]
    ///         interface IHash&lt;S&gt;
    ///         {
    ///             int Of(S self);
    ///         }
    ///     </code>
    /// </example>
    public static class Trait
    {
        public static readonly DiagnosticDescriptor MustHaveAtLeastOneGenericParameter = new(
            "TR0001",
            "Traits must contain at least one generic parameter",
            "Traits must contain at least one generic parameter.",
            "Traits",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor ShouldNotExtendOtherInterfaces = new(
            "TR0002",
            "Trait should not extend other interfaces",
            "Trait should not extend other interfaces.",
            "Traits",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);
    }

    /// <summary>
    ///     Diagnostics related to trait implementations.
    /// </summary>
    /// <example>
    ///     A typical trait implementation:
    ///     <code>
    ///         sealed class IntHash : IHash&lt;int&gt;
    ///         {
    ///             public int Of(int self) =>
    ///                 self;
    ///         }
    ///     </code>
    /// </example>
    public static class Implementation
    {
        public static readonly DiagnosticDescriptor MustBeSealed = new(
            "TR1001",
            "Trait implementation should be sealed",
            "Trait implementation should be sealed.",
            "Traits",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor MustNotImplementOtherInterfaces = new(
            "TR1002",
            "Trait implementation should not implement other interfaces",
            "Trait implementation should not implement other interfaces.",
            "Traits",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor MustContainParameterlessConstructor = new(
            "TR1003",
            "Trait implementation must contain a parameterless constructor",
            "Trait implementation must contain a parameterless constructor.",
            "Traits",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor MustBeAsVisibleAsTrait = new(
            "TR1004",
            "Trait implementation should be as visible as the trait itself",
            "Trait implementation should be as visible as the trait itself.",
            "Traits",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor MustNotBeNested = new(
            "TR1005",
            "Trait implementation should not be nested",
            "Trait implementation should not be nested.",
            "Traits",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor Conflict = new(
            "TR1006",
            "Trait implementations should be unique and unambiguous",
            "Conflicting implementation of trait '{0}'.",
            "Traits",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);
    }

    /// <summary>
    ///     Diagnostics related to trait constraints.
    /// </summary>
    /// <example>
    ///     A typical trait constraint:
    ///     <code>
    ///         int Bucket&lt;[Hash] T&gt;(T item, int size) =>
    ///             Hash.Of(item) % size;
    ///     </code>
    /// </example>
    public static class Constraint
    {
        public static readonly DiagnosticDescriptor IsNotSatisfied = new(
            "TR2001",
            "Trait constraints must be satisfied",
            "The type '{0}' must satisfy trait constraint '{1}' in order to be used as parameter '{2}'.",
            "Traits",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);
    }
}