

using SS.CMS.Abstractions;

namespace SS.CMS
{
    public class ContentColumn
    {
        public string AttributeName { get; set; }

        public string DisplayName { get; set; }

        public InputType InputType { get; set; }

        public bool IsList { get; set; }

        public bool IsCalculate { get; set; }
    }
}
