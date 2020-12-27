using SSCMS.Enums;

namespace SSCMS.Configuration
{
    public class ContentColumn
    {
        public string AttributeName { get; set; }
        public string DisplayName { get; set; }
        public InputType InputType { get; set; }
        public int Width { get; set; }
        public bool IsList { get; set; }
        public bool IsSearchable { get; set; }
    }
}
