using System.Collections.Immutable;
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
    /// <remarks>
    ///     TODO: Support other diagnostics.
    /// </remarks>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(
            Diagnostics.Trait.MustHaveAtLeastOneGenericParameter,
            Diagnostics.Trait.CannotExtendOtherInterfaces,
            Diagnostics.Constraint.IsNotSatisfied);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(x => Analyze(x, (InterfaceDeclarationSyntax) x.Node), SyntaxKind.InterfaceDeclaration);
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

        if (!symbol.HasAttribute(Types.Traits.ConstraintAttribute))
            return;

        if (symbol.Arity < 1)
            cx.ReportDiagnostic(
                Diagnostic.Create(
                    Diagnostics.Trait.MustHaveAtLeastOneGenericParameter, node.Identifier.GetLocation()));

        if (symbol.AllInterfaces.Length > 0)
            cx.ReportDiagnostic(
                Diagnostic.Create(
                    Diagnostics.Trait.CannotExtendOtherInterfaces, node.BaseList?.GetLocation()));
    }

    /// <summary>
    ///     Analyzes generic names to ensure that type parameters satisfy trait constraints.
    /// </summary>
    private static void Analyze(SyntaxNodeAnalysisContext cx, GenericNameSyntax name)
    {
        if (name.TypeArgumentList.Arguments.Count == 0)
            return;

        var info = cx.SemanticModel.GetSymbolInfo(name);
        if (info.Symbol is INamedTypeSymbol type)
        {
            for (var i = 0; i < type.TypeArguments.Length; ++i)
            {
                var parameter = type.TypeParameters[i];
                var argument = type.TypeArguments[i];
                var syntax = name.TypeArgumentList.Arguments[i];

                foreach (var trait in Violations(parameter, argument, cx.Compilation.GlobalNamespace))
                    cx.ReportDiagnostic(
                        Diagnostic.Create(
                            Diagnostics.Constraint.IsNotSatisfied, syntax.GetLocation(), argument, trait, parameter));
            }
        }

        if (info.Symbol is IMethodSymbol method && name.Parent is not InvocationExpressionSyntax)
        {
            for (var i = 0; i < method.TypeArguments.Length; ++i)
            {
                var parameter = method.TypeParameters[i];
                var argument = method.TypeArguments[i];
                var syntax = name.TypeArgumentList.Arguments[i];

                foreach (var trait in Violations(parameter, argument, cx.Compilation.GlobalNamespace))
                    cx.ReportDiagnostic(
                        Diagnostic.Create(
                            Diagnostics.Constraint.IsNotSatisfied, syntax.GetLocation(), argument, trait, parameter));
            }
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

        for (var i = 0; i < method.TypeArguments.Length; ++i)
        {
            var parameter = method.TypeParameters[i];
            var argument = method.TypeArguments[i];

            foreach (var trait in Violations(parameter, argument, cx.Compilation.GlobalNamespace))
                cx.ReportDiagnostic(
                    Diagnostic.Create(
                        Diagnostics.Constraint.IsNotSatisfied, invocation.Expression.GetLocation(), argument, trait, parameter));
        }

        // TODO: Check inferred delegates.
        foreach (var argument in operation.Arguments)
        {
        }
    }

    /// <summary>
    ///     Gets a list of traits that the specified argument does not satisfy.
    /// </summary>
    private static IEnumerable<ITypeSymbol> Violations(ISymbol parameter, ITypeSymbol argument, ISymbol root)
    {
        // Loop through attributes defined for the target type parameter.
        foreach (var attribute in parameter.GetAttributes())
        {
            // A trait constraint must inherit the `ConstraintAttribute` class
            // (e.g., `HashAttribute : ConstraintAttribute`).
            if (attribute.AttributeClass is null || !attribute.AttributeClass.Inherits(Types.Traits.ConstraintAttribute))
                continue;

            // Get the `ConstraintAttributeAttribute` definition in order to understand
            // which trait this attribute was generated for
            // (e.g., `[For(typeof(IHash<>))]` for `HashAttribute`).
            var definition = attribute.AttributeClass.GetAttribute(Types.Traits.ForAttribute);
            if (definition is null)
                continue;

            // Get the trait this attribute was generated for (e.g., `IHash<>`).
            var constructor = definition.ConstructorArguments.FirstOrDefault();
            if (constructor.Value is not ITypeSymbol trait)
                continue;

            // If the argument is a generic type parameter itself, then we need to
            // make sure that it also contains the same trait constraints.
            if (argument.TypeKind == TypeKind.TypeParameter)
            {
                if (!Constrained(argument, attribute.AttributeClass))
                    yield return trait;
            }
            else if (!Satisfies(root, argument, trait))
                yield return trait;
        }
    }

    /// <summary>
    ///     Checks whether an argument has the specified attribute.
    /// </summary>
    private static bool Constrained(ITypeSymbol type, ITypeSymbol attribute) =>
        type.HasAttribute(attribute);

    /// <summary>
    ///     Checks whether an argument implements the specified trait.
    /// </summary>
    private static bool Satisfies(ISymbol root, ITypeSymbol type, ITypeSymbol trait)
    {
        var visitor = new ImplementationsVisitor(trait);

        root.Accept(visitor);

        return visitor.Implementations.Any(x =>
            SymbolEqualityComparer.Default.Equals(x.AllInterfaces.Single().TypeArguments[0], type));
    }
}