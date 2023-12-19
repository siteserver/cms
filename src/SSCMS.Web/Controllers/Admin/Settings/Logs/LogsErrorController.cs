using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Models;
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
        private const string RouteExport = "settings/logsError/actions/export";
        private const string RouteDelete = "settings/logsError/actions/delete";

        private readonly IAuthManager _authManager;
        private readonly IPluginManager _pluginManager;
        private readonly IPathManager _pathManager;
        private readonly IErrorLogRepository _errorLogRepository;

        public LogsErrorController(IAuthManager authManager, IPluginManager pluginManager, IPathManager pathManager, IErrorLogRepository errorLogRepository)
        {
            _authManager = authManager;
            _pluginManager = pluginManager;
            _pathManager = pathManager;
            _errorLogRepository = errorLogRepository;
        }

        public class SearchRequest : PageRequest
        {
            public string Category { get; set; }
            public string PluginId { get; set; }
            public string Keyword { get; set; }
            public string DateFrom { get; set; }
            public string DateTo { get; set; }
        }

        public class SearchResult : PageResult<ErrorLog>
        {
            public List<Select<string>> Categories { get; set; }
            public List<Select<string>> PluginIds { get; set; }
        }

        public async Task<SearchResult> GetResultsAsync(SearchRequest request)
        {
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
