using Datory;

namespace SiteServer.Abstractions
{
    public class ContentColumn : TableColumn
    {
        public string DisplayName { get; set; }

        public InputType InputType { get; set; }

        public bool IsList { get; set; }

        public bool IsCalculate { get; set; }
    }
}
