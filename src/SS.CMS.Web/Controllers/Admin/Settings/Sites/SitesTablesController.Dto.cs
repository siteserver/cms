using System.Collections.Generic;
using Datory;

namespace SS.CMS.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesTablesController
    {
        public class GetResult
        {
            public List<string> Value { get; set; }
            public Dictionary<string, List<string>> NameDict { get; set; }
        }

        public class GetColumnsResult
        {
            public List<TableColumn> Columns { get; set; }
            public int Count { get; set; }
        }
    }
}
