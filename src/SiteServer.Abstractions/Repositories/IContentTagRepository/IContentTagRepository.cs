using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;


namespace SiteServer.Abstractions
{
    public partial interface IContentTagRepository : IRepository
    {
        Task<int> InsertAsync(ContentTag tagInfo);

        Task<bool> UpdateAsync(ContentTag tagInfo);

        Task<ContentTag> GetContentTagInfoAsync(int siteId, string tag);

        Task<IEnumerable<ContentTag>> GetContentTagInfoListAsync(int siteId, int contentId);

        Task<IEnumerable<string>> GetContentTagListByStartStringAsync(int siteId, string startString, int totalNum);

        Task<IEnumerable<string>> GetContentTagListAsync(int siteId);

        Task DeleteContentTagsAsync(int siteId);

        Task DeleteContentTagAsync(string tag, int siteId);

        Task<int> GetContentTagCountAsync(string tag, int siteId);

        Task<IEnumerable<int>> GetContentIdListByContentTagAsync(string tag, int siteId);
    }
}
