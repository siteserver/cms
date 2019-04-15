using System.Collections.Generic;
using Datory;

namespace SiteServer.Cli.Core
{
    public class TableInfo
    {
        public List<TableColumn> Columns { get; set; }
        public int TotalCount { get; set; }
        public List<string> RowFiles { get; set; }
    }
}
