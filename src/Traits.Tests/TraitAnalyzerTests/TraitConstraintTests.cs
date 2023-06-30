using Traits.Analyzers;
using Traits.Tests.Core.Verifiers;

namespace Traits.Tests.TraitAnalyzerTests;

public class TraitConstraintTests
{
    [Fact]
    public async Task EmitsNothing_OnLiteralOfImplementedType()
    {
        // lang=C#
        await Verify.Analyzer(
            """
            class Test
            {
                void Case()
                {
                    Hash.Of(42);
                    Hash.Of<int>(42);
                }
            }
            """,
            Hash.Definition() + Hash.For<int>());
    }

    [Fact]
    public async Task EmitsError_OnLiteralOfUnimplementedType()
    {
        // lang=C#
        await Verify.Analyzer(
            Diagnostics.Constraint.IsNotSatisfied,
            $$"""
            class Test
            {
                void Case()
                {
                    Hash.{{"Of"}}(42L);
                    Hash.{{"Of<long>"}}(42L);
                }
            }
            """,
            Hash.Definition() + Hash.For<int>());
    }

    [Fact]
    public async Task EmitsNothing_OnArgumentOfImplementedType()
    {
        // lang=C#
        await Verify.Analyzer(
            """
            class Test
            {
                void Case(int x)
                {
                    Hash.Of(x);
                    Hash.Of<int>(x);
                }
            }
            """,
            Hash.Definition() + Hash.For<int>());
    }

    [Fact]
    public async Task EmitsError_OnArgumentOfUnimplementedType()
    {
        // lang=C#
        await Verify.Analyzer(
            Diagnostics.Constraint.IsNotSatisfied,
            $$"""
            class Test
            {
                void Case(long x)
                {
                    Hash.{{"Of"}}(x);
                    Hash.{{"Of<long>"}}(x);
                }
            }
            """,
            Hash.Definition() + Hash.For<int>());
    }

    [Fact]
    public async Task EmitsNothing_OnArgumentOfRestrictedGenericType()
    {
        // lang=C#
        await Verify.Analyzer(
            """
            class Test
            {
                void Case<[Hash] T>(T x)
                {
                    Hash.Of(x);
                    Hash.Of<T>(x);
                }
            }
            """,
            Hash.Definition() + Hash.For<int>());
    }

    [Theory]
    [InlineData("")]
    [InlineData("[Default]")]
    public async Task EmitsError_OnArgumentOfUnrestrictedGenericType(string attributes)
    {
        // lang=C#
        await Verify.Analyzer(
            Diagnostics.Constraint.IsNotSatisfied,
            $$"""
            class Test
            {
                void Case<{{attributes:_}} T>(T x)
                {
                    Hash.{{"Of"}}(x);
                    Hash.{{"Of<T>"}}(x);
                }
            }
            """,
            Hash.Definition() + Hash.For<int>(),
            Default.Definition() + Default.For<int>());
    }

    [Fact]
    public async Task EmitsNothing_WhenTraitRequiresAnotherTrait_AndItIsImplemented()
    {
        // lang=C#
        await Verify.Analyzer(
            """
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
            Diagnostics.Constraint.IsNotSatisfied,
            $$"""
            using Traits;
            
            [Trait] interface ISemigroup<S> { }
            [Trait] interface IMonoid<[Semigroup] S> { }
            
            sealed class IntMonoid : {{"IMonoid<int>"}} { }
            """);
    }

    [Fact]
    public async Task EmitsNothing_WhenTraitImpliesAnotherTrait()
    {
        // lang=C#
        await Verify.Analyzer(
            """
            class Test
            {
                void Case<[Monoid] T>(T x)
                {
                    Semigroup.Dot(x, Monoid.Zero<T>());
                }
            }
            """,
            Monoid.Definition(),
            Semigroup.Definition());
    }

    [Fact]
    public async Task EmitsError_WhenGenericClassRequiresTrait_AndItIsNotImplemented()
    {
        // lang=C#
        await Verify.Analyzer(
            Diagnostics.Constraint.IsNotSatisfied,
            $$"""
            class Test
            {
                void Case()
                {
                    {{"Collection<int>"}} _ = new {{"Collection<int>"}}();
                }
            }
            
            class Collection<[Hash] T>
            {
            }
            """,
            Hash.Definition());
    }

    [Fact]
    public async Task EmitsNothing_WhenGenericClassRequiresTrait_AndItIsImplemented()
    {
        // lang=C#
        await Verify.Analyzer(
            """
            class Test
            {
                void Case()
                {
                    Collection<int> _ = new Collection<int>();
                }
            }
            
            class Collection<[Hash] T>
            {
            }
            """,
            Hash.Definition() + Hash.For<int>());
    }

    #region Traits

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
            """
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

    #endregion
}