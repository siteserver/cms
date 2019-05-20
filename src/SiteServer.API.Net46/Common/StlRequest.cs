using System.Collections.Generic;
using System.Web;
using SiteServer.BackgroundPages.Common;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.Model;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.API.Common
{
    public class StlRequest
    {
        private Request Request { get; }

        public bool IsApiAuthorized { get; }

        public SiteInfo SiteInfo { get; }

        public PageInfo PageInfo { get; }

        public ContextInfo ContextInfo { get; }

        public StlRequest()
        {
            Request = Request.Current;
            IsApiAuthorized = Request.IsApiAuthenticated && AccessTokenManager.IsScope(Request.ApiToken, AccessTokenManager.ScopeStl);

            if (!IsApiAuthorized) return;

            var siteId = Request.GetQueryInt("siteId");
            var siteDir = Request.GetQueryString("siteDir");

            var channelId = Request.GetQueryInt("channelId");
            var contentId = Request.GetQueryInt("contentId");

            if (siteId > 0)
            {
                SiteInfo = SiteManager.GetSiteInfo(siteId);
            }
            else if (!string.IsNullOrEmpty(siteDir))
            {
                SiteInfo = SiteManager.GetSiteInfoByDirectory(siteDir);
            }
            else
            {
                SiteInfo = SiteManager.GetSiteInfoByIsRoot();
                if (SiteInfo == null)
                {
                    var siteInfoList = SiteManager.GetSiteInfoList();
                    if (siteInfoList != null && siteInfoList.Count > 0)
                    {
                        SiteInfo = siteInfoList[0];
                    }
                }
            }

            if (SiteInfo == null) return;

            if (channelId == 0)
            {
                channelId = SiteInfo.Id;
            }

            var templateInfo = new TemplateInfo
            {
                SiteId = SiteInfo.Id,
                TemplateName = string.Empty,
                Type = TemplateType.IndexPageTemplate,
                RelatedFileName = string.Empty,
                CreatedFileFullName = string.Empty,
                CreatedFileExtName = string.Empty,
                Default = true
            };

            PageInfo = new PageInfo(channelId, contentId, SiteInfo, templateInfo, new Dictionary<string, object>())
            {
                UniqueId = 1000,
                UserInfo = Request.UserInfo
            };

            var attributes = TranslateUtils.NewIgnoreCaseNameValueCollection();
            var queryString = HttpContext.Current.Request.QueryString;
            foreach (var key in queryString.AllKeys)
            {
                attributes[key] = queryString[key];
            }

            ContextInfo = new ContextInfo(PageInfo)
            {
                IsStlEntity = true,
                Attributes = attributes,
                InnerHtml = string.Empty
            };
        }
    }
}
