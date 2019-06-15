using System.Collections.Generic;
using SS.CMS.Abstractions.Models;
using SS.CMS.Data;

namespace SS.CMS.Abstractions.Repositories
{
    public interface IContentCheckRepository : IRepository
    {
        int Insert(ContentCheckInfo checkInfo);

        IList<ContentCheckInfo> GetCheckInfoList(string tableName, int contentId);
    }
}