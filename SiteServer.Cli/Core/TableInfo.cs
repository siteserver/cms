using System.Collections.Generic;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Core
{
    public class TableInfo
    {
        public List<TableColumnInfo> Columns { get; set; }
        public int TotalCount { get; set; }
        public List<string> RowFiles { get; set; }
    }
}
