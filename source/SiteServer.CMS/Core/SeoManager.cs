using System.Text;
using System.Collections.Generic;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Net;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Core
{
	public class SeoManager
	{
		private SeoManager()
		{
		}

        public static SeoMetaInfo GetSeoMetaInfo(string content)
        {
            var seoMetaInfo = new SeoMetaInfo();

            if (!string.IsNullOrEmpty(content))
            {
                var metaRegex = @"<title>(?<meta>[^<]*?)</title>";
                var metaContent = RegexUtils.GetContent("meta", metaRegex, content);
                if (!string.IsNullOrEmpty(metaContent))
                {
                    seoMetaInfo.PageTitle = metaContent;
                }

                metaRegex = @"<META\s+NAME=(?:""|')keywords(?:""|')\s+CONTENT=(?:""|')(?<meta>[^<]*?)(?:""|')(?:[^>]*)>|<META\s+CONTENT=(?:""|')(?<meta>[^<]*?)(?:""|')\s+NAME=(?:""|')keywords(?:""|')(?:[^>]*)>";
                metaContent = RegexUtils.GetContent("meta", metaRegex, content);
                if (!string.IsNullOrEmpty(metaContent))
                {
                    seoMetaInfo.Keywords = metaContent;
                }

                metaRegex = @"<META\s+NAME=(?:""|')description(?:""|')\s+CONTENT=(?:""|')(?<meta>[^<]*?)(?:""|')(?:[^>]*)>|<META\s+CONTENT=(?:""|')(?<meta>[^<]*?)(?:""|')\s+NAME=(?:""|')description(?:""|')(?:[^>]*)>";
                metaContent = RegexUtils.GetContent("meta", metaRegex, content);
                if (!string.IsNullOrEmpty(metaContent))
                {
                    seoMetaInfo.Description = metaContent;
                }

                metaRegex = @"<META\s+NAME=(?:""|')copyright(?:""|')\s+CONTENT=(?:""|')(?<meta>[^<]*?)(?:""|')(?:[^>]*)>|<META\s+CONTENT=(?:""|')(?<meta>[^<]*?)(?:""|')\s+NAME=(?:""|')copyright(?:""|')(?:[^>]*)>";
                metaContent = RegexUtils.GetContent("meta", metaRegex, content);
                if (!string.IsNullOrEmpty(metaContent))
                {
                    seoMetaInfo.Copyright = metaContent;
                }

                metaRegex = @"<META\s+NAME=(?:""|')author(?:""|')\s+CONTENT=(?:""|')(?<meta>[^<]*?)(?:""|')(?:[^>]*)>|<META\s+CONTENT=(?:""|')(?<meta>[^<]*?)(?:""|')\s+NAME=(?:""|')author(?:""|')(?:[^>]*)>";
                metaContent = RegexUtils.GetContent("meta", metaRegex, content);
                if (!string.IsNullOrEmpty(metaContent))
                {
                    seoMetaInfo.Author = metaContent;
                }

                metaRegex = @"<META\s+NAME=(?:""|')email(?:""|')\s+CONTENT=(?:""|')(?<meta>[^<]*?)(?:""|')(?:[^>]*)>|<META\s+CONTENT=(?:""|')(?<meta>[^<]*?)(?:""|')\s+NAME=(?:""|')email(?:""|')(?:[^>]*)>";
                metaContent = RegexUtils.GetContent("meta", metaRegex, content);
                if (!string.IsNullOrEmpty(metaContent))
                {
                    seoMetaInfo.Email = metaContent;
                }

                metaRegex = @"<META\s+NAME=(?:""|')language(?:""|')\s+CONTENT=(?:""|')(?<meta>[^<]*?)(?:""|')(?:[^>]*)>|<META\s+CONTENT=(?:""|')(?<meta>[^<]*?)(?:""|')\s+NAME=(?:""|')language(?:""|')(?:[^>]*)>";
                metaContent = RegexUtils.GetContent("meta", metaRegex, content);
                if (!string.IsNullOrEmpty(metaContent))
                {
                    seoMetaInfo.Language = metaContent;
                }

                metaRegex = @"<META\s+NAME=(?:""|')charset(?:""|')\s+CONTENT=(?:""|')(?<meta>[^<]*?)(?:""|')(?:[^>]*)>|<META\s+CONTENT=(?:""|')(?<meta>[^<]*?)(?:""|')\s+NAME=(?:""|')charset(?:""|')(?:[^>]*)>";
                metaContent = RegexUtils.GetContent("meta", metaRegex, content);
                if (!string.IsNullOrEmpty(metaContent))
                {
                    seoMetaInfo.Charset = metaContent;
                }

                metaRegex = @"<META\s+NAME=(?:""|')distribution(?:""|')\s+CONTENT=(?:""|')(?<meta>[^<]*?)(?:""|')(?:[^>]*)>|<META\s+CONTENT=(?:""|')(?<meta>[^<]*?)(?:""|')\s+NAME=(?:""|')distribution(?:""|')(?:[^>]*)>";
                metaContent = RegexUtils.GetContent("meta", metaRegex, content);
                if (!string.IsNullOrEmpty(metaContent))
                {
                    seoMetaInfo.Distribution = metaContent;
                }

                metaRegex = @"<META\s+NAME=(?:""|')rating(?:""|')\s+CONTENT=(?:""|')(?<meta>[^<]*?)(?:""|')(?:[^>]*)>|<META\s+CONTENT=(?:""|')(?<meta>[^<]*?)(?:""|')\s+NAME=(?:""|')rating(?:""|')(?:[^>]*)>";
                metaContent = RegexUtils.GetContent("meta", metaRegex, content);
                if (!string.IsNullOrEmpty(metaContent))
                {
                    seoMetaInfo.Rating = metaContent;
                }

                metaRegex = @"<META\s+NAME=(?:""|')robots(?:""|')\s+CONTENT=(?:""|')(?<meta>[^<]*?)(?:""|')(?:[^>]*)>|<META\s+CONTENT=(?:""|')(?<meta>[^<]*?)(?:""|')\s+NAME=(?:""|')robots(?:""|')(?:[^>]*)>";
                metaContent = RegexUtils.GetContent("meta", metaRegex, content);
                if (!string.IsNullOrEmpty(metaContent))
                {
                    seoMetaInfo.Robots = metaContent;
                }

                metaRegex = @"<META\s+NAME=(?:""|')revisit-after(?:""|')\s+CONTENT=(?:""|')(?<meta>[^<]*?)(?:""|')(?:[^>]*)>|<META\s+CONTENT=(?:""|')(?<meta>[^<]*?)(?:""|')\s+NAME=(?:""|')revisit-after(?:""|')(?:[^>]*)>";
                metaContent = RegexUtils.GetContent("meta", metaRegex, content);
                if (!string.IsNullOrEmpty(metaContent))
                {
                    seoMetaInfo.RevisitAfter = metaContent;
                }

                metaRegex = @"<META\s+NAME=(?:""|')expires(?:""|')\s+CONTENT=(?:""|')(?<meta>[^<]*?)(?:""|')(?:[^>]*)>|<META\s+CONTENT=(?:""|')(?<meta>[^<]*?)(?:""|')\s+NAME=(?:""|')expires(?:""|')(?:[^>]*)>";
                metaContent = RegexUtils.GetContent("meta", metaRegex, content);
                if (!string.IsNullOrEmpty(metaContent))
                {
                    seoMetaInfo.Expires = metaContent;
                }
            }

            return seoMetaInfo;
        }

        public static SeoMetaInfo GetSeoMetaInfo(string siteUrl, ECharset charset)
        {
            siteUrl = PageUtils.AddProtocolToUrl(siteUrl);
            var content = WebClientUtils.GetRemoteFileSource(siteUrl, charset);
            return GetSeoMetaInfo(content);
        }


        public static string GetMetaContent(SeoMetaInfo seoMetaInfo)
        {
            var codeBuilder = new StringBuilder();
            if (!string.IsNullOrEmpty(seoMetaInfo.PageTitle))
            {
                codeBuilder.Append($"<TITLE>{seoMetaInfo.PageTitle}</TITLE>\r\n");
            }
            if (!string.IsNullOrEmpty(seoMetaInfo.Keywords))
            {
                codeBuilder.Append($"<META NAME=\"keywords\" CONTENT=\"{seoMetaInfo.Keywords}\">\r\n");
            }
            if (!string.IsNullOrEmpty(seoMetaInfo.Description))
            {
                codeBuilder.Append($"<META NAME=\"description\" CONTENT=\"{seoMetaInfo.Description}\">\r\n");
            }
            if (!string.IsNullOrEmpty(seoMetaInfo.Copyright))
            {
                codeBuilder.Append($"<META NAME=\"copyright\" CONTENT=\"{seoMetaInfo.Copyright}\">\r\n");
            }
            if (!string.IsNullOrEmpty(seoMetaInfo.Author))
            {
                codeBuilder.Append($"<META NAME=\"author\" CONTENT=\"{seoMetaInfo.Author}\">\r\n");
            }
            if (!string.IsNullOrEmpty(seoMetaInfo.Email))
            {
                codeBuilder.Append($"<META NAME=\"email\" CONTENT=\"{seoMetaInfo.Email}\">\r\n");
            }
            if (!string.IsNullOrEmpty(seoMetaInfo.Language))
            {
                codeBuilder.Append($"<META NAME=\"language\" CONTENT=\"{seoMetaInfo.Language}\">\r\n");
            }
            if (!string.IsNullOrEmpty(seoMetaInfo.Charset))
            {
                codeBuilder.Append($"<META NAME=\"charset\" CONTENT=\"{seoMetaInfo.Charset}\">\r\n");
            }
            if (!string.IsNullOrEmpty(seoMetaInfo.Distribution))
            {
                codeBuilder.Append($"<META NAME=\"distribution\" CONTENT=\"{seoMetaInfo.Distribution}\">\r\n");
            }
            if (!string.IsNullOrEmpty(seoMetaInfo.Rating))
            {
                codeBuilder.Append($"<META NAME=\"rating\" CONTENT=\"{seoMetaInfo.Rating}\">\r\n");
            }
            if (!string.IsNullOrEmpty(seoMetaInfo.Robots))
            {
                codeBuilder.Append($"<META NAME=\"robots\" CONTENT=\"{seoMetaInfo.Robots}\">\r\n");
            }
            if (!string.IsNullOrEmpty(seoMetaInfo.RevisitAfter))
            {
                codeBuilder.Append($"<META NAME=\"revisit-after\" CONTENT=\"{seoMetaInfo.RevisitAfter}\">\r\n");
            }
            if (!string.IsNullOrEmpty(seoMetaInfo.Expires))
            {
                codeBuilder.Append($"<META NAME=\"expires\" CONTENT=\"{seoMetaInfo.Expires}\">\r\n");
            }
            return codeBuilder.ToString();
        }

        public static List<int>[] GetSeoMetaArrayLists(int publishmentSystemId)
        {
            var cacheKey = GetCacheKey(publishmentSystemId);
            lock (LockObject)
            {
                if (CacheUtils.Get(cacheKey) == null)
                {
                    var lists = DataProvider.SeoMetaDao.GetSeoMetaLists(publishmentSystemId);
                    CacheUtils.InsertMinutes(cacheKey, lists, 30);
                    return lists;
                }
                return CacheUtils.Get(cacheKey) as List<int>[];
            }
        }

        public static void RemoveCache(int publishmentSystemId)
        {
            var cacheKey = GetCacheKey(publishmentSystemId);
            CacheUtils.Remove(cacheKey);
        }

        private static string GetCacheKey(int publishmentSystemId)
        {
            return CacheKeyPrefix + publishmentSystemId;
        }

        private static readonly object LockObject = new object();
        private const string CacheKeyPrefix = "SiteServer.CMS.Core.SeoMeta.";
    }
}
