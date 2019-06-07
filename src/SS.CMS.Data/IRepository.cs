using System.Collections.Generic;

namespace SS.CMS.Data
{
    public interface IRepository
    {
        DbContext DbContext { get; }

        string TableName { get; }

        List<TableColumn> TableColumns { get; }
    }
}
