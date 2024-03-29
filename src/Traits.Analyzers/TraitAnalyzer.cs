﻿using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using Traits.Analyzers.Extensions;
using Traits.Analyzers.Visitors;

namespace Traits.Analyzers;

/// <summary>
///     An analyzer which ensures that traits are used correctly.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class TraitAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(
            Diagnostics.Trait.MustHaveAtLeastOneGenericParameter,
            Diagnostics.Trait.ShouldNotExtendOtherInterfaces,
            Diagnostics.Implementation.MustBeSealed,
            Diagnostics.Implementation.MustNotImplementOtherInterfaces,
            Diagnostics.Implementation.MustContainParameterlessConstructor,
            Diagnostics.Implementation.MustBeAsAccessibleAsTrait,
            Diagnostics.Implementation.MustNotBeNested,
            Diagnostics.Implementation.Conflict,
            Diagnostics.Constraint.IsNotSatisfied);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(x => Analyze(x, (InterfaceDeclarationSyntax) x.Node), SyntaxKind.InterfaceDeclaration);
        context.RegisterSyntaxNodeAction(x => Analyze(x, (TypeDeclarationSyntax) x.Node), SyntaxKind.ClassDeclaration);
        context.RegisterSyntaxNodeAction(x => Analyze(x, (GenericNameSyntax) x.Node), SyntaxKind.GenericName);
        context.RegisterOperationAction(x => Analyze(x, (IInvocationOperation) x.Operation), OperationKind.Invocation);
    }

    /// <summary>
    ///     Analyzes interface declarations to ensure that trait definitions are correct.
    /// </summary>
    private static void Analyze(SyntaxNodeAnalysisContext cx, InterfaceDeclarationSyntax node)
    {
        var symbol = cx.SemanticModel.GetDeclaredSymbol(node);
        if (symbol is null)
            return;

        if (!symbol.HasAttribute(Types.Traits.TraitAttribute))
            return;

        if (symbol.Arity < 1)
            Diagnostics.Trait.MustHaveAtLeastOneGenericParameter
                .Report(cx, loc: node.Identifier);

        if (symbol.AllInterfaces.Length > 0)
            Diagnostics.Trait.ShouldNotExtendOtherInterfaces
                .Report(cx, loc: node.BaseList!);
    }

    /// <summary>
    ///     Analyzes class declarations to ensure that trait implementations are correct.
    /// </summary>
    private static void Analyze(SyntaxNodeAnalysisContext cx, TypeDeclarationSyntax node)
    {
        var impl = cx.SemanticModel.GetDeclaredSymbol(node);
        if (impl is null)
            return;

        var trait = impl.AllInterfaces.FirstOrDefault(x => x.HasAttribute(Types.Traits.TraitAttribute));
        if (trait is null)
            return;

        if (!impl.IsSealed)
            Diagnostics.Implementation.MustBeSealed
                .Report(cx, loc: node.Identifier);

        if (impl.AllInterfaces.Length > 1)
            Diagnostics.Implementation.MustNotImplementOtherInterfaces
                .Report(cx, loc: node.Identifier);

        if (impl.DeclaredAccessibility != trait.DeclaredAccessibility)
            Diagnostics.Implementation.MustBeAsAccessibleAsTrait
                .Report(cx, loc: node.Identifier);

        if (impl.InstanceConstructors.Length > 0 &&
            impl.InstanceConstructors.All(x => x.Parameters.Length > 0 || x.DeclaredAccessibility != Accessibility.Public))
            Diagnostics.Implementation.MustContainParameterlessConstructor
                .Report(cx, loc: node.Identifier);

        if (impl.ContainingSymbol is ITypeSymbol)
            Diagnostics.Implementation.MustNotBeNested
                .Report(cx, loc: node.Identifier);

        var visitor = new ConflictsVisitor(impl);

        cx.Compilation.GlobalNamespace.Accept(visitor);

        if (visitor.Conflicts.Count > 0)
            Diagnostics.Implementation.Conflict
                .Report(cx, loc: node.Identifier, trait);
    }

    /// <summary>
    ///     Analyzes generic names to ensure that type parameters satisfy trait constraints.
    /// </summary>
    private static void Analyze(SyntaxNodeAnalysisContext cx, GenericNameSyntax name)
    {
        if (name.TypeArgumentList.Arguments.Count == 0)
            return;

        if (name.IsPartOfStructuredTrivia())
            return;

        var info = cx.SemanticModel.GetSymbolInfo(name);
        if (info.Symbol is INamedTypeSymbol type)
        {
            if (type.IsUnboundGenericType)
                return;

            foreach (var violation in Violations(type, cx.Compilation))
                Diagnostics.Constraint.IsNotSatisfied
                    .Report(cx, loc: name, violation.Argument, violation.Trait, violation.Parameter);
        }

        if (info.Symbol is IMethodSymbol method)
        {
            foreach (var violation in Violations(method, cx.Compilation))
                Diagnostics.Constraint.IsNotSatisfied
                    .Report(cx, loc: name, violation.Argument, violation.Trait, violation.Parameter);
        }
    }

    /// <summary>
    ///     Analyzes invocation operations to ensure that implicitly inferred type parameters
    ///     satisfy trait constraints.
    /// </summary>
    private static void Analyze(OperationAnalysisContext cx, IInvocationOperation operation)
    {
        var method = operation.TargetMethod;
        if (method.Arity == 0)
            return;

        if (operation.Syntax is not InvocationExpressionSyntax invocation)
            return;

        var expression = invocation.Expression is MemberAccessExpressionSyntax member
            ? member.Name
            : invocation.Expression;

        // Analyze the invoked expression only if it has implicit type arguments.
        // It will otherwise be analyzed as a `GenericNameSyntax` node.
        if (expression is not GenericNameSyntax)
            foreach (var violation in Violations(method, cx.Compilation))
                Diagnostics.Constraint.IsNotSatisfied
                    .Report(cx, loc: expression, violation.Argument, violation.Trait, violation.Parameter);

        // TODO: Check inferred delegates.
        foreach (var _ in operation.Arguments)
        {
        }
    }

    private static IEnumerable<(ITypeSymbol Parameter, ITypeSymbol Argument, ITypeSymbol Trait)> Violations(IMethodSymbol method, Compilation compilation) =>
        Violations(method.TypeParameters, method.TypeArguments, compilation);

    private static IEnumerable<(ITypeSymbol Parameter, ITypeSymbol Argument, ITypeSymbol Trait)> Violations(INamedTypeSymbol type, Compilation compilation) =>
        Violations(type.TypeParameters, type.TypeArguments, compilation);

    private static IEnumerable<(ITypeSymbol, ITypeSymbol, ITypeSymbol)> Violations(
        ImmutableArray<ITypeParameterSymbol> parameters,
        ImmutableArray<ITypeSymbol> arguments,
        Compilation compilation)
    {
        for (var i = 0; i < parameters.Length; ++i)
        {
            var parameter = parameters[i];
            var argument = arguments[i];

            var constraints = Constraints(parameter);
            if (constraints.Count == 0)
                continue;

            var implementations = Implementations(argument, compilation);

            foreach (var constraint in constraints)
            {
                var trait = Substitute(constraint);

                if (!implementations.Contains(trait, SymbolEqualityComparer.Default))
                    yield return (parameter, argument, trait);
            }
        }

        ITypeSymbol Substitute(INamedTypeSymbol constraint)
        {
            var substituted = constraint.TypeArguments.ToArray();

            for (var i = 0; i < constraint.Arity; ++i)
            for (var j = 0; j < parameters.Length; ++j)
                substituted[i] = substituted[i].Substitute(parameters[j], arguments[j]);

            return constraint.OriginalDefinition.Construct(substituted);
        }
    }

    /// <summary>
    ///     Returns all traits this parameter requires.
    /// </summary>
    private static ISet<INamedTypeSymbol> Constraints(ITypeParameterSymbol parameter)
    {
        var set = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);

        foreach (var attribute in parameter.GetAttributes())
        {
            // A trait constraint must inherit the `ConstraintAttribute` class
            // (e.g., `HashAttribute : ConstraintAttribute`).
            if (attribute.AttributeClass is null || !attribute.AttributeClass.Inherits(Types.Traits.ConstraintAttribute))
                continue;

            // Get the `ForAttribute` definition in order to understand which trait this attribute
            // was generated for (e.g., `[For(typeof(IHash<>))]` for `HashAttribute`).
            var @for = attribute.AttributeClass.GetAttribute(Types.Traits.ForAttribute);
            if (@for is null)
                continue;

            // Get the trait this attribute was generated for (e.g., `IHash<>`).
            var type = @for.ConstructorArguments.FirstOrDefault();
            if (type.Value is not INamedTypeSymbol { IsGenericType: true } trait)
                continue;

            // Substitute trait type parameters with type arguments from the attribute
            // (e.g., `IFrom<,>` -> `IFrom<T, string>` for `<[From<string>] T>`).
            var arguments = new List<ITypeSymbol> { parameter };

            // Extract `nameof` arguments for generic type parameters
            // (e.g., for signatures like `<[Into(nameof(Y))] X, Y>`).
            if (attribute.ConstructorArguments.Length > 0)
            {
                foreach (var argument in attribute.ConstructorArguments)
                {
                    if (argument.Value is not string name)
                        continue;

                    // TODO: Report a diagnostic if the referenced type cannot be found.
                    // TODO: Resolve the type from a semantic model if it references a concrete type.
                    var referenced = Referenced(name, parameter.ContainingSymbol);
                    if (referenced is not null)
                        arguments.Add(referenced);
                }
            }
            // Otherwise, if the attribute is generic, extract its type arguments
            // (e.g., for signatures like `<[Into<string>] T>`).
            else if (attribute.AttributeClass.IsGenericType)
                arguments.AddRange(attribute.AttributeClass.TypeArguments);

            if (!set.Add(trait.OriginalDefinition.Construct(arguments.ToArray())))
                continue;

            // Add all traits implied by the current trait. For example, if the current 
            // type parameter is marked with `[Monoid] T`, then it is implied that `T` also
            // implements the `Semigroup` trait, and it should thus be added to the set.
            var implications = Constraints(trait.TypeParameters.First());

            foreach (var implication in implications)
            {
                var substituted = implication;
                for (var i = 0; i < trait.Arity; ++i)
                    substituted = substituted.Substitute(trait.TypeParameters[i], arguments[i]);

                set.Add(substituted);
            }
        }

        return set;

        static ITypeSymbol? Referenced(string name, ISymbol symbol)
        {
            if (symbol is IMethodSymbol method)
            {
                var index = method.TypeParameters.IndexOf(x => x.Name == name);
                if (index >= 0)
                    return method.TypeArguments[index];
            }

            if (symbol is INamedTypeSymbol type)
            {
                var index = type.TypeParameters.IndexOf(x => x.Name == name);
                if (index >= 0)
                    return type.TypeArguments[index];
            }

            return symbol is not INamespaceSymbol ? Referenced(name, symbol.ContainingSymbol) : null;
        }
    }

    /// <summary>
    ///     Returns all traits this argument implements.
    /// </summary>
    private static ISet<INamedTypeSymbol> Implementations(ITypeSymbol argument, Compilation compilation)
    {
        if (argument is ITypeParameterSymbol parameter)
            return Constraints(parameter);
        else
        {
            var visitor = new TraitsVisitor(argument);

            compilation.GlobalNamespace.Accept(visitor);

            return visitor.Implementations;
        }
    }
}