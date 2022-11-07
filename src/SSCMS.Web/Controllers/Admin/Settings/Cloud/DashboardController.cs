using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Settings.Cloud
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class DashboardController : ControllerBase
    {
        private const string RouteActionsDisconnect = "settings/cloudDashboard/actions/disconnect";

        private readonly IAuthManager _authManager;
        private readonly ICloudManager _cloudManager;

        public DashboardController(IAuthManager authManager, ICloudManager cloudManager)
        {
            _authManager = authManager;
            _cloudManager = cloudManager;
        }
    }
}
