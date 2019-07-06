using System.Collections.Generic;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public interface IContentCheckRepository : IRepository
    {
        int Insert(ContentCheckInfo checkInfo);

        IList<ContentCheckInfo> GetCheckInfoList(string tableName, int contentId);
    }
}