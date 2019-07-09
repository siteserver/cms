using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface ITagRepository : IRepository
    {
        Task<int> InsertAsync(TagInfo tagInfo);

        Task<bool> UpdateAsync(TagInfo tagInfo);

        Task<TagInfo> GetTagInfoAsync(int siteId, string tag);

        Task<IEnumerable<TagInfo>> GetTagInfoListAsync(int siteId, int contentId);

        Task<IEnumerable<string>> GetTagListByStartStringAsync(int siteId, string startString, int totalNum);

        Task<IEnumerable<string>> GetTagListAsync(int siteId);

        Task DeleteTagsAsync(int siteId);

        Task DeleteTagAsync(string tag, int siteId);

        Task<int> GetTagCountAsync(string tag, int siteId);

        Task<IEnumerable<int>> GetContentIdListByTagAsync(string tag, int siteId);
    }
}
