using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public interface ITableStyleItemRepository : IRepository
    {
        Task InsertAllAsync(int tableStyleId, List<TableStyleItemInfo> styleItems);

        Task DeleteAndInsertStyleItemsAsync(int tableStyleId, List<TableStyleItemInfo> styleItems);

        Task<Dictionary<int, List<TableStyleItemInfo>>> GetAllTableStyleItemsAsync();
    }
}