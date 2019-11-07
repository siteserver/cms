using System.Collections.Generic;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Db;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.CMS.StlParser.Model;
using SiteServer.Plugin;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Api.V1
{
    public class StlRequest
    {
        private AuthenticatedRequest Request { get; }

        public bool IsApiAuthorized { get; }

        public Site Site { get; }

        public PageInfo PageInfo { get; }

        public ContextInfo ContextInfo { get; }

        public StlRequest(AuthenticatedRequest request, bool isApiAuthorized)
        {
            //Request = new AuthenticatedRequest();
            //IsApiAuthorized = Request.IsApiAuthenticated && AccessTokenManager.IsScope(Request.ApiToken, AccessTokenManager.ScopeStl);

            Request = request;
            IsApiAuthorized = isApiAuthorized;

            if (!IsApiAuthorized) return;

            var siteId = Request.GetQueryInt("siteId");
            var siteDir = Request.GetQueryString("siteDir");

            var channelId = Request.GetQueryInt("channelId");
            var contentId = Request.GetQueryInt("contentId");

            if (siteId > 0)
            {
                Site = SiteManager.GetSiteAsync(siteId).GetAwaiter().GetResult();
            }
            else if (!string.IsNullOrEmpty(siteDir))
            {
                Site = SiteManager.GetSiteByDirectoryAsync(siteDir).GetAwaiter().GetResult();
            }
            else
            {
                Site = SiteManager.GetSiteByIsRootAsync().GetAwaiter().GetResult();
                if (Site == null)
                {
                    var siteList = SiteManager.GetSiteListAsync().GetAwaiter().GetResult();
                    if (siteList != null && siteList.Count > 0)
                    {
                        Site = siteList[0];
                    }
                }
            }

            if (Site == null) return;

            if (channelId == 0)
            {
                channelId = Site.Id;
            }

            var templateInfo = new TemplateInfo(0, Site.Id, string.Empty, TemplateType.IndexPageTemplate, string.Empty, string.Empty, string.Empty, ECharset.utf_8, true);

            PageInfo = new PageInfo(channelId, contentId, Site, templateInfo, new Dictionary<string, object>())
            {
                UniqueId = 1000,
                User = Request.User
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
