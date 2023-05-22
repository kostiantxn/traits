using Microsoft.CodeAnalysis;

namespace Traits.Analyzers;

public static class Diagnostics
{
    public static class Trait
    {
        public static readonly DiagnosticDescriptor MustHaveAtLeastOneGenericParameter = new(
            "TR0001",
            "Generic",
            "Trait must contain at least one generic parameter",
            "Traits",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor CannotExtendOtherInterfaces = new(
            "TR0002",
            "Generic",
            "Trait cannot extend other interfaces",
            "Traits",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);
    }

    public static class Implementation
    {
        public static readonly DiagnosticDescriptor MustBeSealed = new(
            "TR1001",
            "Generic",
            "Trait implementation must be sealed",
            "Traits",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor CannotImplementOtherInterfaces = new(
            "TR1002",
            "Generic",
            "Trait implementation cannot implement other interfaces",
            "Traits",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor CanContainOnlyParameterlessConstructor = new(
            "TR1003",
            "Generic",
            "Trait implementation can contain only a parameterless constructor",
            "Traits",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor IsDuplicate = new(
            "TR1004",
            "Generic",
            "Trait implementation for type '{0}' is duplicate",
            "Traits",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor MustBeAsVisibleAsTrait = new(
            "TR1005",
            "Generic",
            "Trait implementation must be as visible as the trait itself",
            "Traits",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor CannotBeNested = new(
            "TR1006",
            "Generic",
            "Trait implementation cannot be nested",
            "Traits",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);
    }

    public static class Constraint
    {
        public static readonly DiagnosticDescriptor IsNotSatisfied = new(
            "TR2001",
            "Generic",
            "The type '{0}' must satisfy the trait constraint '{1}' in order to be used as parameter '{2}'",
            "Traits",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);
    }
}