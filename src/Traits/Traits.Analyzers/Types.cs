namespace Traits.Analyzers;

/// <summary>
///     Full type names of some important types.
/// </summary>
internal static class Types
{
    public class Info
    {
        private readonly string _namespace;
        private readonly string _name;

        public Info(string @namespace, string name)
        {
            _namespace = @namespace;
            _name = name;
        }

        public string Short => _name;
        public string Full => $"{_namespace}.{_name}";

        public override string ToString() =>
            Full;

        public static implicit operator string(Info info) =>
            info.ToString();
    }

    /// <summary>
    ///     Types located in the <c>Traits</c> namespace.
    /// </summary>
    public static class Traits
    {
        /// <summary>
        ///     Full type name of the <c>TraitAttribute</c> type.
        /// </summary>
        public static readonly Info TraitAttribute =
            new("Traits", nameof(TraitAttribute));

        /// <summary>
        ///     Full type name of the <c>ConstraintAttribute</c> type.
        /// </summary>
        public static readonly Info ConstraintAttribute =
            new("Traits", nameof(ConstraintAttribute));

        /// <summary>
        ///     Full type name of the <c>ForAttribute</c> type.
        /// </summary>
        public static readonly Info ForAttribute =
            new("Traits", nameof(ForAttribute));
    }
}