using System.Collections.Generic;
using Datory;
using SiteServer.Plugin;
using TableColumn = Datory.TableColumn;

namespace SiteServer.Cli.Core
{
    public class TableInfo
    {
        public List<TableColumn> Columns { get; set; }
        public int TotalCount { get; set; }
        public List<string> RowFiles { get; set; }
    }
}
