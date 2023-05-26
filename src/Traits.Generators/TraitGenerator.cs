using System.Text;
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
            (x, _) => x is InterfaceDeclarationSyntax,
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

        // Trait interfaces must contain at least one generic parameter.
        if (!type.IsGenericType)
            return;

        // Traits must be marked with the `TraitAttribute`.
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
        var accessibility = SyntaxFacts.GetText(type.DeclaredAccessibility);

        var methods = type.GetMembers().OfType<IMethodSymbol>()
            .Where(x => x.DeclaredAccessibility == Accessibility.Public)
            .Where(x => !x.IsStatic)
            .Select(x => Method(type, x, name));
        var body = string.Join("\n\n", methods);

        var self = type.TypeParameters.First();
        var parameters = type.TypeParameters.Length > 1
            ? $"<{string.Join(", ", type.TypeParameters.Skip(1).Select(Generic))}>"
            : string.Empty;

        var predicate = string.Join(
            string.Empty,
            type.TypeParameters.Skip(1).Select((x, i) => $".Where(x => x.GenericTypeArguments[{i + 1}] == typeof({x.Name}))\n                "));

        var source = new StringBuilder();

        if (!type.ContainingNamespace.IsGlobalNamespace)
            source
                .AppendLine($"namespace {type.ContainingNamespace};")
                .AppendLine();

        source.AppendLine(
            $$"""
            using System.Linq;

            /// <inheritdoc cref="{{Escape(FullName(type))}}"/>
            {{accessibility}} static class {{name}}{{parameters}}
            {
            #pragma warning disable CS0649
            #pragma warning disable TR2001
                private static class Impl<{{self}}>
                {
                    public static {{FullName(type)}} Instance;
                }
            #pragma warning restore TR2001
            #pragma warning restore CS0649

                static {{name}}()
                {
                    var traits = global::System.AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(x => x.GetTypes())
                        .Select(x => (Impl: x, Definition: Definition(x)))
                        .Where(x => x.Definition is not null);

                    global::System.Type Definition(global::System.Type type) =>
                        type.GetInterfaces()
                            .Where(x => x.IsGenericType)
                            .Where(x => x.GetGenericTypeDefinition() == typeof({{FullName(type.ConstructUnboundGenericType())}}))
                            {{predicate}}.FirstOrDefault();

                    foreach (var trait in traits)
                    {
                        var self = trait.Definition.GenericTypeArguments.First();
                        var arguments = trait.Definition.GenericTypeArguments.Skip(1).Append(self).ToArray();
                        var impl = typeof(Impl<>).MakeGenericType(arguments);

                        var instance = impl.GetField(nameof(Impl<object>.Instance));
                        if (instance is null)
                            continue;

                        var ctor = trait.Impl.GetConstructor(global::System.Type.EmptyTypes);
                        if (ctor is null)
                            continue;

                        instance.SetValue(null, ctor.Invoke(null));
                    }
                }

            {{body}}
            }
            """);

        return ($"{name}.g.cs", source.ToString());

        static string Method(INamedTypeSymbol type, IMethodSymbol method, string name)
        {
            var attribute = name + "Attribute";

            if (!type.ContainingNamespace.IsGlobalNamespace)
                attribute = "global::" + type.ContainingNamespace + "." + attribute;

            if (type.TypeParameters.Length > 1)
                attribute = attribute + "(" + string.Join(", ", type.TypeParameters.Skip(1).Select(x => "nameof(" + x.Name + ")")) + ")";

            return 
                $"""
                    /// <inheritdoc cref="{Escape(FullName(type))}.{Escape(FullName(method))}"/>
                    public static {method.ReturnType} {method.Name}<[{attribute}] {type.TypeArguments.First()}>({string.Join(", ", method.Parameters.Select(Parameter))}) =>
                        Impl<{FullName(type.TypeArguments.First())}>.Instance.{method.Name}({string.Join(", ", method.Parameters.Select(x => x.Name))});
                """;
        }

        static string Parameter(IParameterSymbol parameter) =>
            parameter.DeclaringSyntaxReferences[0].GetSyntax().ToString();
    }

    /// <summary>
    ///     Generates a trait constraint attribute for the specified trait.
    /// </summary>
    /// <example>
    ///     A <c>HashAttribute</c> trait constraint will be generated for the <c>IHash&lt;S&gt;</c> trait.
    /// </example>
    private static (string File, string Source) Attribute(INamedTypeSymbol type, string name)
    {
        var source = new StringBuilder();

        if (!type.ContainingNamespace.IsGlobalNamespace)
            source
                .AppendLine($"namespace {type.ContainingNamespace};")
                .AppendLine();

        var accessibility = SyntaxFacts.GetText(type.DeclaredAccessibility);
        var parameters = string.Join(", ", type.TypeParameters.Skip(1).Select(x => "string " + CamelCase(x.Name)));

        source.AppendLine(
            $$"""
            /// <summary>
            ///     Requires the marked type parameter to implement the <see cref="{{Escape(FullName(type))}}"/> trait.
            /// </summary>
            [global::Traits.ForAttribute(typeof({{FullName(type.ConstructUnboundGenericType())}}))]
            {{accessibility}} class {{name}}Attribute : global::Traits.ConstraintAttribute
            {
                public {{name}}Attribute({{parameters}})
                {
                }
            }
            """);

        if (type.TypeParameters.Length > 1)
        {
            var generics = string.Join(", ", type.TypeParameters.Skip(1).Select(Generic));

            source
                .AppendLine()
                .AppendLine(
                    $$"""
                    /// <summary>
                    ///     Requires the marked type parameter to implement the <see cref="{{Escape(FullName(type))}}"/> trait.
                    /// </summary>
                    [global::Traits.ForAttribute(typeof({{FullName(type.ConstructUnboundGenericType())}}))]
                    {{accessibility}} class {{name}}Attribute<{{generics}}> : global::Traits.ConstraintAttribute
                    {
                    }
                    """);
        }

        return ($"{name}Attribute.g.cs", source.ToString());
    }

    private static string FullName(ISymbol symbol) =>
        symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

    private static string Generic(ITypeParameterSymbol parameter)
    {
        var attributes = parameter.GetAttributes();
        if (attributes.Length > 0)
            return "[" + string.Join(", ", parameter.GetAttributes()) + "] " + parameter;
        else
            return parameter.ToString();
    }

    private static string Escape(string x) =>
        x.Replace('<', '{').Replace('>', '}');

    private static string CamelCase(string x) =>
        x.Length > 0 ? char.ToLower(x[0]) + x.Substring(1) : x;
}