using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SiteServer.Abstractions;
using SiteServer.Web.Core;

namespace SiteServer.Web.Controllers.Pages
{
    [Route("pages/syncDatabase")]
    public partial class PagesSyncDatabaseController : ControllerBase
    {
        private const string Route = "";

        private readonly ISettingsManager _settingsManager;
        private readonly IConfigRepository _configRepository;
        private readonly IDatabaseRepository _databaseRepository;

        public PagesSyncDatabaseController(ISettingsManager settingsManager, IConfigRepository configRepository, IDatabaseRepository databaseRepository)
        {
            _settingsManager = settingsManager;
            _configRepository = configRepository;
            _databaseRepository = databaseRepository;
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<SubmitResult>> Submit()
        {
            //var request = await AuthenticatedRequest.GetAuthAsync();
            //if (!request.IsAdminLoggin || !request.AdminPermissions.IsSuperAdmin())
            //{
            //    return Unauthorized();
            //}

            if (await _configRepository.IsNeedInstallAsync())
            {
                return this.Error("系统未安装，向导被禁用！");
            }

            await _databaseRepository.SyncDatabaseAsync();

            return new SubmitResult
            {
                Version = _settingsManager.ProductVersion
            };
        }
    }
}