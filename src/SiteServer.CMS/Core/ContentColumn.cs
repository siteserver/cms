using SiteServer.Plugin;

namespace SiteServer.CMS.Core
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
