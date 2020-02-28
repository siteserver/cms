using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Core;

namespace SS.CMS.Web.Controllers.Admin.Settings.Utilities
{
    [Route("admin/settings/utilitiesParameters")]
    public class UtilitiesParametersController : ControllerBase
    {
        private const string Route = "";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IConfigRepository _configRepository;

        public UtilitiesParametersController(ISettingsManager settingsManager, IAuthManager authManager, IConfigRepository configRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _configRepository = configRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<List<KeyValuePair<string, string>>>> Get()
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUtilitiesParameters))
            {
                return Unauthorized();
            }

            var config = await _configRepository.GetAsync();

            var parameterList = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("系统主机名", Dns.GetHostName().ToUpper()),
                new KeyValuePair<string, string>("系统根目录地址", _settingsManager.ContentRootPath),
                new KeyValuePair<string, string>("站点根目录地址", _settingsManager.WebRootPath),
                new KeyValuePair<string, string>(".NET 框架", _settingsManager.TargetFramework),
                new KeyValuePair<string, string>(".NET Core 版本", Environment.Version.ToString()),
                new KeyValuePair<string, string>("SiteServer CMS 版本", _settingsManager.ProductVersion),
                new KeyValuePair<string, string>("SS.CMS.Abstractions 版本", _settingsManager.PluginVersion),
                new KeyValuePair<string, string>("最近升级时间", DateUtils.GetDateAndTimeString(config.UpdateDate)),
                new KeyValuePair<string, string>("数据库类型", _settingsManager.Database.DatabaseType.GetValue()),
                new KeyValuePair<string, string>("数据库名称", SqlUtils.GetDatabaseNameFormConnectionString(_settingsManager.Database.DatabaseType, _settingsManager.Database.ConnectionString)),
                new KeyValuePair<string, string>("缓存类型", string.IsNullOrEmpty(_settingsManager.Redis.ConnectionString) ? "Memory" : "Redis")
            };

            return parameterList;
        }
    }
}
