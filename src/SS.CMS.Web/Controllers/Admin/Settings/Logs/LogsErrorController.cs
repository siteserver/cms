using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Core;

namespace SS.CMS.Web.Controllers.Admin.Settings.Logs
{
    [Route("admin/settings/logsError")]
    public partial class LogsErrorController : ControllerBase
    {
        private const string Route = "";

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
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsLogsError))
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

            var pluginIds = new List<Select<string>>();
            foreach (var plugin in _pluginManager.GetPlugins())
            {
                pluginIds.Add(new Select<string>(plugin.PluginId, plugin.Name));
            }

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
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsLogsError))
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
