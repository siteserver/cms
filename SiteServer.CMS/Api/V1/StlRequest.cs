using System.Collections.Generic;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.CMS.StlParser.Model;
using SiteServer.Plugin;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Api.V1
{
    public class StlRequest
    {
        private RequestImpl Request { get; }

        public bool IsApiAuthorized { get; }

        public SiteInfo SiteInfo { get; }

        public PageInfo PageInfo { get; }

        public ContextInfo ContextInfo { get; }

        public StlRequest()
        {
            Request = new RequestImpl();
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

            var templateInfo = new TemplateInfo(0, SiteInfo.Id, string.Empty, TemplateType.IndexPageTemplate, string.Empty, string.Empty, string.Empty, ECharset.utf_8, true);

            PageInfo = new PageInfo(channelId, contentId, SiteInfo, templateInfo, new Dictionary<string, object>())
            {
                UniqueId = 1000,
                UserInfo = Request.UserInfo
            };

            var attributes = TranslateUtils.NewIgnoreCaseNameValueCollection();
            foreach (var key in Request.QueryString.AllKeys)
            {
                attributes[key] = Request.QueryString[key];
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
