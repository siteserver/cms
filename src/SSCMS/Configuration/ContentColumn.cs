using SSCMS.Enums;
using System.Collections.Generic;

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
        public bool IsExtend { get; set; }
        public List<InputStyleItem> Items { get; set; }
    }
}