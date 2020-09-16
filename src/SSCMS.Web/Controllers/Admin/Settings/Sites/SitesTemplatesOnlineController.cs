using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SitesTemplatesOnlineController : ControllerBase
    {
        private const string Route = "settings/sitesTemplatesOnline";

        private readonly IAuthManager _authManager;

        public SitesTemplatesOnlineController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        public class GetResult
        {
            public bool SiteAddPermission { get; set; }
        }
    }
}
