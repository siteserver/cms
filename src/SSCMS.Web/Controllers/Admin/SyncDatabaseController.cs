using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin
{
    [OpenApiIgnore]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SyncDatabaseController : ControllerBase
    {
        public const string Route = "syncDatabase";

        private readonly ISettingsManager _settingsManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IOldPluginManager _pluginManager;
        private readonly IConfigRepository _configRepository;

        public SyncDatabaseController(ISettingsManager settingsManager, IDatabaseManager databaseManager, IOldPluginManager pluginManager, IConfigRepository configRepository)
        {
            _settingsManager = settingsManager;
            _databaseManager = databaseManager;
            _pluginManager = pluginManager;
            _configRepository = configRepository;
        }

        public class GetResult
        {
            public string DatabaseVersion { get; set; }
            public string Version { get; set; }
        }

        public class SubmitResult
        {
            public string Version { get; set; }
        }
    }
}