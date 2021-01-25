using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin
{
    public partial class SyncDatabaseController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (await _configRepository.IsNeedInstallAsync())
            {
                return this.Error("系统未安装，向导被禁用！");
            }

            var config = await _configRepository.GetAsync();

            if (config.DatabaseVersion == _settingsManager.Version && !await _authManager.IsSuperAdminAsync())
            {
                return Unauthorized();
            }

            return new GetResult
            {
                DatabaseVersion = config.DatabaseVersion,
                Version = _settingsManager.Version
            };
        }
    }
}
