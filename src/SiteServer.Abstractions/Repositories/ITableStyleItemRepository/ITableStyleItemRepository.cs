using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;


namespace SiteServer.Abstractions
{
    public interface ITableStyleItemRepository : IRepository
    {
        Task InsertAllAsync(int tableStyleId, List<TableStyleItem> styleItems);

        Task DeleteAndInsertStyleItemsAsync(int tableStyleId, List<TableStyleItem> styleItems);

        Task<Dictionary<int, List<TableStyleItem>>> GetAllTableStyleItemsAsync();
    }
}