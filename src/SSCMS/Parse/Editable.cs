using SSCMS.Utils;

namespace SSCMS.Parse
{
    public class Editable
    {
        public string Id { get; set; }
        public string ElementName { get; set; }
        public string StlElement { get; set; }
        public string ParsedContent { get; set; }
        public string EditedContent { get; set; }
        public string File { get; set; }
        public int Index { get; set; }
    }
}
