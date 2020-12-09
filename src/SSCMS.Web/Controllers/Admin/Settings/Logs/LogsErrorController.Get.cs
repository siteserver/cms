using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Settings.Logs
{
    public partial class LogsErrorController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<SearchResult>> Get([FromBody] SearchRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsLogsError))
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
    }
}
