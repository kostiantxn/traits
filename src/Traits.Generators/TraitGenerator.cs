using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Traits.Generators.Extensions;

namespace Traits.Generators;

/// <summary>
///     Generates necessary auxiliary types for interfaces marked with the <c>TraitAttribute</c>.
/// </summary>
[Generator]
public class TraitGenerator : IIncrementalGenerator
{
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider.CreateSyntaxProvider(
            (x, _) => x is InterfaceDeclarationSyntax,
            (x, _) => x.SemanticModel.GetDeclaredSymbol(x.Node) as INamedTypeSymbol);

        context.RegisterSourceOutput(provider, Run);
    }

    /// <summary>
    ///     Generates necessary auxiliary types for the specified trait, such as
    ///     a static facade class, and a trait constraint attribute.
    /// </summary>
    /// <example>
    ///     For the <c>IHash&lt;S&gt;</c> trait, a static <c>Hash</c> class
    ///     and a <c>HashAttribute</c> trait constraint will be generated.
    /// </example>
    private static void Run(SourceProductionContext cx, INamedTypeSymbol? type)
    {
        if (type is null)
            return;

        // Trait interfaces must contain at least one generic parameter.
        if (!type.IsGenericType)
            return;

        // Traits must be marked with the `TraitAttribute`.
        if (!type.HasAttribute(Types.Traits.TraitAttribute))
            return;

        var facade = Templates.Facade(type);
        var attribute = Templates.Attribute(type);

        cx.AddSource(facade.Path, facade.Source);
        cx.AddSource(attribute.Path, attribute.Source);
    }
}