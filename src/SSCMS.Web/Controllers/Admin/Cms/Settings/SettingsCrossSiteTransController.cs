using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SettingsCrossSiteTransController : ControllerBase
    {
        private const string Route = "cms/settings/settingsCrossSiteTrans";

        private readonly IAuthManager _authManager;
        private readonly ISiteRepository _siteRepository;

        public SettingsCrossSiteTransController(IAuthManager authManager, ISiteRepository siteRepository)
        {
            _authManager = authManager;
            _siteRepository = siteRepository;
        }

        public class SubmitRequest : SiteRequest
        {
            public bool IsCrossSiteTransChecked { get; set; }
        }
    }
}
