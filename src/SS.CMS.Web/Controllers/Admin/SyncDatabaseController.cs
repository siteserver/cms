using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Extensions;

namespace SS.CMS.Web.Controllers.Admin
{
    [ApiController]
    [Route(Constants.ApiRoute)]
    public partial class SyncDatabaseController : ControllerBase
    {
        public const string Route = "syncDatabase";

        private readonly ISettingsManager _settingsManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IPluginManager _pluginManager;
        private readonly IConfigRepository _configRepository;

        public SyncDatabaseController(ISettingsManager settingsManager, IDatabaseManager databaseManager, IPluginManager pluginManager, IConfigRepository configRepository)
        {
            _settingsManager = settingsManager;
            _databaseManager = databaseManager;
            _pluginManager = pluginManager;
            _configRepository = configRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (await _configRepository.IsNeedInstallAsync())
            {
                return this.Error("系统未安装，向导被禁用！");
            }

            var config = await _configRepository.GetAsync();

            return new GetResult
            {
                DatabaseVersion = config.DatabaseVersion,
                ProductVersion = _settingsManager.ProductVersion
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<SubmitResult>> Submit()
        {
            //
            //if (!request.IsAdminLoggin || !request.AdminPermissions.IsSuperAdmin())
            //{
            //    return Unauthorized();
            //}

            await _databaseManager.SyncDatabaseAsync(_pluginManager);

            return new SubmitResult
            {
                Version = _settingsManager.ProductVersion
            };
        }
    }
}