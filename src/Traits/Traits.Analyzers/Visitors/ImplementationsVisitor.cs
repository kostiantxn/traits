using Microsoft.CodeAnalysis;

namespace Traits.Analyzers.Visitors;

/// <summary>
///     Searches for all implementations of a specific trait.
/// </summary>
internal class ImplementationsVisitor : SymbolVisitor
{
    private readonly ITypeSymbol _trait;
    private readonly HashSet<INamedTypeSymbol> _implementations = new(SymbolEqualityComparer.Default);

    public ImplementationsVisitor(ITypeSymbol trait) =>
        _trait = trait.OriginalDefinition;

    public IReadOnlyCollection<INamedTypeSymbol> Implementations => _implementations;

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

        if (SymbolEqualityComparer.Default.Equals(@interface.OriginalDefinition, _trait))
            _implementations.Add(symbol);
    }
}