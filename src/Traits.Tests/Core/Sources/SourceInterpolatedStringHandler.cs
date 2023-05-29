using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Traits.Tests.Core.Sources;

[InterpolatedStringHandler]
internal ref struct SourceInterpolatedStringHandler
{
    private const string Default = "0";
    private const string Discard = "_";

    private readonly StringBuilder _analyze;
    private readonly StringBuilder _generate;
    private readonly Dictionary<string, DiagnosticDescriptor> _diagnostics;

    public SourceInterpolatedStringHandler(int size, int _, DiagnosticDescriptor diagnostic)
        : this(size, _, new[] { diagnostic }) { }

    public SourceInterpolatedStringHandler(int size, int _, params DiagnosticDescriptor[] descriptors)
        : this(size, _, Dictionary(descriptors)) { }

    public SourceInterpolatedStringHandler(int size, int _, Dictionary<string, DiagnosticDescriptor> diagnostics)
    {
        _analyze = new(size);
        _generate = new(size);
        _diagnostics = diagnostics;
    }

    public string Analyze => _analyze.ToString();
    public string Generate => _generate.ToString();
    public Source Source => new(Analyze, Generate);

    public void AppendLiteral(string? text)
    {
        _analyze.Append(text);
        _generate.Append(text);
    }

    public void AppendFormatted<T>(T value) =>
        AppendFormatted(value, Default);

    public void AppendFormatted<T>(T value, string format)
    {
        var text = value?.ToString();

        if (format is Discard)
            AppendLiteral(text);
        else
        {
            _analyze.Append("{|" + _diagnostics[format].Id + ":" + text + "|}");
            _generate.Append(text);
        }
    }

    private static Dictionary<string, DiagnosticDescriptor> Dictionary(DiagnosticDescriptor[] descriptors) =>
        descriptors
            .Select((x, i) => (Index: i, Value: x))
            .ToDictionary(x => x.Index.ToString(), x => x.Value);
}