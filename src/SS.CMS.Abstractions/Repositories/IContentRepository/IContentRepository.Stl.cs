using System.Collections.Generic;
using SS.CMS.Models;

namespace SS.CMS.Repositories.IContentRepository
{
    public partial interface IContentRepository
    {
        List<KeyValuePair<int, ContentInfo>> GetContainerContentListBySqlString(string sqlString, string orderString, int totalCount, int itemsPerPage, int currentPageIndex);
    }
}
