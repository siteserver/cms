using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;


namespace SiteServer.Abstractions
{
    public partial interface IContentTagRepository
    {
        Task UpdateContentTagsAsync(string tagsPrevious, string tagsNow, int siteId, int contentId);

        Task RemoveContentTagsAsync(int siteId, IEnumerable<int> contentIdList);

        Task RemoveContentTagsAsync(int siteId, int contentId);

        string GetContentTagsString(StringCollection tags);

        List<string> ParseContentTagsString(string tagsString);

        IList<ContentTag> GetContentTagInfoList(IEnumerable<ContentTag> tagInfoList);

        IList<ContentTag> GetContentTagInfoList(IEnumerable<ContentTag> tagInfoList, int totalNum, int tagLevel);
    }
}
