using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface ITagRepository : IRepository
    {
        Task<int> InsertAsync(Tag tagInfo);

        Task<bool> UpdateAsync(Tag tagInfo);

        Task<Tag> GetTagInfoAsync(int siteId, string tag);

        Task<IEnumerable<Tag>> GetTagInfoListAsync(int siteId, int contentId);

        Task<IEnumerable<string>> GetTagListByStartStringAsync(int siteId, string startString, int totalNum);

        Task<IEnumerable<string>> GetTagListAsync(int siteId);

        Task DeleteTagsAsync(int siteId);

        Task DeleteTagAsync(string tag, int siteId);

        Task<int> GetTagCountAsync(string tag, int siteId);

        Task<IEnumerable<int>> GetContentIdListByTagAsync(string tag, int siteId);
    }
}
