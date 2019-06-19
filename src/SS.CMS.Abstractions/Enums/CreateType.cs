using System;

namespace SS.CMS.Enums
{
    public sealed class CreateType
    {
        public static readonly CreateType Channel = new CreateType("Channel");
        public static readonly CreateType Content = new CreateType("Content");
        public static readonly CreateType File = new CreateType("File");
        public static readonly CreateType Special = new CreateType("Special");
        public static readonly CreateType AllContent = new CreateType("AllContent");

        private CreateType(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }

        public static CreateType Parse(string val)
        {
            if (string.Equals(Content.Value, val, StringComparison.OrdinalIgnoreCase))
            {
                return Content;
            }
            if (string.Equals(File.Value, val, StringComparison.OrdinalIgnoreCase))
            {
                return File;
            }
            if (string.Equals(Special.Value, val, StringComparison.OrdinalIgnoreCase))
            {
                return Special;
            }
            if (string.Equals(AllContent.Value, val, StringComparison.OrdinalIgnoreCase))
            {
                return AllContent;
            }
            return Channel;
        }
    }
}
