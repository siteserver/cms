using System.Collections.Generic;

namespace SS.CMS.Data
{
    public interface IRepository
    {
        IDatabase Database { get; }

        string TableName { get; }

        List<TableColumn> TableColumns { get; }
    }
}
