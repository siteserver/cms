using System.Collections.Generic;
using System.Net.Http;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Database.Models;
using SiteServer.CMS.StlParser.Model;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Core.RestRoutes.V1
{
    public class StlRequest
    {
        private HttpRequestMessage Request { get; }

        public bool IsApiAuthorized { get; }

        public SiteInfo SiteInfo { get; }

        public PageInfo PageInfo { get; }

        public ContextInfo ContextInfo { get; }
        
        public StlRequest(HttpRequestMessage request)
        {
            Request = request;

            var rest = request.GetAuthenticatedRequest();

            IsApiAuthorized = AccessTokenManager.IsScope(request.GetApiToken(), AccessTokenManager.ScopeStl);

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

            var userInfo = UserManager.GetUserInfoByUserId(rest.UserId);

            PageInfo = new PageInfo(channelId, contentId, SiteInfo, templateInfo, new Dictionary<string, object>())
            {
                UniqueId = 1000,
                UserInfo = userInfo
            };

            var attributes = TranslateUtils.NewIgnoreCaseNameValueCollection();
            //foreach (var key in Request.QueryDict.Keys)
            //{
            //    attributes[key] = Request.QueryDict[key];
            //}
            var dict = request.GetQueryDirectory();
            if (dict != null && dict.Count > 0)
            {
                foreach (var key in dict.Keys)
                {
                    attributes[key] = dict[key];
                }
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
