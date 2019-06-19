using System.Collections;

namespace SS.CMS.Repositories
{
    public partial interface ITagRepository
    {
        SortedList ReadContent(int siteId);
    }
}