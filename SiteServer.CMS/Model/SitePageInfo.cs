using System.Web;

using SiteServer.Utils;
using SiteServer.CMS.Core;

namespace SiteServer.CMS.Model
{
    public class SitePageInfo
    {
        private SitePageInfo()
        {
            SiteId = ChannelId = ContentId = PageIndex = 0;
        }

        public static SitePageInfo GetInstance()
        {
            var pageInfo = new SitePageInfo();

            if (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["SiteID"]))
            {
                pageInfo.SiteId = TranslateUtils.ToInt(HttpContext.Current.Request.QueryString["SiteID"]);
            }
            if (pageInfo.SiteId == 0)
            {
                pageInfo.SiteId = PathUtility.GetCurrentSiteId();
            }
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ChannelID"]))
            {
                pageInfo.ChannelId = TranslateUtils.ToInt(HttpContext.Current.Request.QueryString["ChannelID"]);
            }
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ContentID"]))
            {
                pageInfo.ContentId = TranslateUtils.ToInt(HttpContext.Current.Request.QueryString["ContentID"]);
            }

            if (pageInfo.ChannelId == 0)
            {
                var path = HttpContext.Current.Request.Path.Substring(0, HttpContext.Current.Request.Path.LastIndexOf(".aspx"));
                if (StringUtils.Contains(path, PathUtility.ChannelFilePathRules.DefaultDirectoryName))
                {
                    pageInfo.ChannelId = TranslateUtils.ToInt(RegexUtils.GetContent("channelID", PathUtility.ChannelFilePathRules.DefaultRegexString, path));
                    pageInfo.PageIndex = TranslateUtils.ToInt(RegexUtils.GetContent("pageIndex", PathUtility.ChannelFilePathRules.DefaultRegexString, path));
                    if (pageInfo.PageIndex > 0)
                    {
                        pageInfo.PageIndex--;
                    }
                }
                else if (StringUtils.Contains(path, PathUtility.ContentFilePathRules.DefaultDirectoryName))
                {
                    pageInfo.ChannelId = TranslateUtils.ToInt(RegexUtils.GetContent("channelID", PathUtility.ContentFilePathRules.DefaultRegexString, path));
                    pageInfo.ContentId = TranslateUtils.ToInt(RegexUtils.GetContent("contentID", PathUtility.ContentFilePathRules.DefaultRegexString, path));
                    pageInfo.PageIndex = TranslateUtils.ToInt(RegexUtils.GetContent("pageIndex", PathUtility.ContentFilePathRules.DefaultRegexString, path));
                    if (pageInfo.PageIndex > 0)
                    {
                        pageInfo.PageIndex--;
                    }
                }
            }
            return pageInfo;
        }

        public int SiteId { get; private set; }

        public int ChannelId { get; private set; }

        public int ContentId { get; private set; }

        public int PageIndex { get; private set; }
    }
}
