using System.Collections.Generic;

namespace SS.CMS.Data
{
    public interface IRepository
    {
        DatabaseType DatabaseType { get; }

        string ConnectionString { get; }

        string TableName { get; }

        List<TableColumn> TableColumns { get; }
    }
}
