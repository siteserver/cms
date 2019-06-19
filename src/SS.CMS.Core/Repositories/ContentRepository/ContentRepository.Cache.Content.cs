using System.Collections.Generic;
using SS.CMS.Models;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public partial class ContentRepository
    {
        private readonly object ContentLockObject = new object();
        private readonly string ContentCachePrefix = StringUtils.GetCacheKey(nameof(ContentRepository)) + "." + "Content";

        private string ContentGetContentCacheKey(int channelId)
        {
            return $"{ContentCachePrefix}.{channelId}";
        }

        private void ContentRemove(int channelId)
        {
            lock (ContentLockObject)
            {
                var cacheKey = ContentGetContentCacheKey(channelId);
                _cacheManager.Remove(cacheKey);
            }
        }

        private Dictionary<int, ContentInfo> ContentGetContentDict(int channelId)
        {
            lock (ContentLockObject)
            {
                var cacheKey = ContentGetContentCacheKey(channelId);
                var dict = _cacheManager.Get<Dictionary<int, ContentInfo>>(cacheKey);
                if (dict == null)
                {
                    dict = new Dictionary<int, ContentInfo>();
                    _cacheManager.InsertHours(cacheKey, dict, 12);
                }

                return dict;
            }
        }

        private ContentInfo ContentGetContent(SiteInfo siteInfo, ChannelInfo channelInfo, int contentId)
        {
            lock (ContentLockObject)
            {
                var dict = ContentGetContentDict(channelInfo.Id);
                dict.TryGetValue(contentId, out var contentInfo);
                if (contentInfo != null && contentInfo.ChannelId == channelInfo.Id && contentInfo.Id == contentId) return contentInfo;

                contentInfo = GetCacheContentInfo(contentId);
                dict[contentId] = contentInfo;

                return contentInfo;
            }
        }

        public ContentInfo GetContentInfo(SiteInfo siteInfo, ChannelInfo channelInfo, int contentId)
        {
            return ContentGetContent(siteInfo, channelInfo, contentId);
        }
    }
}