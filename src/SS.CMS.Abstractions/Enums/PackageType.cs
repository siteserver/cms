using System;
using System.Collections.Generic;

namespace SS.CMS.Enums
{
    public sealed class PackageType : IEquatable<PackageType>, IComparable<PackageType>
    {
        public static readonly PackageType SsCms = new PackageType(nameof(SsCms));
        public static readonly PackageType Plugin = new PackageType(nameof(Plugin));
        public static readonly PackageType Library = new PackageType(nameof(Library));

        public static PackageType Parse(string val)
        {
            if (string.Equals(SsCms.Value, val, StringComparison.OrdinalIgnoreCase))
            {
                return SsCms;
            }
            if (string.Equals(Library.Value, val, StringComparison.OrdinalIgnoreCase))
            {
                return Library;
            }
            return Plugin;
        }

        private PackageType(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            Value = value;
        }

        public string Value { get; }

        public override bool Equals(object obj)
        {
            return Equals(obj as PackageType);
        }

        public static bool operator ==(PackageType a, PackageType b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if ((object)a == null || (object)b == null)
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(PackageType a, PackageType b)
        {
            return !(a == b);
        }

        public bool Equals(PackageType other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return
                Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public int CompareTo(PackageType other)
        {

            if (other == null)
            {
                return 1;
            }

            if (ReferenceEquals(this, other))
            {
                return 0;
            }

            return StringComparer.OrdinalIgnoreCase.Compare(Value, other.Value);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<string>.Default.GetHashCode(Value);
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
