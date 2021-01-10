using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using SqlKata;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Services;

namespace SSCMS.Repositories
{
    public partial interface IContentRepository
    {
        Task<List<ContentSummary>> GetSummariesAsync(IDatabaseManager databaseManager, Site site, int channelId, int contentId,
            string groupContent, string groupContentNot, string tags, bool isImageExists, bool isImage,
            bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, bool isRelatedContents, int startNum,
            int totalNum, string orderByString, bool isTopExists, bool isTop, bool isRecommendExists, bool isRecommend,
            bool isHotExists, bool isHot, bool isColorExists, bool isColor, ScopeType scopeType, string groupChannel,
            string groupChannelNot, NameValueCollection others);

        Task<List<KeyValuePair<int, Content>>> ParserGetContentsDataSourceAsync(Site site, int channelId, int contentId,
            string groupContent, string groupContentNot, string tags, bool isImageExists, bool isImage,
            bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, bool isRelatedContents, int startNum,
            int totalNum, TaxisType taxisType, bool isTopExists, bool isTop, bool isRecommendExists, bool isRecommend,
            bool isHotExists, bool isHot, bool isColorExists, bool isColor, ScopeType scopeType, string groupChannel,
            string groupChannelNot, NameValueCollection others, Query query);
    }
}