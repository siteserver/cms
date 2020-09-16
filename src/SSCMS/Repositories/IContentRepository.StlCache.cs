using System.Threading.Tasks;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public partial interface IContentRepository
    {

        string GetStlWhereString(int siteId, string group, string groupNot, string tags, bool isImageExists,
            bool isImage, bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, bool isTopExists,
            bool isTop, bool isRecommendExists, bool isRecommend, bool isHotExists, bool isHot, bool isColorExists,
            bool isColor, string where);

        string GetStlWhereStringBySearch(string group, string groupNot, bool isImageExists, bool isImage,
            bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, bool isTopExists, bool isTop,
            bool isRecommendExists, bool isRecommend, bool isHotExists, bool isHot, bool isColorExists, bool isColor,
            string where);

        string GetStlWhereString(int siteId, string group, string groupNot, string tags, bool isTopExists, bool isTop,
            string where);

        Task<int> GetContentIdAsync(string tableName, int channelId, int taxis, bool isNextContent);

        int GetContentId(string tableName, int channelId, bool isCheckedOnly, string orderByString);

        Task<int> GetSequenceAsync(string tableName, int siteId, int channelId, int contentId);

        Task<int> GetCountCheckedImageAsync(Site site, Channel channel);
    }
}