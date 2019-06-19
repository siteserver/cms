using System.Collections.Generic;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface ITableStyleRepository : IRepository
    {
        bool IsExists(int relatedIdentity, string tableName, string attributeName);

        int Insert(TableStyleInfo styleInfo);

        void Update(TableStyleInfo info, bool deleteAndInsertStyleItems = true);

        void Delete(int relatedIdentity, string tableName, string attributeName);

        void Delete(List<int> relatedIdentities, string tableName);
    }
}