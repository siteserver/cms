using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;

namespace SS.CMS.Services
{
    public partial interface ITableManager
    {
        Task CreateContentTableAsync(string tableName, IList<TableColumn> tableColumns);

        void SyncContentTables();

        List<TableColumn> ContentTableDefaultColumns { get; }
    }

}
