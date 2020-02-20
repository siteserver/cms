using System.Collections.Generic;
using System.Threading.Tasks;

namespace SiteServer.Abstractions
{
    public partial interface IContentTagRepository
    {
        Task<List<string>> GetTagNamesAsync(int siteId);
    }
}