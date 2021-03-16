using System.Collections.Generic;

namespace SSCMS.Parse
{
    public class Editable
    {
        public string ElementId { get; set; }
        public string ElementName { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public string InnerHtml { get; set; }
        public string StlElement { get; set; }
        public string IncludeFile { get; set; }
        public int StartIndex { get; set; }
        public bool IsChanged { get; set; }
    }
}
