using System.Collections.Generic;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public interface ITableStyleItemRepository : IRepository
    {
        void Insert(int tableStyleId, List<TableStyleItemInfo> styleItems);

        void DeleteAndInsertStyleItems(int tableStyleId, List<TableStyleItemInfo> styleItems);

        Dictionary<int, List<TableStyleItemInfo>> GetAllTableStyleItems();
    }
}