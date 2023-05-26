using Microsoft.CodeAnalysis;
using Traits.Analyzers.Extensions;

namespace Traits.Analyzers.Visitors;

/// <summary>
///     Searches for conflicting implementations of a trait.
/// </summary>
internal class ConflictsVisitor : SymbolVisitor
{
    private readonly ITypeSymbol _implementation;
    private readonly ITypeSymbol _trait;
    private readonly HashSet<INamedTypeSymbol> _conflicts = new(SymbolEqualityComparer.Default);

    public ConflictsVisitor(ITypeSymbol implementation)
    {
        _implementation = implementation;
        _trait = implementation.AllInterfaces.Single(x => x.HasAttribute(Types.Traits.TraitAttribute));
    }

    public ISet<INamedTypeSymbol> Conflicts => _conflicts;

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

        if (SymbolEqualityComparer.Default.Equals(symbol, _implementation))
            return;

        var @interface = symbol.AllInterfaces.Single();
        if (@interface.TypeArguments.Length < 1)
            return;

        if (SymbolEqualityComparer.Default.Equals(@interface, _trait))
            _conflicts.Add(symbol);
    }
}