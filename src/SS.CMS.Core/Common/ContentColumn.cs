using SS.CMS.Plugin;

namespace SS.CMS.Core.Common
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
