using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Parse;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.V1
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

            public ParsePage PageInfo { get; private set; }

            public ParseContext ContextInfo { get; private set; }

            public async Task LoadAsync(IAuthManager auth, IPathManager pathManager, IConfigRepository configRepository, ISiteRepository siteRepository, bool isApiAuthorized, GetRequest request)
            {
                //Request = new AuthenticatedRequest();
                //IsApiAuthorized = Request.IsApiAuthenticated && AccessTokenManager.IsScope(Request.ApiToken, AccessTokenManager.ScopeStl);

                Auth = auth;
                IsApiAuthorized = isApiAuthorized;

                if (!IsApiAuthorized) return;

                if (request.SiteId > 0)
                {
                    Site = await siteRepository.GetAsync(request.SiteId);
                }
                else if (!string.IsNullOrEmpty(request.SiteDir))
                {
                    Site = await siteRepository.GetSiteByDirectoryAsync(request.SiteDir);
                }
                else
                {
                    Site = await siteRepository.GetSiteByIsRootAsync();
                    if (Site == null)
                    {
                        var siteList = await siteRepository.GetSiteListAsync();
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
                    DefaultTemplate = true
                };

                var config = await configRepository.GetAsync();
                PageInfo = new ParsePage(pathManager, config, request.ChannelId, request.ContentId, Site, templateInfo,
                    new Dictionary<string, object>())
                {
                    UniqueId = 1000, User = await Auth.GetUserAsync()
                };

                var attributes = TranslateUtils.NewIgnoreCaseNameValueCollection();
                foreach (var key in request.Keys)
                {
                    attributes[key] = request[key];
                }

                ContextInfo = new ParseContext(PageInfo)
                {
                    IsStlEntity = true,
                    Attributes = attributes,
                    InnerHtml = string.Empty
                };
            }
        }
    }
}
