using BaiRong.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Core
{
	public class TagStyleManager
	{
        private TagStyleManager()
		{
		}

        public static TagStyleInfo GetTagStyleInfo(int styleID)
        {
            var styleInfo = DataProvider.TagStyleDao.GetTagStyleInfo(styleID);
            if (styleInfo == null)
            {
                styleInfo = new TagStyleInfo();
            }
            return styleInfo;
        }

        public static TagStyleInfo GetTagStyleInfo(int publishmentSystemID, string elementName, string styleName)
        {
            if (publishmentSystemID == 0 || string.IsNullOrEmpty(elementName) || string.IsNullOrEmpty(styleName))
            {
                return null;
            }
            var cacheKey = GetCacheKey(publishmentSystemID, elementName, styleName);
            lock (lockObject)
            {
                if (CacheUtils.Get(cacheKey) == null)
                {
                    var tagStyleInfo = DataProvider.TagStyleDao.GetTagStyleInfo(publishmentSystemID, elementName, styleName);
                    CacheUtils.Insert(cacheKey, tagStyleInfo, 30);
                    return tagStyleInfo;
                }
                return CacheUtils.Get(cacheKey) as TagStyleInfo;
            }
        }

        public static void RemoveCache(int publishmentSystemID, string elementName, string styleName)
        {
            var cacheKey = GetCacheKey(publishmentSystemID, elementName, styleName);
            CacheUtils.Remove(cacheKey);
        }

        private static string GetCacheKey(int publishmentSystemID, string elementName, string styleName)
        {
            return $"SiteServer.CMS.Core.TagStyleManager.{publishmentSystemID}.{elementName}.{styleName}";
        }

        private static readonly object lockObject = new object();
	}
}
