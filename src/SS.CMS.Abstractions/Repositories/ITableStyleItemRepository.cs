using System.Collections.Generic;
using SS.CMS.Abstractions.Models;
using SS.CMS.Data;

namespace SS.CMS.Abstractions.Repositories
{
    public interface ITableStyleItemRepository : IRepository
    {
        void Insert(int tableStyleId, List<TableStyleItemInfo> styleItems);

        void DeleteAndInsertStyleItems(int tableStyleId, List<TableStyleItemInfo> styleItems);

        Dictionary<int, List<TableStyleItemInfo>> GetAllTableStyleItems();
    }
}