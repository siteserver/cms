using System.Collections.Generic;
using System.Text;
using SqlKata;
using SS.CMS.Abstractions.Models;
using SS.CMS.Data;

namespace SS.CMS.Abstractions.Repositories
{
    public interface ITagRepository : IRepository
    {
        void Insert(TagInfo tagInfo);

        void Update(TagInfo tagInfo);

        TagInfo GetTagInfo(int siteId, string tag);

        IList<TagInfo> GetTagInfoList(int siteId, int contentId);

        // string GetSqlString(int siteId, int contentId, bool isOrderByCount, int totalNum);

        IList<TagInfo> GetTagInfoList(int siteId, int contentId, bool isOrderByCount, int totalNum);

        IList<string> GetTagListByStartString(int siteId, string startString, int totalNum);

        IList<string> GetTagList(int siteId);

        void DeleteTags(int siteId);

        void DeleteTag(string tag, int siteId);

        int GetTagCount(string tag, int siteId);

        List<int> GetContentIdListByTag(string tag, int siteId);

        IList<int> GetContentIdListByTagCollection(List<string> tagCollection, int siteId);
    }
}
