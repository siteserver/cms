using System;
using System.Collections.Generic;
using System.Text;

namespace SiteServer.CMS.StlParser.Model
{
    public class AttrType : IEquatable<AttrType>, IComparable<AttrType>
    {
        public static readonly AttrType Boolean = new AttrType(nameof(Boolean));
        public static readonly AttrType DateTime = new AttrType(nameof(DateTime));
        public static readonly AttrType Decimal = new AttrType(nameof(Decimal));
        public static readonly AttrType Integer = new AttrType(nameof(Integer));
        public static readonly AttrType Enum = new AttrType(nameof(Enum));
        public static readonly AttrType String = new AttrType(nameof(String));

        private AttrType(string value)
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
            return Equals(obj as AttrType);
        }

        public static bool operator ==(AttrType a, AttrType b)
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

        public static bool operator !=(AttrType a, AttrType b)
        {
            return !(a == b);
        }

        public bool Equals(AttrType other)
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

        public int CompareTo(AttrType other)
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
