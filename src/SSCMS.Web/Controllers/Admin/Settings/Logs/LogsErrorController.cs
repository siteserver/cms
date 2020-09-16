using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Core.Utils;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Settings.Logs
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LogsErrorController : ControllerBase
    {
        private const string Route = "settings/logsError";

        private readonly IAuthManager _authManager;
        private readonly IPluginManager _pluginManager;
        private readonly IErrorLogRepository _errorLogRepository;

        public LogsErrorController(IAuthManager authManager, IPluginManager pluginManager, IErrorLogRepository errorLogRepository)
        {
            _authManager = authManager;
            _pluginManager = pluginManager;
            _errorLogRepository = errorLogRepository;
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<SearchResult>> List([FromBody] SearchRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsLogsError))
            {
                return Unauthorized();
            }

            var count = await _errorLogRepository.GetCountAsync(request.Category, request.PluginId, request.Keyword, request.DateFrom, request.DateTo);
            var logs = await _errorLogRepository.GetAllAsync(request.Category, request.PluginId, request.Keyword, request.DateFrom, request.DateTo, request.Offset, request.Limit);

            var categories = new List<Select<string>>();
            foreach (var category in LogUtils.AllCategoryList.Value)
            {
                categories.Add(new Select<string>(category.Key, category.Value));
            }

            var pluginIds = _pluginManager
                .EnabledPlugins
                .Select(plugin => new Select<string>(plugin.PluginId, plugin.DisplayName))
                .ToList();

            return new SearchResult
            {
                Items = logs,
                Count = count,
                Categories = categories,
                PluginIds = pluginIds
            };
        }

        [HttpDelete, Route(Route)]
        public async Task<ActionResult<BoolResult>> Delete()
        {
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsLogsError))
            {
                return Unauthorized();
            }

            await _errorLogRepository.DeleteAllAsync();

            await _authManager.AddAdminLogAsync("清空错误日志");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
