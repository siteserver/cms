using System.Collections.Generic;
using SS.CMS.Data;

namespace SS.CMS.Cli.Core
{
    public class TableInfo
    {
        public IList<TableColumn> Columns { get; set; }
        public int TotalCount { get; set; }
        public IList<string> RowFiles { get; set; }
    }
}
