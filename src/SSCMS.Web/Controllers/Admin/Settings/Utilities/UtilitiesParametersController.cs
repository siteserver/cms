using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Settings.Utilities
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class UtilitiesParametersController : ControllerBase
    {
        private const string Route = "settings/utilitiesParameters";

        private readonly ISettingsManager _settingsManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IAuthManager _authManager;
        private readonly IConfigRepository _configRepository;

        public UtilitiesParametersController(ISettingsManager settingsManager, IDatabaseManager databaseManager, IAuthManager authManager, IConfigRepository configRepository)
        {
            _settingsManager = settingsManager;
            _databaseManager = databaseManager;
            _authManager = authManager;
            _configRepository = configRepository;
        }

        public class GetResult
        {
            public List<KeyValuePair<string, string>> Environments { get; set; }
            public List<KeyValuePair<string, string>> Settings { get; set; }
        }
    }
}
