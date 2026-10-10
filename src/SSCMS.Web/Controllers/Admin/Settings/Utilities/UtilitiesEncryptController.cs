using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Settings.Utilities
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class UtilitiesEncryptController : ControllerBase
    {
        private const string Route = "settings/utilitiesEncrypt";
        private const string RouteRotateKey = "settings/utilitiesEncrypt/actions/rotateKey";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        public UtilitiesEncryptController(ISettingsManager settingsManager, IAuthManager authManager,
            IAdministratorRepository administratorRepository, IAccessTokenRepository accessTokenRepository,
            IHostApplicationLifetime hostApplicationLifetime)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _administratorRepository = administratorRepository;
            _accessTokenRepository = accessTokenRepository;
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        public class SubmitRequest
        {
            public bool IsEncrypt { get; set; }
            public string Value { get; set; }
        }

        public class RotateKeyRequest
        {
            public string Password { get; set; }
        }
    }
}
