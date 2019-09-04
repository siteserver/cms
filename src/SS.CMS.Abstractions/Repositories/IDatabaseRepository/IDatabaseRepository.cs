using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;

namespace SS.CMS.Repositories
{
    public partial interface IDatabaseRepository
    {
        IDatabase Database { get; }

        Task<IEnumerable<TableColumn>> GetTableColumnInfoListAsync(string tableName);

        Task<IEnumerable<TableColumn>> GetTableColumnInfoListAsync(string tableName, List<string> excludeAttributeNameList);

        Task<IEnumerable<TableColumn>> GetTableColumnInfoListAsync(string tableName, DataType excludeDataType);

        Task<TableColumn> GetTableColumnInfoAsync(string tableName, string attributeName);

        Task<bool> IsAttributeNameExistsAsync(string tableName, string attributeName);

        Task<List<string>> GetTableColumnNameListAsync(string tableName);

        Task<List<string>> GetTableColumnNameListAsync(string tableName, List<string> excludeAttributeNameList);

        Task<List<string>> GetTableColumnNameListAsync(string tableName, DataType excludeDataType);

        List<TableColumn> ContentTableDefaultColumns { get; }
    }
}
