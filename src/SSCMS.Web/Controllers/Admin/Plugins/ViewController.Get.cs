using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Plugins
{
    public partial class ViewController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.PluginsManagement))
            {
                return Unauthorized();
            }

            var plugin = _pluginManager.GetPlugin(!string.IsNullOrEmpty(request.PluginId)
                ? request.PluginId
                : $"{request.UserName}.{request.Name}");

            var content = string.Empty;
            var changeLog = string.Empty;

            if (plugin != null)
            {
                var readmePath = PathUtils.Combine(plugin.ContentRootPath, Constants.ReadmeFileName);
                if (FileUtils.IsFileExists(readmePath))
                {
                    content = MarkdownUtils.ToHtml(FileUtils.ReadText(readmePath));
                }
                var changeLogPath = PathUtils.Combine(plugin.ContentRootPath, Constants.ChangeLogFileName);
                if (FileUtils.IsFileExists(changeLogPath))
                {
                    changeLog = MarkdownUtils.ToHtml(FileUtils.ReadText(changeLogPath));
                }
            }

            return new GetResult
            {
                CmsVersion = _settingsManager.Version,
                Plugin = plugin,
                Content = content,
                ChangeLog = changeLog
            };
        }
    }
}
