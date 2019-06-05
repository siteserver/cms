using System.Collections.Generic;
using SS.CMS.Data;

namespace SS.CMS.Core.Common
{
    public interface IDatabaseDao
    {
        string TableName { get; }
        List<TableColumn> TableColumns { get; }
    }
}
