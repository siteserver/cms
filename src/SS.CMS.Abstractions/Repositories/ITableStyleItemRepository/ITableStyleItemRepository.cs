using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public interface ITableStyleItemRepository : IRepository
    {
        Task InsertAllAsync(int tableStyleId, List<TableStyleItem> styleItems);

        Task DeleteAndInsertStyleItemsAsync(int tableStyleId, List<TableStyleItem> styleItems);

        Task<Dictionary<int, List<TableStyleItem>>> GetAllTableStyleItemsAsync();
    }
}