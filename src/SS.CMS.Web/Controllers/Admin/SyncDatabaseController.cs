using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Web.Extensions;

namespace SS.CMS.Web.Controllers.Admin
{
    [ApiController]
    [Route(Constants.ApiRoute)]
    public partial class SyncDatabaseController : ControllerBase
    {
        public const string Route = "syncDatabase";

        private readonly ISettingsManager _settingsManager;
        private readonly IConfigRepository _configRepository;
        private readonly IDatabaseManager _databaseRepository;

        public SyncDatabaseController(ISettingsManager settingsManager, IConfigRepository configRepository, IDatabaseManager databaseRepository)
        {
            _settingsManager = settingsManager;
            _configRepository = configRepository;
            _databaseRepository = databaseRepository;
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
            //var auth = await _authManager.GetAdminAsync();
            //if (!request.IsAdminLoggin || !request.AdminPermissions.IsSuperAdmin())
            //{
            //    return Unauthorized();
            //}

            await _databaseRepository.SyncDatabaseAsync();

            return new SubmitResult
            {
                Version = _settingsManager.ProductVersion
            };
        }
    }
}