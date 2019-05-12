using System.Collections.Generic;
using Datory;

namespace SiteServer.CMS.Data
{
    public interface IDatabaseDao
    {
        string TableName { get; }
        List<TableColumn> TableColumns { get; }
    }
}
