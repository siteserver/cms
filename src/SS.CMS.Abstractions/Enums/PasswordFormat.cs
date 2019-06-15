using System;

namespace SS.CMS.Abstractions.Enums
{
    public sealed class PasswordFormat
    {
        public static readonly PasswordFormat Clear = new PasswordFormat("Clear");
        public static readonly PasswordFormat Hashed = new PasswordFormat("Hashed");
        public static readonly PasswordFormat Encrypted = new PasswordFormat("Encrypted");

        private PasswordFormat(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }

        public static PasswordFormat Parse(string val)
        {
            if (string.Equals(Clear.Value, val, StringComparison.OrdinalIgnoreCase))
            {
                return Clear;
            }
            else if (string.Equals(Hashed.Value, val, StringComparison.OrdinalIgnoreCase))
            {
                return Hashed;
            }
            return Encrypted;
        }
    }
}