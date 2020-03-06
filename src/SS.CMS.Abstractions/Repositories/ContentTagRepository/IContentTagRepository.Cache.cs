using System.Collections.Generic;
using System.Threading.Tasks;

namespace SS.CMS.Abstractions
{
    public partial interface IContentTagRepository
    {
        Task<List<string>> GetTagNamesAsync(int siteId);
    }
}