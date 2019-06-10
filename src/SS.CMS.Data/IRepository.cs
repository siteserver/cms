using System.Collections.Generic;

namespace SS.CMS.Data
{
    public interface IRepository
    {
        IDb Db { get; }

        string TableName { get; }

        List<TableColumn> TableColumns { get; }
    }
}
