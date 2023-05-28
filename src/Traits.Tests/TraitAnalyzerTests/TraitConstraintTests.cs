using Microsoft.CodeAnalysis.Testing;
using Traits.Analyzers;
using Traits.Tests.Core.Sources;
using Traits.Tests.Core.Verifiers;

namespace Traits.Tests.TraitAnalyzerTests;

public class TraitConstraintTests
{
    private const string File = "Test.cs";

    [Fact]
    public async Task EmitsNothing_OnLiteralOfImplementedType()
    {
        // lang=C#
        await Verify.Analyzer(
            Hash<int>(),
            """
            static class Test
            {
                static void Case()
                {
                    Hash.Of(42);
                }
            }
            """);
    }

    [Fact]
    public async Task EmitsError_OnLiteralOfUnimplementedType()
    {
        // lang=C#
        await Verify.Analyzer(
            Hash<int>(),
            """
            static class Test
            {
                static void Case()
                {
                    Hash.Of(42L);
                }
            }
            """.Path(File),
            DiagnosticResult
                .CompilerError(Diagnostics.Constraint.IsNotSatisfied.Id)
                .WithLocation(File, 5, 9));
    }

    [Fact]
    public async Task EmitsNothing_OnArgumentOfImplementedType()
    {
        // lang=C
        await Verify.Analyzer(
            Hash<int>(),
            """
            static class Test
            {
                static void Case(int x)
                {
                    Hash.Of(x);
                }
            }
            """);
    }

    [Fact]
    public async Task EmitsError_OnArgumentOfUnimplementedType()
    {
        // lang=C#
        await Verify.Analyzer(
            Hash<int>(),
            """
            static class Test
            {
                static void Case(double x)
                {
                    Hash.Of(x);
                }
            }
            """.Path(File),
            DiagnosticResult
                .CompilerError(Diagnostics.Constraint.IsNotSatisfied.Id)
                .WithLocation(File, 5, 9));
    }

    [Fact]
    public async Task EmitsNothing_OnArgumentOfRestrictedGenericType()
    {
        // lang=C#
        await Verify.Analyzer(
            Hash<int>(),
            """
            static class Test
            {
                static void Case<[Hash] T>(T x)
                {
                    Hash.Of(x);
                }
            }
            """);
    }

    [Theory]
    [InlineData("")]
    [InlineData("[Default]")]
    public async Task EmitsError_OnArgumentOfUnrestrictedGenericType(string attributes)
    {
        // lang=C#
        await Verify.Analyzer(
            Hash<int>(),
            Default<int>(),
            $$"""
            static class Test
            {
                static void Case<{{attributes}} T>(T x)
                {
                    Hash.Of(x);
                }
            }
            """.Path(File),
            DiagnosticResult
                .CompilerError(Diagnostics.Constraint.IsNotSatisfied.Id)
                .WithLocation(File, 5, 9));
    }

    private static string Hash<T>() =>
        Hash(typeof(T));

    private static string Hash(Type type) =>
        $$"""
        using Traits;

        [Trait]
        interface IHash<S>
        {
            int Of(S self);
        }

        sealed class {{type.Name}}Hash : IHash<{{type.FullName}}>
        {
            public int Of({{type.FullName}} self) =>
                self.GetHashCode();
        }
        """;

    private static string Default<T>() =>
        Default(typeof(T));

    private static string Default(Type type) =>
        $$"""
        using Traits;

        [Trait]
        interface IDefault<S>
        {
            S Of();
        }

        sealed class {{type.Name}}Default : IDefault<{{type.FullName}}>
        {
            public {{type.FullName}} Of() =>
                default;
        }
        """;
}