using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface ITableStyleRepository : IRepository
    {
        Task<bool> IsExistsAsync(int relatedIdentity, string tableName, string attributeName);

        Task<int> InsertAsync(TableStyleInfo styleInfo);

        Task UpdateAsync(TableStyleInfo info, bool deleteAndInsertStyleItems = true);

        Task DeleteAsync(int relatedIdentity, string tableName, string attributeName);

        Task DeleteAsync(List<int> relatedIdentities, string tableName);
    }
}