using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Utilities
{
    public partial class UtilitiesParametersController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUtilitiesParameters))
            {
                return Unauthorized();
            }

            var config = await _configRepository.GetAsync();

            var environments = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("运行环境", _settingsManager.Containerized ? "容器" : "主机"),
                new KeyValuePair<string, string>("SSCMS 版本", $"SSCMS {_settingsManager.Version}"),
                new KeyValuePair<string, string>(".NET Core 版本", _settingsManager.FrameworkDescription),
                new KeyValuePair<string, string>("OS 版本", _settingsManager.OSDescription),
                new KeyValuePair<string, string>("CPU Cores", _settingsManager.CPUCores.ToString()),
                new KeyValuePair<string, string>("系统主机名", Dns.GetHostName().ToUpper()),
            };

            var settings = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("系统根目录地址", _settingsManager.ContentRootPath),
                new KeyValuePair<string, string>("站点根目录地址", _settingsManager.WebRootPath),
                new KeyValuePair<string, string>("最近升级时间", DateUtils.GetDateAndTimeString(config.UpdateDate)),
                new KeyValuePair<string, string>("数据库类型", _settingsManager.Database.DatabaseType.GetValue()),
                new KeyValuePair<string, string>("数据库名称", _databaseManager.GetDatabaseNameFormConnectionString(_settingsManager.Database.ConnectionString)),
                new KeyValuePair<string, string>("缓存类型", string.IsNullOrEmpty(_settingsManager.Redis.ConnectionString) ? "Memory" : "Redis")
            };

            return new GetResult
            {
                Environments = environments,
                Settings = settings
            };
        }
    }
}
