using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Abstractions;
using SS.CMS.Framework;
using SS.CMS.StlParser.Model;

namespace SS.CMS.Web.Controllers.V1
{
    public partial class StlController
    {
        public class GetRequest : Dictionary<string, string>
        {
            public string ElementName { get; set; }
            public int SiteId { get; set; }
            public string SiteDir { get; set; }
            public int ChannelId { get; set; }
            public int ContentId { get; set; }
        }

        public class GetResult
        {
            public object Value { get; set; }
        }

        public class StlRequest
        {
            private IAuthManager Auth { get; set; }

            public bool IsApiAuthorized { get; private set; }

            public Site Site { get; private set; }

            public PageInfo PageInfo { get; private set; }

            public ContextInfo ContextInfo { get; private set; }

            public async Task LoadAsync(IAuthManager auth, bool isApiAuthorized, GetRequest request)
            {
                //Request = new AuthenticatedRequest();
                //IsApiAuthorized = Request.IsApiAuthenticated && AccessTokenManager.IsScope(Request.ApiToken, AccessTokenManager.ScopeStl);

                Auth = auth;
                IsApiAuthorized = isApiAuthorized;

                if (!IsApiAuthorized) return;

                if (request.SiteId > 0)
                {
                    Site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
                }
                else if (!string.IsNullOrEmpty(request.SiteDir))
                {
                    Site = await DataProvider.SiteRepository.GetSiteByDirectoryAsync(request.SiteDir);
                }
                else
                {
                    Site = await DataProvider.SiteRepository.GetSiteByIsRootAsync();
                    if (Site == null)
                    {
                        var siteList = await DataProvider.SiteRepository.GetSiteListAsync();
                        if (siteList != null && siteList.Count > 0)
                        {
                            Site = siteList[0];
                        }
                    }
                }

                if (Site == null) return;

                if (request.ChannelId == 0)
                {
                    request.ChannelId = Site.Id;
                }

                var templateInfo = new Template
                {
                    Id = 0,
                    SiteId = Site.Id,
                    TemplateName = string.Empty,
                    TemplateType = TemplateType.IndexPageTemplate,
                    RelatedFileName = string.Empty,
                    CreatedFileFullName = string.Empty,
                    CreatedFileExtName = string.Empty,
                    Default = true
                };

                PageInfo = await PageInfo.GetPageInfoAsync(request.ChannelId, request.ContentId, Site, templateInfo, new Dictionary<string, object>());

                PageInfo.UniqueId = 1000;
                PageInfo.User = Auth.User;

                var attributes = TranslateUtils.NewIgnoreCaseNameValueCollection();
                foreach (var key in request.Keys)
                {
                    attributes[key] = request[key];
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
}
