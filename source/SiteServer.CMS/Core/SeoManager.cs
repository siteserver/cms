using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using BaiRong.Core;
using BaiRong.Core.Model.Attributes;
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

        private const string siteMapGoogleHead = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<urlset
      xmlns=""http://www.sitemaps.org/schemas/sitemap/0.9""
      xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
      xsi:schemaLocation=""
            http://www.sitemaps.org/schemas/sitemap/0.9
            http://www.sitemaps.org/schemas/sitemap/09/sitemap.xsd"">";

        private const string siteMapGoogleFoot = @"
</urlset>";

        private const string siteMapGoogleUrlFotmat = @"
	<url>
		<loc><![CDATA[{0}]]></loc>
		<priority>{1}</priority>
		<changefreq>{2}</changefreq>
	</url>
";

        private const string siteMapGoogleUrlWithLastModifiedFotmat = @"
	<url>
		<loc><![CDATA[{0}]]></loc>
		<priority>{1}</priority>
		<changefreq>{2}</changefreq>
		<lastmod>{3}</lastmod>
	</url>
";

        public static void CreateSiteMapGoogle(PublishmentSystemInfo publishmentSystemInfo)
        {
            var totalNum = DataProvider.NodeDao.GetContentNumByPublishmentSystemId(publishmentSystemInfo.PublishmentSystemId);

            if (totalNum == 0 || totalNum <= publishmentSystemInfo.Additional.SiteMapGooglePageCount)
            {
                var siteMapBuilder = new StringBuilder();
                siteMapBuilder.Append(siteMapGoogleHead);

                var urlFormat = publishmentSystemInfo.Additional.SiteMapGoogleIsShowLastModified ? siteMapGoogleUrlWithLastModifiedFotmat : siteMapGoogleUrlFotmat;
                var lastmode = DateUtils.GetDateString(DateTime.Now);
                var urlArrayList = new ArrayList();
                //首页
                var publishmentSystemUrl = PageUtils.AddProtocolToUrl(publishmentSystemInfo.PublishmentSystemUrl.ToLower());
                siteMapBuilder.AppendFormat(urlFormat, publishmentSystemUrl, "1.0", publishmentSystemInfo.Additional.SiteMapGoogleChangeFrequency, lastmode);

                //栏目页
                var nodeIdList = DataProvider.NodeDao.GetNodeIdListByPublishmentSystemId(publishmentSystemInfo.PublishmentSystemId);
                if (nodeIdList != null && nodeIdList.Count > 0)
                {
                    foreach (int nodeID in nodeIdList)
                    {
                        var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeID);
                        var channelUrl = PageUtils.AddProtocolToUrl(PageUtility.GetChannelUrl(publishmentSystemInfo, nodeInfo));
                        if (!string.IsNullOrEmpty(channelUrl))
                        {
                            if (urlArrayList.Contains(channelUrl.ToLower()))
                            {
                                continue;
                            }
                            else
                            {
                                urlArrayList.Add(channelUrl.ToLower());
                            }
                            if (channelUrl.ToLower().StartsWith(publishmentSystemUrl))
                            {
                                siteMapBuilder.AppendFormat(urlFormat, channelUrl, "0.8", publishmentSystemInfo.Additional.SiteMapGoogleChangeFrequency, lastmode);
                            }
                        }
                    }
                }

                if (nodeIdList != null && nodeIdList.Count > 0)
                {
                    foreach (int nodeID in nodeIdList)
                    {
                        var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeID);
                        var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, nodeInfo);
                        var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);
                        var contentIdList = DataProvider.ContentDao.GetContentIdListChecked(tableName, nodeID, string.Empty);

                        //内容页
                        if (contentIdList != null && contentIdList.Count > 0)
                        {
                            foreach (var contentId in contentIdList)
                            {
                                var contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contentId);
                                var contentUrl = PageUtils.AddProtocolToUrl(PageUtility.GetContentUrl(publishmentSystemInfo, contentInfo));
                                if (!string.IsNullOrEmpty(contentUrl))
                                {
                                    if (urlArrayList.Contains(contentUrl.ToLower()))
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        urlArrayList.Add(contentUrl.ToLower());
                                    }
                                    if (contentUrl.ToLower().StartsWith(publishmentSystemUrl))
                                    {
                                        siteMapBuilder.AppendFormat(urlFormat, contentUrl, "0.5", publishmentSystemInfo.Additional.SiteMapGoogleChangeFrequency, lastmode);
                                    }
                                }
                            }
                        }
                    }
                }

                siteMapBuilder.Append(siteMapGoogleFoot);

                var siteMapPath = PathUtility.MapPath(publishmentSystemInfo, publishmentSystemInfo.Additional.SiteMapGooglePath);
                FileUtils.WriteText(siteMapPath, ECharset.utf_8, siteMapBuilder.ToString());
            }
            else
            {
                var nodeIdList = DataProvider.NodeDao.GetNodeIdListByPublishmentSystemId(publishmentSystemInfo.PublishmentSystemId);
                var nodeIDWithContentIDArrayList = new ArrayList();

                if (nodeIdList != null && nodeIdList.Count > 0)
                {
                    foreach (int nodeID in nodeIdList)
                    {
                        var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeID);
                        var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);
                        var idList = DataProvider.ContentDao.GetContentIdListChecked(tableName, nodeID, string.Empty);
                        foreach (var contentId in idList)
                        {
                            nodeIDWithContentIDArrayList.Add($"{nodeID}_{contentId}");
                        }
                    }
                }

                var deci = (double)nodeIDWithContentIDArrayList.Count / publishmentSystemInfo.Additional.SiteMapGooglePageCount;
                var count = Convert.ToInt32(Math.Ceiling(deci));
                var siteMapIndexBuilder = new StringBuilder();

                var siteMapGooglePath = publishmentSystemInfo.Additional.SiteMapGooglePath.ToLower();
                var ext = PageUtils.GetExtensionFromUrl(siteMapGooglePath);

                var urlFormat = publishmentSystemInfo.Additional.SiteMapGoogleIsShowLastModified ? siteMapGoogleUrlWithLastModifiedFotmat : siteMapGoogleUrlFotmat;
                var lastmode = DateUtils.GetDateString(DateTime.Now);
                var publishmentSystemUrl = PageUtils.AddProtocolToUrl(publishmentSystemInfo.PublishmentSystemUrl.ToLower());

                for (var i = 1; i <= count; i++)
                {
                    var virtualPath = StringUtils.InsertBefore(ext, siteMapGooglePath, i.ToString());

                    siteMapIndexBuilder.Append($@"
  <sitemap>
    <loc>{PageUtility.ParseNavigationUrl(publishmentSystemInfo, virtualPath)}</loc>
    <lastmod>{DateUtils.GetDateString(DateTime.Now)}</lastmod>
  </sitemap>
");

                    var siteMapBuilder = new StringBuilder();
                    siteMapBuilder.Append(siteMapGoogleHead);
                    var urlArrayList = new ArrayList();

                    if (i == 1)
                    {
                        //首页
                        siteMapBuilder.AppendFormat(urlFormat, publishmentSystemUrl, "1.0", publishmentSystemInfo.Additional.SiteMapGoogleChangeFrequency, lastmode);

                        //栏目页
                        if (nodeIdList != null && nodeIdList.Count > 0)
                        {
                            foreach (int nodeID in nodeIdList)
                            {
                                var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeID);
                                var channelUrl = PageUtils.AddProtocolToUrl(PageUtility.GetChannelUrl(publishmentSystemInfo, nodeInfo));
                                if (!string.IsNullOrEmpty(channelUrl))
                                {
                                    if (urlArrayList.Contains(channelUrl.ToLower()))
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        urlArrayList.Add(channelUrl.ToLower());
                                    }
                                    if (channelUrl.ToLower().StartsWith(publishmentSystemUrl))
                                    {
                                        siteMapBuilder.AppendFormat(urlFormat, channelUrl, "0.8", publishmentSystemInfo.Additional.SiteMapGoogleChangeFrequency, lastmode);
                                    }
                                }
                            }
                        }
                    }

                    var pageCount = publishmentSystemInfo.Additional.SiteMapGooglePageCount;
                    if (i == count)
                    {
                        pageCount = nodeIDWithContentIDArrayList.Count - (count - 1) * publishmentSystemInfo.Additional.SiteMapGooglePageCount;
                    }
                    var pageNodeIDWithContentIDArrayList = nodeIDWithContentIDArrayList.GetRange((i - 1) * publishmentSystemInfo.Additional.SiteMapGooglePageCount, pageCount);

                    //内容页
                    foreach (string nodeIDWithContentID in pageNodeIDWithContentIDArrayList)
                    {
                        var nodeID = TranslateUtils.ToInt(nodeIDWithContentID.Split('_')[0]);
                        var contentID = TranslateUtils.ToInt(nodeIDWithContentID.Split('_')[1]);

                        var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeID);
                        var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, nodeInfo);
                        var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);
                        var contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contentID);

                        var contentUrl = PageUtils.AddProtocolToUrl(PageUtility.GetContentUrl(publishmentSystemInfo, contentInfo));
                        if (!string.IsNullOrEmpty(contentUrl))
                        {
                            if (urlArrayList.Contains(contentUrl.ToLower()))
                            {
                                continue;
                            }
                            else
                            {
                                urlArrayList.Add(contentUrl.ToLower());
                            }
                            if (contentUrl.ToLower().StartsWith(publishmentSystemUrl))
                            {
                                siteMapBuilder.AppendFormat(urlFormat, contentUrl, "0.5", publishmentSystemInfo.Additional.SiteMapGoogleChangeFrequency, lastmode);
                            }
                        }
                    }

                    siteMapBuilder.Append(siteMapGoogleFoot);

                    var siteMapPagePath = PathUtility.MapPath(publishmentSystemInfo, virtualPath);
                    FileUtils.WriteText(siteMapPagePath, ECharset.utf_8, siteMapBuilder.ToString());
                }

                string sitemapIndexString = $@"
