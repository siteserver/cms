using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface IContentGroupRepository
    {
        Task<Dictionary<int, List<ContentGroup>>> GetAllContentGroupsAsync();

        Task<bool> IsExistsAsync(int siteId, string groupName);

        Task<ContentGroup> GetContentGroupInfoAsync(int siteId, string groupName);

        Task<List<string>> GetGroupNameListAsync(int siteId);

        Task<List<ContentGroup>> GetContentGroupInfoListAsync(int siteId);
    }
}