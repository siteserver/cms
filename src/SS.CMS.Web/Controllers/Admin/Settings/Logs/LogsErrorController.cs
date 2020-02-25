using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Core;
using SS.CMS.Framework;
using SS.CMS.Plugins;

namespace SS.CMS.Web.Controllers.Admin.Settings.Logs
{
    [Route("admin/settings/logsError")]
    public partial class LogsErrorController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;

        public LogsErrorController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<SearchResult>> List([FromBody] SearchRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsLogsError))
            {
                return Unauthorized();
            }

            var count = await DataProvider.ErrorLogRepository.GetCountAsync(request.Category, request.PluginId, request.Keyword, request.DateFrom, request.DateTo);
            var logs = await DataProvider.ErrorLogRepository.GetAllAsync(request.Category, request.PluginId, request.Keyword, request.DateFrom, request.DateTo, request.Offset, request.Limit);

            var categories = new List<Select<string>>();
            foreach (var category in LogUtils.AllCategoryList.Value)
            {
                categories.Add(new Select<string>(category.Key, category.Value));
            }

            var pluginIds = new List<Select<string>>();
            foreach (var pluginInfo in await PluginManager.GetAllPluginInfoListAsync())
            {
                pluginIds.Add(new Select<string>(pluginInfo.Id, pluginInfo.Metadata.Title));
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
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsLogsError))
            {
                return Unauthorized();
            }

            await DataProvider.ErrorLogRepository.DeleteAllAsync();

            await auth.AddAdminLogAsync("清空错误日志");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
