using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Plugins;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesAddController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<IntResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSitesAdd))
            {
                return Unauthorized();
            }

            if (!request.Root)
            {
                if (_pathManager.IsSystemDirectory(request.SiteDir))
                {
                    return this.Error("文件夹名称不能为系统文件夹名称，请更改文件夹名称！");
                }
                if (!DirectoryUtils.IsDirectoryNameCompliant(request.SiteDir))
                {
                    return this.Error("文件夹名称不符合系统要求，请更改文件夹名称！");
                }
                var sitePath = await _pathManager.GetSitePathAsync(request.ParentId);
                var directories = DirectoryUtils.GetDirectoryNames(sitePath);
                if (ListUtils.ContainsIgnoreCase(directories, request.SiteDir))
                {
                    return this.Error("已存在相同的文件夹，请更改文件夹名称！");
                }
                var list = await _siteRepository.GetSiteDirsAsync(request.ParentId);
                if (ListUtils.ContainsIgnoreCase(list, request.SiteDir))
                {
                    return this.Error("已存在相同的站点文件夹，请更改文件夹名称！");
                }
            }

            var channelInfo = new Channel();

            channelInfo.ChannelName = channelInfo.IndexName = "首页";
            channelInfo.ParentId = 0;
            channelInfo.ContentModelPluginId = string.Empty;

            var tableName = string.Empty;
            if (StringUtils.EqualsIgnoreCase(request.SiteType, Types.SiteTypes.Web) || StringUtils.EqualsIgnoreCase(request.SiteType, Types.SiteTypes.Wx))
            {
                if (request.TableRule == TableRule.Choose)
                {
                    tableName = request.TableChoose;
                }
                else if (request.TableRule == TableRule.HandWrite)
                {
                    tableName = request.TableHandWrite;

                    if (!await _settingsManager.Database.IsTableExistsAsync(tableName))
                    {
                        await _contentRepository.CreateContentTableAsync(tableName, _contentRepository.GetTableColumns(tableName));
                    }
                    else
                    {
                        await _settingsManager.Database.AlterTableAsync(tableName, _contentRepository.GetTableColumns(tableName));
                    }
                }
            }

            var adminId = _authManager.AdminId;

            var siteId = await _siteRepository.InsertSiteAsync(channelInfo, new Site
            {
                SiteName = request.SiteName,
                SiteType = request.SiteType,
                SiteDir = request.SiteDir,
                TableName = tableName,
                ParentId = request.ParentId,
                Root = request.Root
            }, adminId);

            if (string.IsNullOrEmpty(tableName))
            {
                tableName = await _contentRepository.CreateNewContentTableAsync();
                await _siteRepository.UpdateTableNameAsync(siteId, tableName);
            }

            if (await _authManager.IsSiteAdminAsync() && !await _authManager.IsSuperAdminAsync())
            {
                var siteIdList = await _authManager.GetSiteIdsAsync() ?? new List<int>();
                siteIdList.Add(siteId);
                var adminInfo = await _administratorRepository.GetByUserIdAsync(adminId);
                await _administratorRepository.UpdateSiteIdsAsync(adminInfo, siteIdList);
            }

            var caching = new CacheUtils(_cacheManager);
            var site = await _siteRepository.GetAsync(siteId);

            caching.SetProcess(request.Guid, "任务初始化...");

            if (request.CreateType == "local")
            {
                var manager = new SiteTemplateManager(_pathManager, _databaseManager, caching);
                await manager.ImportSiteTemplateToEmptySiteAsync(site, request.LocalDirectoryName, request.IsImportContents, request.IsImportTableStyles, adminId, request.Guid);

                caching.SetProcess(request.Guid, "生成站点页面...");
                await _createManager.CreateByAllAsync(site.Id);

                caching.SetProcess(request.Guid, "清除系统缓存...");
                _cacheManager.Clear();
            }
            else if (request.CreateType == "cloud")
            {
                caching.SetProcess(request.Guid, "开始下载模板压缩包，可能需要几分钟，请耐心等待...");

                var filePath = _pathManager.GetSiteTemplatesPath($"T_{request.CloudThemeName}.zip");
                FileUtils.DeleteFileIfExists(filePath);
                var downloadUrl = CloudUtils.Dl.GetThemesDownloadUrl(request.CloudThemeUserName, request.CloudThemeName);
                WebClientUtils.Download(downloadUrl, filePath);

                caching.SetProcess(request.Guid, "模板压缩包下载成功，开始解压缩，可能需要几分钟，请耐心等待...");

                var siteTemplateDir = $"T_{request.CloudThemeName}";
                var directoryPath = _pathManager.GetSiteTemplatesPath(siteTemplateDir);
                DirectoryUtils.DeleteDirectoryIfExists(directoryPath);
                _pathManager.ExtractZip(filePath, directoryPath);

                caching.SetProcess(request.Guid, "模板压缩包解压成功，正在导入数据...");

                var manager = new SiteTemplateManager(_pathManager, _databaseManager, caching);
                await manager.ImportSiteTemplateToEmptySiteAsync(site, siteTemplateDir, request.IsImportContents, request.IsImportTableStyles, adminId, request.Guid);

                caching.SetProcess(request.Guid, "生成站点页面...");
                await _createManager.CreateByAllAsync(site.Id);

                caching.SetProcess(request.Guid, "清除系统缓存...");
                _cacheManager.Clear();
            }
            else
            {
                var templates = await _templateRepository.GetSummariesAsync(site.Id);
                foreach (var summary in templates)
                {
                    var template = await _templateRepository.GetAsync(summary.Id);
                    await _pathManager.WriteContentToTemplateFileAsync(site, template, Constants.Html5Empty, _authManager.AdminId);
                }

                await _createManager.CreateByAllAsync(site.Id);
            }

            return new IntResult
            {
                Value = siteId
            };
        }
    }
}