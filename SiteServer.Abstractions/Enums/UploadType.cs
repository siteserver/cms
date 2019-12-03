using System;


namespace SiteServer.Abstractions
{
    public sealed class UploadType
    {
        public static readonly UploadType Image = new UploadType("Image");
        public static readonly UploadType Video = new UploadType("Video");
        public static readonly UploadType File = new UploadType("File");
        public static readonly UploadType Special = new UploadType("Special");

        private UploadType(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }

        public static UploadType Parse(string val)
        {
            if (string.Equals(Video.Value, val, StringComparison.OrdinalIgnoreCase))
            {
                return Video;
            }
            if (string.Equals(File.Value, val, StringComparison.OrdinalIgnoreCase))
            {
                return File;
            }
            if (string.Equals(Special.Value, val, StringComparison.OrdinalIgnoreCase))
            {
                return Special;
            }
            return Image;
        }
    }
}
