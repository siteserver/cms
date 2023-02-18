using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin
{
    public partial class SyncDatabaseController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<SubmitResult>> Submit([FromBody] SubmitRequest request)
        {
            var config = await _configRepository.GetAsync();

            if (config.DatabaseVersion == _settingsManager.Version)
            {
                if (_settingsManager.SecurityKey != request.SecurityKey)
                {
                    return this.Error("SecurityKey 输入错误！");
                }
            }

            await _databaseManager.SyncDatabaseAsync();

            return new SubmitResult
            {
                Version = _settingsManager.Version
            };
        }
    }
}
