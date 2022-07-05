using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin
{
    public partial class AgentController
    {
        [HttpPost, Route(RouteInstall)]
        public async Task<ActionResult<BoolResult>> Install([FromBody] InstallRequest request)
        {
            if (string.IsNullOrEmpty(request.SecurityKey) ||
                string.IsNullOrEmpty(request.UserName) ||
                string.IsNullOrEmpty(request.Password))
            {
                return this.Error("系统参数不足");
            }
            if (_settingsManager.SecurityKey != request.SecurityKey)
            {
                return this.Error("SecurityKey不正确");
            }
            var (success, errorMessage) = await _administratorRepository.InsertValidateAsync(request.UserName, request.Password, string.Empty, string.Empty);
            if (!success)
            {
                return this.Error(errorMessage);
            }

            if (await _configRepository.IsNeedInstallAsync())
            {
                if (_settingsManager.DatabaseType == DatabaseType.SQLite)
                {
                    var filePath = PathUtils.Combine(_settingsManager.ContentRootPath,
                        Constants.LocalDbContainerVirtualPath.Substring(1));
                    if (!FileUtils.IsFileExists(filePath))
                    {
                        await FileUtils.WriteTextAsync(filePath, string.Empty);
                    }
                }

                (success, errorMessage) = await _databaseManager.InstallAsync(request.UserName, request.Password, request.Email, request.Mobile);
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
            }
            else
            {
                var admin = await _administratorRepository.GetByUserNameAsync(request.UserName);
                if (admin != null)
                {
                    (success, errorMessage) = await _administratorRepository.ChangePasswordAsync(admin, request.Password);
                    if (!success)
                    {
                        return this.Error(errorMessage);
                    }
                    var cacheKey = Constants.GetSessionIdCacheKey(admin.Id);
                    await _dbCacheRepository.RemoveAsync(cacheKey);
                }
                else
                {
                    admin = new Administrator
                    {
                        UserName = request.UserName,
                        Email = request.Email,
                        Mobile = request.Mobile,
                    };
                    (success, errorMessage) = await _administratorRepository.InsertAsync(admin, request.Password);
                    if (!success)
                    {
                        return this.Error(errorMessage);
                    }
                    await _administratorRepository.AddUserToRoleAsync(admin.UserName, PredefinedRole.ConsoleAdministrator.GetValue());
                }
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}