<?xml version=""1.0"" encoding=""UTF-8""?>
<sitemapindex xmlns=""http://www.sitemaps.org/schemas/sitemap/0.9"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/siteindex.xsd"">
{siteMapIndexBuilder.ToString()}
</sitemapindex>
";

                var siteMapPath = PathUtility.MapPath(publishmentSystemInfo, publishmentSystemInfo.Additional.SiteMapGooglePath);
                FileUtils.WriteText(siteMapPath, ECharset.utf_8, sitemapIndexString);
            }
        }

        public static void CreateSiteMapBaidu(PublishmentSystemInfo publishmentSystemInfo)
        {
            var publishmentSystemUrl = PageUtils.AddProtocolToUrl(publishmentSystemInfo.PublishmentSystemUrl.ToLower());

            var siteMapBuilder = new StringBuilder();
            siteMapBuilder.Append($@"<?xml version=""1.0"" encoding=""GB2312"" ?>
<document>
<webSite>{publishmentSystemUrl}</webSite>
<webMaster>{publishmentSystemInfo.Additional.SiteMapBaiduWebMaster}</webMaster>
<updatePeri>{publishmentSystemInfo.Additional.SiteMapBaiduUpdatePeri}</updatePeri>
");

            var urlArrayList = new ArrayList();

            //内容页
            var nodeIdList = DataProvider.NodeDao.GetNodeIdListByPublishmentSystemId(publishmentSystemInfo.PublishmentSystemId);

            if (nodeIdList != null && nodeIdList.Count > 0)
            {
                foreach (int nodeID in nodeIdList)
                {
                    var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeID);
                    var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, nodeInfo);
                    var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);
                    var contentIdList = DataProvider.ContentDao.GetContentIdListChecked(tableName, nodeID, string.Empty);

                    if (contentIdList != null && contentIdList.Count > 0)
                    {
                        foreach (var contentId in contentIdList)
                        {
                            var contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contentId);
                            var contentUrl = PageUtils.AddProtocolToUrl(PageUtility.GetContentUrl(publishmentSystemInfo, contentInfo));
                            if (!string.IsNullOrEmpty(contentUrl))
                            {
                                if (urlArrayList.Contains(contentUrl.ToLower()))
                                {
                                    continue;
                                }
                                else
                                {
                                    urlArrayList.Add(contentUrl.ToLower());
                                }
                                if (contentUrl.ToLower().StartsWith(publishmentSystemUrl))
                                {
                                    siteMapBuilder.Append($@"
<item>
    <link><![CDATA[{contentUrl}]]></link>
    <title><![CDATA[{contentInfo.Title}]]></title>
    <text><![CDATA[{StringUtils.StripTags(contentInfo.GetExtendedAttribute(BackgroundContentAttribute.Content))}]]></text>
    <image><![CDATA[{PageUtility.ParseNavigationUrl(publishmentSystemInfo, contentInfo.GetExtendedAttribute(BackgroundContentAttribute.ImageUrl))}]]></image>
    <category><![CDATA[{NodeManager.GetNodeName(publishmentSystemInfo.PublishmentSystemId, contentInfo.NodeId)}]]></category>
    <pubDate>{DateUtils.GetDateAndTimeString(contentInfo.AddDate)}</pubDate>
</item>
");
                                }
                            }
                        }
                    }
                }
            }

            siteMapBuilder.Append(@"
</document>");

            var siteMapPath = PathUtility.MapPath(publishmentSystemInfo, publishmentSystemInfo.Additional.SiteMapBaiduPath);
            FileUtils.WriteText(siteMapPath, ECharset.gb2312, siteMapBuilder.ToString());
        }

        public static List<int>[] GetSeoMetaArrayLists(int publishmentSystemID)
        {
            var cacheKey = GetCacheKey(publishmentSystemID);
            lock (lockObject)
            {
                if (CacheUtils.Get(cacheKey) == null)
                {
                    var lists = DataProvider.SeoMetaDao.GetSeoMetaLists(publishmentSystemID);
                    CacheUtils.Insert(cacheKey, lists, 30);
                    return lists;
                }
                return CacheUtils.Get(cacheKey) as List<int>[];
            }
        }

        public static void RemoveCache(int publishmentSystemID)
        {
            var cacheKey = GetCacheKey(publishmentSystemID);
            CacheUtils.Remove(cacheKey);
        }

        private static string GetCacheKey(int publishmentSystemID)
        {
            return cacheKeyPrefix + publishmentSystemID;
        }

        private static readonly object lockObject = new object();
        private const string cacheKeyPrefix = "SiteServer.CMS.Core.SeoMeta.";
    }
}
