using System.Collections;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace SS.CMS.Repositories
{
    public partial interface ITagRepository
    {
        Task<SortedList> ReadContentAsync(int siteId);
    }
}