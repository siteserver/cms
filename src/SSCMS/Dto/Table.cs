using System.Collections.Generic;
using Datory;

namespace SSCMS.Dto
{
    public class Table
    {
        public List<TableColumn> Columns { get; set; }
        public int TotalCount { get; set; }
        public List<string> Rows { get; set; }
    }
}
