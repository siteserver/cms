using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Parse;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.V1
{
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route(Constants.ApiV1Prefix)]
    public partial class StlController : ControllerBase
    {
        private const string Route = "stl/{elementName}";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IParseManager _parseManager;
        private readonly IConfigRepository _configRepository;
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly ISiteRepository _siteRepository;

        public StlController(IAuthManager authManager, IPathManager pathManager, IParseManager parseManager, IConfigRepository configRepository, IAccessTokenRepository accessTokenRepository, ISiteRepository siteRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _parseManager = parseManager;
            _configRepository = configRepository;
            _accessTokenRepository = accessTokenRepository;
            _siteRepository = siteRepository;
        }

        public class GetRequest : Dictionary<string, string>
        {
            public int SiteId { get; set; }
            public string SiteDir { get; set; }
            public int ChannelId { get; set; }
            public int ContentId { get; set; }

            public void InitialParameters()
            {
                SiteId = InitialIntValue(nameof(SiteId));
                SiteDir = InitialStringValue(nameof(SiteDir));
                ChannelId = InitialIntValue(nameof(ChannelId));
                ContentId = InitialIntValue(nameof(ContentId));
            }

            private string ToCamelName(string name) => name[0..1].ToLower() + name[1..];

            private int InitialIntValue(string key) => System.Convert.ToInt32(InitialStringValue(key));

            private string InitialStringValue(string key)
            {
                if (string.IsNullOrEmpty(key)) return null;
                if (Remove(key, out string value))
                {
                    Remove(ToCamelName(key));
                }
                else
                {
                    Remove(ToCamelName(key), out value);
                }
                return value;
            }
        }

        public class GetResult
        {
            public object Value { get; set; }
        }

        public class StlRequest
        {
            private IAuthManager Auth { get; set; }

            public Site Site { get; private set; }

            public ParsePage PageInfo { get; private set; }

            public ParseContext ContextInfo { get; private set; }

            public async Task LoadAsync(IAuthManager auth, IPathManager pathManager, IConfigRepository configRepository, ISiteRepository siteRepository, GetRequest request)
            {
                //Request = new AuthenticatedRequest();
                //IsApiAuthorized = Request.IsApiAuthenticated && AccessTokenManager.IsScope(Request.ApiToken, AccessTokenManager.ScopeStl);

                Auth = auth;

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
                        var siteList = await siteRepository.GetSitesAsync();
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
                PageInfo = new ParsePage(pathManager, EditMode.Default, config, request.ChannelId, request.ContentId, Site, templateInfo,
                    new Dictionary<string, object>())
                {
                    User = await Auth.GetUserAsync()
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
