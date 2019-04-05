using System.Collections.Generic;

namespace Datory
{
    public interface IRepository
    {
        DatabaseType DatabaseType { get; }

        string ConnectionString { get; }

        string TableName { get; }

        List<TableColumn> TableColumns { get; }
    }
}
