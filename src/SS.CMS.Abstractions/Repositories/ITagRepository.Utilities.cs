using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface ITagRepository
    {
        Task UpdateTagsAsync(string tagsPrevious, string tagsNow, int siteId, int contentId);

        void RemoveTags(int siteId, IList<int> contentIdList);

        void RemoveTags(int siteId, int contentId);

        string GetTagsString(StringCollection tags);

        List<string> ParseTagsString(string tagsString);

        IList<TagInfo> GetTagInfoList(IList<TagInfo> tagInfoList);

        IList<TagInfo> GetTagInfoList(IList<TagInfo> tagInfoList, int totalNum, int tagLevel);
    }
}
