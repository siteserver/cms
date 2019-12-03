using System.Collections.Generic;
using System.Threading.Tasks;


namespace SiteServer.Abstractions
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