using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;

namespace SS.CMS.Abstractions
{
    public partial interface IDatabaseManager
    {
        Task<List<TableColumn>> GetTableColumnInfoListAsync(string tableName, List<string> excludeAttributeNameList);

        Task<List<TableColumn>> GetTableColumnInfoListAsync(string tableName, DataType excludeDataType);

        Task<TableColumn> GetTableColumnInfoAsync(string tableName, string attributeName);

        Task<bool> IsAttributeNameExistsAsync(string tableName, string attributeName);

        Task<List<string>> GetTableColumnNameListAsync(string tableName);

        Task<List<string>> GetTableColumnNameListAsync(string tableName, List<string> excludeAttributeNameList);

        Task<List<string>> GetTableColumnNameListAsync(string tableName, DataType excludeDataType);
    }

}