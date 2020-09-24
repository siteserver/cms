using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class AdministratorsAccessTokensLayerViewController : ControllerBase
    {
        private const string Route = "settings/administratorsAccessTokensLayerView";
        private const string RouteRegenerate = "settings/administratorsAccessTokensLayerView/actions/regenerate";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IAccessTokenRepository _accessTokenRepository;

        public AdministratorsAccessTokensLayerViewController(ISettingsManager settingsManager, IAuthManager authManager, IAccessTokenRepository accessTokenRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _accessTokenRepository = accessTokenRepository;
        }

        public class GetResult
        {
            public AccessToken Token { get; set; }
            public string AccessToken { get; set; }
        }

        public class RegenerateResult
        {
            public string AccessToken { get; set; }
        }
    }
}
