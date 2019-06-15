using SS.CMS.Abstractions.Enums;
using SS.CMS.Data;

namespace SS.CMS.Abstractions
{
    public class ContentColumn : TableColumn
    {
        public string DisplayName { get; set; }

        public InputType InputType { get; set; }

        public bool IsList { get; set; }

        public bool IsCalculate { get; set; }
    }
}
