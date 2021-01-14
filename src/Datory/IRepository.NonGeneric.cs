using System.Collections.Generic;

namespace Datory
{
    public interface IRepository
    {
        IDatabase Database { get; }

        string TableName { get; }

        List<TableColumn> TableColumns { get; }
    }
}
