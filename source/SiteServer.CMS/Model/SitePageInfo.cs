using System.Web;

using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.CMS.Model
{
    public class SitePageInfo
    {
        private int siteID;
        private int channelID;
        private int contentID;
        private int pageIndex;

        private SitePageInfo()
        {
            siteID = channelID = contentID = pageIndex = 0;
        }

        public static SitePageInfo GetInstance()
        {
            var pageInfo = new SitePageInfo();

            if (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["SiteID"]))
            {
                pageInfo.siteID = TranslateUtils.ToInt(HttpContext.Current.Request.QueryString["SiteID"]);
            }
            if (pageInfo.siteID == 0)
            {
                pageInfo.siteID = PathUtility.GetCurrentPublishmentSystemId();
            }
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ChannelID"]))
            {
                pageInfo.channelID = TranslateUtils.ToInt(HttpContext.Current.Request.QueryString["ChannelID"]);
            }
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ContentID"]))
            {
                pageInfo.contentID = TranslateUtils.ToInt(HttpContext.Current.Request.QueryString["ContentID"]);
            }

            if (pageInfo.channelID == 0)
            {
                var path = HttpContext.Current.Request.Path.Substring(0, HttpContext.Current.Request.Path.LastIndexOf(".aspx"));
                if (StringUtils.Contains(path, PathUtility.ChannelFilePathRules.DefaultDirectoryName))
                {
                    pageInfo.channelID = TranslateUtils.ToInt(RegexUtils.GetContent("channelID", PathUtility.ChannelFilePathRules.DefaultRegexString, path));
                    pageInfo.pageIndex = TranslateUtils.ToInt(RegexUtils.GetContent("pageIndex", PathUtility.ChannelFilePathRules.DefaultRegexString, path));
                    if (pageInfo.pageIndex > 0)
                    {
                        pageInfo.pageIndex--;
                    }
                }
                else if (StringUtils.Contains(path, PathUtility.ContentFilePathRules.DefaultDirectoryName))
                {
                    pageInfo.channelID = TranslateUtils.ToInt(RegexUtils.GetContent("channelID", PathUtility.ContentFilePathRules.DefaultRegexString, path));
                    pageInfo.contentID = TranslateUtils.ToInt(RegexUtils.GetContent("contentID", PathUtility.ContentFilePathRules.DefaultRegexString, path));
                    pageInfo.pageIndex = TranslateUtils.ToInt(RegexUtils.GetContent("pageIndex", PathUtility.ContentFilePathRules.DefaultRegexString, path));
                    if (pageInfo.pageIndex > 0)
                    {
                        pageInfo.pageIndex--;
                    }
                }
            }
            return pageInfo;
        }

        public int SiteID => siteID;

        public int ChannelID => channelID;

        public int ContentID => contentID;

        public int PageIndex => pageIndex;
    }
}
