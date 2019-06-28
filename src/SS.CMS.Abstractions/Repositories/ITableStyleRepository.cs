using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface ITableStyleRepository : IRepository
    {
        bool IsExists(int relatedIdentity, string tableName, string attributeName);

        int Insert(TableStyleInfo styleInfo);

        Task UpdateAsync(TableStyleInfo info, bool deleteAndInsertStyleItems = true);

        Task DeleteAsync(int relatedIdentity, string tableName, string attributeName);

        Task DeleteAsync(List<int> relatedIdentities, string tableName);
    }
}