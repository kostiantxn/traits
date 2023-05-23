using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Traits.Analyzers;

/// <summary>
///     Extensions for reporting diagnostics.
/// </summary>
internal static class DiagnosticsExtensions
{
    public static void Report(this DiagnosticDescriptor self, Action<Diagnostic> report, Location loc, params object[] args)
    {
        report(Diagnostic.Create(self, loc, args.Select(Encode).ToArray()));

        object Encode(object x) =>
            x.ToString().Replace('<', '{').Replace('>', '}');
    }

    public static void Report(this DiagnosticDescriptor self, SyntaxNodeAnalysisContext cx, Location loc, params object[] args) =>
        self.Report(cx.ReportDiagnostic, loc, args);

    public static void Report(this DiagnosticDescriptor self, SyntaxNodeAnalysisContext cx, SyntaxNode loc, params object[] args) =>
        self.Report(cx, loc.GetLocation(), args);

    public static void Report(this DiagnosticDescriptor self, SyntaxNodeAnalysisContext cx, SyntaxToken loc, params object[] args) =>
        self.Report(cx, loc.GetLocation(), args);

    public static void Report(this DiagnosticDescriptor self, SymbolAnalysisContext cx, Location loc, params object[] args) =>
        self.Report(cx.ReportDiagnostic, loc, args);

    public static void Report(this DiagnosticDescriptor self, SymbolAnalysisContext cx, SyntaxNode loc, params object[] args) =>
        self.Report(cx, loc.GetLocation(), args);

    public static void Report(this DiagnosticDescriptor self, SymbolAnalysisContext cx, SyntaxToken loc, params object[] args) =>
        self.Report(cx, loc.GetLocation(), args);

    public static void Report(this DiagnosticDescriptor self, OperationAnalysisContext cx, Location loc, params object[] args) =>
        self.Report(cx.ReportDiagnostic, loc, args);

    public static void Report(this DiagnosticDescriptor self, OperationAnalysisContext cx, SyntaxNode loc, params object[] args) =>
        self.Report(cx, loc.GetLocation(), args);

    public static void Report(this DiagnosticDescriptor self, OperationAnalysisContext cx, SyntaxToken loc, params object[] args) =>
        self.Report(cx, loc.GetLocation(), args);
}