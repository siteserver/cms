using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;


namespace SiteServer.Abstractions
{
    public partial interface ITableStyleRepository : IRepository
    {
        Task<bool> IsExistsAsync(int relatedIdentity, string tableName, string attributeName);

        Task<int> InsertAsync(TableStyle styleInfo);

        Task UpdateAsync(TableStyle info, bool deleteAndInsertStyleItems = true);

        Task DeleteAsync(int relatedIdentity, string tableName, string attributeName);

        Task DeleteAsync(List<int> relatedIdentities, string tableName);
    }
}