using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSCMS.Repositories
{
    public partial interface IContentTagRepository
    {
        Task<List<string>> GetTagNamesAsync(int siteId);
    }
}