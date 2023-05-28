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
            Hash.Definition() + Hash.For<int>(),
            """
            class Test
            {
                void Case()
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
            Hash.Definition() + Hash.For<int>(),
            """
            class Test
            {
                void Case()
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
        // lang=C#
        await Verify.Analyzer(
            Hash.Definition() + Hash.For<int>(),
            """
            class Test
            {
                void Case(int x)
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
            Hash.Definition() + Hash.For<int>(),
            """
            class Test
            {
                void Case(double x)
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
            Hash.Definition() + Hash.For<int>(),
            """
            class Test
            {
                void Case<[Hash] T>(T x)
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
            Hash.Definition() + Hash.For<int>(),
            Default.Definition() + Default.For<int>(),
            $$"""
            class Test
            {
                void Case<{{attributes}} T>(T x)
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
    public async Task EmitsNothing_WhenTraitRequiresAnotherTrait_AndItIsImplemented()
    {
        // lang=C#
        await Verify.Analyzer(
            $$"""
            using Traits;
            
            [Trait] interface ISemigroup<S> { }
            [Trait] interface IMonoid<[Semigroup] S> { }
            
            sealed class IntSemigroup : ISemigroup<int> { }
            sealed class IntMonoid : IMonoid<int> { }
            """);
    }

    [Fact]
    public async Task EmitsError_WhenTraitRequiresAnotherTrait_AndItIsNotImplemented()
    {
        // lang=C#
        await Verify.Analyzer(
            $$"""
            using Traits;
            
            [Trait] interface ISemigroup<S> { }
            [Trait] interface IMonoid<[Semigroup] S> { }
            
            sealed class IntMonoid : IMonoid<int> { }
            """.Path(File),
            DiagnosticResult
                .CompilerError(Diagnostics.Constraint.IsNotSatisfied.Id)
                .WithLocation(File, 6, 26));
    }

    [Fact]
    public async Task EmitsNothing_WhenTraitImpliesAnotherTrait()
    {
        // lang=C#
        await Verify.Analyzer(
            Monoid.Definition(),
            Semigroup.Definition(),
            $$"""
            class Test
            {
                void Case<[Monoid] T>(T x)
                {
                    Semigroup.Dot(x, Monoid.Zero<T>());
                }
            }
            """);
    }

    private static class Hash
    {
        public static string Definition() =>
            """
            using Traits;

            [Trait]
            interface IHash<S>
            {
                int Of(S self);
            }
            """ + "\n\n";

        public static string For<T>() =>
            For(typeof(T));

        public static string For(Type type) =>
            $$"""
            sealed class {{type.Name}}Hash : IHash<{{type.FullName}}>
            {
                public int Of({{type.FullName}} self) =>
                    throw new System.NotImplementedException();
            }
            """;
    }

    private static class Default
    {
        public static string Definition() =>
            $$"""
            using Traits;

            [Trait]
            interface IDefault<S>
            {
                S Of();
            }
            """ + "\n\n";

        public static string For<T>() =>
            For(typeof(T));

        public static string For(Type type) =>
            $$"""
            sealed class {{type.Name}}Default : IDefault<{{type.FullName}}>
            {
                public {{type.FullName}} Of() =>
                    default;
            }
            """;
    }

    private static class Semigroup
    {
        public static string Definition() =>
            """
            using Traits;

            [Trait]
            interface ISemigroup<S> 
            { 
                S Dot(S x, S y);
            }
            """ + "\n\n";

        public static string For<T>() =>
            For(typeof(T));

        public static string For(Type type) =>
            $$"""
            sealed class {{type.Name}}Semigroup : ISemigroup<{{type.FullName}}>
            {
                public {{type.FullName}} Dot({{type.FullName}} x, {{type.FullName}} y) =>
                    throw new System.NotImplementedException();
            }

            sealed class {{type.Name}}Monoid : IMonoid<{{type.FullName}}>
            {
                public {{type.FullName}} Zero() =>
                    throw new System.NotImplementedException();
            }
            """;
    }

    private static class Monoid
    {
        public static string Definition() =>
            """
            using Traits;

            [Trait]
            interface IMonoid<[Semigroup] S>
            { 
                S Zero();
            }
            """ + "\n\n";

        public static string For<T>() =>
            For(typeof(T));

        public static string For(Type type) =>
            $$"""
            sealed class {{type.Name}}Monoid : IMonoid<{{type.FullName}}>
            {
                public {{type.FullName}} Zero() =>
                    throw new System.NotImplementedException();
            }
            """;
    }
}