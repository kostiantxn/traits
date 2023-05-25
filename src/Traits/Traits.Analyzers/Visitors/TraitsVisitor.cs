using Microsoft.CodeAnalysis;

namespace Traits.Analyzers.Visitors;

/// <summary>
///     Searches for all traits a specific type implements.
/// </summary>
internal class TraitsVisitor : SymbolVisitor
{
    private readonly ITypeSymbol _type;
    private readonly HashSet<INamedTypeSymbol> _implementations = new(SymbolEqualityComparer.Default);

    public TraitsVisitor(ITypeSymbol type) =>
        _type = type;

    public ISet<INamedTypeSymbol> Implementations => _implementations;

    public override void VisitAssembly(IAssemblySymbol symbol) =>
        symbol.GlobalNamespace.Accept(this);

    public override void VisitModule(IModuleSymbol symbol) => 
        symbol.GlobalNamespace.Accept(this);

    public override void VisitNamespace(INamespaceSymbol symbol)
    {
        foreach (var type in symbol.GetMembers())
            type.Accept(this);
    }

    public override void VisitNamedType(INamedTypeSymbol symbol)
    {
        // Implementations cannot be abstract.
        if (symbol.IsAbstract)
            return;

        // Implementations must always be sealed.
        if (!symbol.IsSealed)
            return;

        // Implementations must implement the trait only.
        if (symbol.AllInterfaces.Length != 1)
            return;

        var @interface = symbol.AllInterfaces.Single();
        if (@interface.TypeArguments.Length < 1)
            return;

        var type = @interface.TypeArguments.First();

        if (SymbolEqualityComparer.Default.Equals(type, _type))
            _implementations.Add(@interface);
    }
}