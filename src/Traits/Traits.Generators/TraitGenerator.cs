using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
            (x, _) => x is InterfaceDeclarationSyntax type && type.HasAttribute(Types.Traits.TraitAttribute.Short),
            (x, _) => x.SemanticModel.GetDeclaredSymbol(x.Node) as INamedTypeSymbol);

        context.RegisterSourceOutput(provider, Run);
    }

    /// <summary>
    ///     Generates necessary auxiliary types for the specified trait.
    /// </summary>
    private static void Run(SourceProductionContext cx, INamedTypeSymbol? type)
    {
        if (type is null)
            return;
        if (!type.IsGenericType)
            return;
        if (!type.HasAttribute(Types.Traits.TraitAttribute))
            return;

        var name = type.Name;
        if (name.Length > 1 && name[0] == 'I' && char.IsUpper(name[1]))
            name = name.Substring(1);

        var facade = Facade(type, name);
        var attribute = Attribute(type, name);

        cx.AddSource(facade.File, facade.Source);
        cx.AddSource(attribute.File, attribute.Source);
    }

    /// <summary>
    ///     Generates the static facade class for the specified attribute.
    /// </summary>
    /// <example>
    ///     A static <c>Hash</c> class will be generated for the <c>IHash&lt;S&gt;</c> trait.
    /// </example>
    private static (string File, string Source) Facade(INamedTypeSymbol type, string name)
    {
        var methods = type.GetMembers().OfType<IMethodSymbol>().Select(x => Method(type, x, name));
        var body = string.Join("\n\n", methods);

        // TODO: Improve the trait implementation registration mechanism.
        return ($"{name}.g.cs",
            $$"""
            namespace {{type.ContainingNamespace}};

            using System.Reflection;
            using Traits;

            /// <inheritdoc cref="{{Cref(type)}}"/>
            {{SyntaxFacts.GetText(type.DeclaredAccessibility)}} static class {{name}}
            {
            #pragma warning disable CS0649
            #pragma warning disable TR2001
                private static class Impl<T>
                {
                    public static {{type.Name}}<T> Instance;
                }
            #pragma warning restore TR2001
            #pragma warning restore CS0649

                static {{name}}()
                {
                    var traits = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(x => x.GetTypes())
                        .Select(x => (Impl: x, Definition: Definition(x)))
                        .Where(x => x.Definition is not null);

                    Type Definition(Type type) =>
                        type.GetInterfaces()
                            .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof({{type.Name}}<>));

                    foreach (var trait in traits)
                    {
                        var impl = typeof(Impl<>).MakeGenericType(trait.Definition!.GenericTypeArguments.First());
                        var field = impl.GetField(nameof(Impl<object>.Instance));
                        if (field is null)
                            continue;

                        var ctor = trait.Impl.GetConstructor(Type.EmptyTypes);
                        if (ctor is null)
                            continue;

                        field.SetValue(null, ctor.Invoke(null));
                    }
                }

            {{body}}
            }
            """);

        static string Cref(ISymbol symbol) =>
            symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                .Replace('<', '{')
                .Replace('>', '}');

        static string Method(INamedTypeSymbol type, IMethodSymbol method, string name) =>
            $"""
                /// <inheritdoc cref="{Cref(type)}.{Cref(method)}"/>
                public static {(method.ReturnsVoid ? "void" : method.ReturnType)} {method.Name}<[{name}] {type.TypeArguments.First()}>({string.Join(", ", method.Parameters.Select(Parameter))}) =>
                    Impl<{type.TypeArguments.First()}>.Instance.{method.Name}({string.Join(", ", method.Parameters.Select(x => x.Name))});
            """;

        static string Parameter(IParameterSymbol parameter) =>
            $"{parameter.DeclaringSyntaxReferences[0].GetSyntax()}";
    }

    /// <summary>
    ///     Generates a trait constraint attribute for the specified trait.
    /// </summary>
    /// <example>
    ///     A <c>HashAttribute</c> trait constraint will be generated for the <c>IHash&lt;S&gt;</c> trait.
    /// </example>
    private static (string File, string Source) Attribute(INamedTypeSymbol type, string name) =>
        ($"{name}Attribute.g.cs",
            $$"""
            namespace {{type.ContainingNamespace}};

            using Traits;

            /// <summary>
            ///     Requires the marked type parameter to implement the <see cref="{{type.ContainingNamespace}}.{{name}}"/> trait.
            /// </summary>
            [For(typeof({{type.Name}}<{{new string(',', type.TypeArguments.Length - 1)}}>))]
            {{SyntaxFacts.GetText(type.DeclaredAccessibility)}} class {{name}}Attribute : ConstraintAttribute
            {
            }
            """);
}