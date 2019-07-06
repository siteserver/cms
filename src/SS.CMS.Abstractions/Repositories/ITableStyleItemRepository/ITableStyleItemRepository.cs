using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public interface ITableStyleItemRepository : IRepository
    {
        void Insert(int tableStyleId, List<TableStyleItemInfo> styleItems);

        Task DeleteAndInsertStyleItemsAsync(int tableStyleId, List<TableStyleItemInfo> styleItems);

        Dictionary<int, List<TableStyleItemInfo>> GetAllTableStyleItems();
    }
}