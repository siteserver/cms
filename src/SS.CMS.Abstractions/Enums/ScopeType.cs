using System;

namespace SS.CMS.Abstractions.Enums
{
    public sealed class ScopeType
    {
        public static readonly ScopeType Self = new ScopeType("Self");
        public static readonly ScopeType Children = new ScopeType("Children");
        public static readonly ScopeType SelfAndChildren = new ScopeType("SelfAndChildren");
        public static readonly ScopeType Descendant = new ScopeType("Descendant");
        public static readonly ScopeType All = new ScopeType("All");

        private ScopeType(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }

        public static ScopeType Parse(string val)
        {
            if (string.Equals(Children.Value, val, StringComparison.OrdinalIgnoreCase))
            {
                return Children;
            }
            if (string.Equals(SelfAndChildren.Value, val, StringComparison.OrdinalIgnoreCase))
            {
                return SelfAndChildren;
            }
            if (string.Equals(Descendant.Value, val, StringComparison.OrdinalIgnoreCase))
            {
                return Descendant;
            }
            if (string.Equals(All.Value, val, StringComparison.OrdinalIgnoreCase))
            {
                return All;
            }
            return Self;
        }
    }
}
