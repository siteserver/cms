using BaiRong.Core;
using SiteServer.CMS.StlParser.Model;

namespace SiteServer.CMS.StlParser.Cache
{
    public class ParsedContent
    {
        private static string GetCacheKey(string stlElement, PageInfo pageInfo, ContextInfo contextInfo)
        {
            return $"{pageInfo.PublishmentSystemId}.{pageInfo.Guid}.{contextInfo.ChannelId}.{contextInfo.ContentId}.{contextInfo.IsInnerElement}.{stlElement}";
        }

        public static string GetParsedContent(string stlElement, PageInfo pageInfo, ContextInfo contextInfo)
        {
            var cacheKey = GetCacheKey(stlElement, pageInfo, contextInfo);
            return CacheUtils.Get(cacheKey) as string;
        }

        public static void SetParsedContent(string stlElement, PageInfo pageInfo, ContextInfo contextInfo, string parsedContent)
        {
            var cacheKey = GetCacheKey(stlElement, pageInfo, contextInfo);
            CacheUtils.InsertMinutes(cacheKey, parsedContent, 10);
        }
    }
}
