using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Settings.Utilities
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class UtilitiesEncryptController : ControllerBase
    {
        private const string Route = "settings/utilitiesEncrypt";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;

        public UtilitiesEncryptController(ISettingsManager settingsManager, IAuthManager authManager)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
        }

        public class SubmitRequest
        {
            public bool IsEncrypt { get; set; }
            public string Value { get; set; }
        }
    }
}
