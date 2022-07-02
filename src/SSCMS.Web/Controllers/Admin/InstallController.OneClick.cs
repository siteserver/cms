using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin
{
    public partial class InstallController
    {
        [HttpPost, Route(RouteOneClick)]
        public async Task<ActionResult<BoolResult>> OneClick([FromBody] OneClickRequest request)
        {
            if (!await _configRepository.IsNeedInstallAsync())
            {
                return this.Error("系统已安装");
            }

            if (string.IsNullOrEmpty(request.SecurityKey) ||
                string.IsNullOrEmpty(request.UserName) ||
                string.IsNullOrEmpty(request.Password))
            {
                return this.Error("系统安装参数不足");
            }

            if (_settingsManager.SecurityKey != request.SecurityKey)
            {
                return this.Error("SecurityKey不正确");
            }

            if (_settingsManager.DatabaseType == DatabaseType.SQLite)
            {
                var filePath = PathUtils.Combine(_settingsManager.ContentRootPath,
                    Constants.LocalDbContainerVirtualPath.Substring(1));
                if (!FileUtils.IsFileExists(filePath))
                {
                    await FileUtils.WriteTextAsync(filePath, string.Empty);
                }
            }

            var (success, errorMessage) = await _databaseManager.InstallAsync(request.UserName, request.Password, request.Email, request.Mobile);
            if (!success)
            {
                return this.Error(errorMessage);
            }

            await FileUtils.WriteTextAsync(_pathManager.GetRootPath("index.html"), Constants.Html5Empty);

            if (!string.IsNullOrEmpty(request.Themes))
            {
                var themes = ListUtils.GetStringList(request.Themes);
                foreach (var themeUrl in themes)
                {
                    var fileName = PageUtils.GetFileNameFromUrl(themeUrl);
                    var filePath = _pathManager.GetSiteTemplatesPath(fileName);
                    FileUtils.DeleteFileIfExists(filePath);
                    await HttpClientUtils.DownloadAsync(themeUrl, filePath);

                    var siteTemplateDir = PathUtils.RemoveExtension(fileName);
                    var directoryPath = _pathManager.GetSiteTemplatesPath(siteTemplateDir);
                    DirectoryUtils.DeleteDirectoryIfExists(directoryPath);
                    _pathManager.ExtractZip(filePath, directoryPath);
                }
            }

            if (!string.IsNullOrEmpty(request.Plugins))
            {
                var plugins = ListUtils.GetStringList(request.Plugins);
                foreach (var plugin in plugins)
                {
                    var arr = plugin.Split(',');
                    if (arr.Length == 3)
                    {
                        await _pluginManager.InstallAsync(arr[0], arr[1], arr[2]);
                    }
                }
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}