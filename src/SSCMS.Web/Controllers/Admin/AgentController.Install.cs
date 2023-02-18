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

                (success, errorMessage) = await _databaseManager.InstallAsync(request.UserName, request.Password, string.Empty, string.Empty);
                if (!success)
                {
                    return this.Error(errorMessage);
                }

                await FileUtils.WriteTextAsync(_pathManager.GetRootPath("index.html"), Constants.Html5Empty);

                // if (!string.IsNullOrEmpty(request.ThemeDownloadUrl))
                // {
                //     var caching = new CacheUtils(_cacheManager);

                //     caching.SetProcess(request.Guid, "任务初始化...");

                //     caching.SetProcess(request.Guid, "开始下载模板压缩包，可能需要几分钟，请耐心等待...");

                //     var fileName = PageUtils.GetFileNameFromUrl(request.ThemeDownloadUrl);

                //     var filePath = _pathManager.GetSiteTemplatesPath(fileName);
                //     FileUtils.DeleteFileIfExists(filePath);
                //     await HttpClientUtils.DownloadAsync(request.ThemeDownloadUrl, filePath);

                //     caching.SetProcess(request.Guid, "模板压缩包下载成功，开始解压缩，可能需要几分钟，请耐心等待...");

                //     var siteTemplateDir = PathUtils.GetFileNameWithoutExtension(filePath);
                //     var directoryPath = _pathManager.GetSiteTemplatesPath(siteTemplateDir);
                //     DirectoryUtils.DeleteDirectoryIfExists(directoryPath);
                //     _pathManager.ExtractZip(filePath, directoryPath);

                //     caching.SetProcess(request.Guid, "模板压缩包解压成功，正在导入数据...");

                //     var channelInfo = new Channel();

                //     channelInfo.ChannelName = channelInfo.IndexName = "首页";
                //     channelInfo.ParentId = 0;
                //     channelInfo.ContentModelPluginId = string.Empty;

                //     var tableName = await _contentRepository.CreateNewContentTableAsync();
                //     var siteId = 0;
                //     (siteId, errorMessage) = await _siteRepository.InsertSiteAsync(channelInfo, new Site
                //     {
                //         SiteName = siteTemplateDir,
                //         SiteType = Types.SiteTypes.Web,
                //         SiteDir = siteTemplateDir,
                //         TableName = tableName,
                //         ParentId = 0,
                //         Root = true
                //     }, 0);

                //     if (siteId == 0)
                //     {
                //         return this.Error(errorMessage);
                //     }

                //     var site = await _siteRepository.GetAsync(siteId);

                //     var manager = new SiteTemplateManager(_pathManager, _databaseManager, caching);
                //     await manager.ImportSiteTemplateToEmptySiteAsync(site, siteTemplateDir, true, true, 0, request.Guid);

                //     caching.SetProcess(request.Guid, "生成站点页面...");
                //     await _createManager.CreateByAllAsync(site.Id);

                //     caching.SetProcess(request.Guid, "清除系统缓存...");
                //     _cacheManager.Clear();
                // }
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