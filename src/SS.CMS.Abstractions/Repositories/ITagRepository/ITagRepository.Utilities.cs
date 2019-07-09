using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface ITagRepository
    {
        Task UpdateTagsAsync(string tagsPrevious, string tagsNow, int siteId, int contentId);

        Task RemoveTagsAsync(int siteId, IEnumerable<int> contentIdList);

        Task RemoveTagsAsync(int siteId, int contentId);

        string GetTagsString(StringCollection tags);

        List<string> ParseTagsString(string tagsString);

        IList<TagInfo> GetTagInfoList(IEnumerable<TagInfo> tagInfoList);

        IList<TagInfo> GetTagInfoList(IEnumerable<TagInfo> tagInfoList, int totalNum, int tagLevel);
    }
}
