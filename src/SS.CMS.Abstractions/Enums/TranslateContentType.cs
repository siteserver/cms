using System;

namespace SS.CMS.Abstractions.Enums
{
    public sealed class TranslateContentType
    {
        public static readonly TranslateContentType Copy = new TranslateContentType("Copy");
        public static readonly TranslateContentType Cut = new TranslateContentType("Cut");
        public static readonly TranslateContentType Reference = new TranslateContentType("Reference");
        public static readonly TranslateContentType ReferenceContent = new TranslateContentType("ReferenceContent");

        private TranslateContentType(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }

        public static TranslateContentType Parse(string val)
        {
            if (string.Equals(Copy.Value, val, StringComparison.OrdinalIgnoreCase))
            {
                return Copy;
            }
            if (string.Equals(Cut.Value, val, StringComparison.OrdinalIgnoreCase))
            {
                return Cut;
            }
            if (string.Equals(Reference.Value, val, StringComparison.OrdinalIgnoreCase))
            {
                return Reference;
            }
            if (string.Equals(ReferenceContent.Value, val, StringComparison.OrdinalIgnoreCase))
            {
                return ReferenceContent;
            }
            return Copy;
        }
    }
}
