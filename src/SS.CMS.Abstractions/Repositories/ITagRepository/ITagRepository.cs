using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface ITagRepository : IRepository
    {
        void Insert(TagInfo tagInfo);

        void Update(TagInfo tagInfo);

        TagInfo GetTagInfo(int siteId, string tag);

        IList<TagInfo> GetTagInfoList(int siteId, int contentId);

        IList<string> GetTagListByStartString(int siteId, string startString, int totalNum);

        Task<IEnumerable<string>> GetTagListAsync(int siteId);

        Task DeleteTagsAsync(int siteId);

        Task DeleteTagAsync(string tag, int siteId);

        int GetTagCount(string tag, int siteId);

        List<int> GetContentIdListByTag(string tag, int siteId);
    }
}
