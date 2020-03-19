using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSCMS.Abstractions
{
    public partial interface IContentTagRepository
    {
        Task<List<string>> GetTagNamesAsync(int siteId);
    }
}