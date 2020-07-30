using System.Collections.Generic;
using System.Threading.Tasks;
using CacheManager.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Dto;
using SSCMS.Core.Utils;
using SSCMS.Extensions;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SitesAddController : ControllerBase
    {
        public const string Route = "settings/sitesAdd";
        private const string RouteProcess = "settings/sitesAdd/actions/process";

        private readonly ICacheManager<CacheUtils.Process> _cacheManager;
        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ICreateManager _createManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IOldPluginManager _oldPluginManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IAdministratorRepository _administratorRepository;

        public SitesAddController(ICacheManager<CacheUtils.Process> cacheManager, ISettingsManager settingsManager, IAuthManager authManager, IPathManager pathManager, ICreateManager createManager, IDatabaseManager databaseManager, IOldPluginManager oldPluginManager, ISiteRepository siteRepository, IContentRepository contentRepository, IAdministratorRepository administratorRepository)
        {
            _cacheManager = cacheManager;
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _createManager = createManager;
            _databaseManager = databaseManager;
            _oldPluginManager = oldPluginManager;
            _siteRepository = siteRepository;
            _contentRepository = contentRepository;
            _administratorRepository = administratorRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsSitesAdd))
            {
                return Unauthorized();
            }

            var caching = new CacheUtils(_cacheManager);
            var manager = new SiteTemplateManager(_pathManager, _oldPluginManager, _databaseManager, caching);
            var siteTemplates = manager.GetSiteTemplateInfoList();

            var tableNameList = await _siteRepository.GetSiteTableNamesAsync(_oldPluginManager);

            var rootExists = await _siteRepository.GetSiteByIsRootAsync() != null;

            var sites = await _siteRepository.GetCascadeChildrenAsync(0);
            sites.Insert(0, new Cascade<int>
            {
                Value = 0,
                Label = "<无上级站点>"
            });

            var siteTypes = _settingsManager.GetSiteTypes();

            return new GetResult
            {
                SiteTypes = siteTypes,
                SiteTemplates = siteTemplates,
                RootExists = rootExists,
                Sites = sites,
                TableNameList = tableNameList,
                Guid = StringUtils.GetShortGuid(false)
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<IntResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsSitesAdd))
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
            if (StringUtils.EqualsIgnoreCase(request.SiteType, AuthTypes.SiteTypes.Web) || StringUtils.EqualsIgnoreCase(request.SiteType, AuthTypes.SiteTypes.Wx))
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

            var siteId = await _siteRepository.InsertSiteAsync(_pathManager, channelInfo, new Site
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
                var manager = new SiteTemplateManager(_pathManager, _oldPluginManager, _databaseManager, caching);
                await manager.ImportSiteTemplateToEmptySiteAsync(site, request.CreateTemplateId, request.IsImportContents, request.IsImportTableStyles, adminId, request.Guid);

                caching.SetProcess(request.Guid, "生成站点页面...");
                await _createManager.CreateByAllAsync(site.Id);

                caching.SetProcess(request.Guid, "清除系统缓存...");
                _cacheManager.Clear();
            }
            else if (request.CreateType == "cloud")
            {
                caching.SetProcess(request.Guid, "开始下载模板压缩包，可能需要几分钟，请耐心等待...");

                var filePath = _pathManager.GetSiteTemplatesPath($"T_{request.CreateTemplateId}.zip");
                FileUtils.DeleteFileIfExists(filePath);
                var downloadUrl = OnlineTemplateManager.GetDownloadUrl(request.CreateTemplateId);
                WebClientUtils.Download(downloadUrl, filePath);

                caching.SetProcess(request.Guid, "模板压缩包下载成功，开始解压缩，可能需要几分钟，请耐心等待...");

                var siteTemplateDir = $"T_{request.CreateTemplateId}";
                var directoryPath = _pathManager.GetSiteTemplatesPath(siteTemplateDir);
                DirectoryUtils.DeleteDirectoryIfExists(directoryPath);
                ZipUtils.ExtractZip(filePath, directoryPath);

                caching.SetProcess(request.Guid, "模板压缩包解压成功，正在导入数据...");

                var manager = new SiteTemplateManager(_pathManager, _oldPluginManager, _databaseManager, caching);
                await manager.ImportSiteTemplateToEmptySiteAsync(site, siteTemplateDir, request.IsImportContents, request.IsImportTableStyles, adminId, request.Guid);

                caching.SetProcess(request.Guid, "生成站点页面...");
                await _createManager.CreateByAllAsync(site.Id);

                caching.SetProcess(request.Guid, "清除系统缓存...");
                _cacheManager.Clear();
            }

            return new IntResult
            {
                Value = siteId
            };
        }

        [HttpPost, Route(RouteProcess)]
        public async Task<ActionResult<CacheUtils.Process>> Process([FromBody] ProcessRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsSitesAdd))
            {
                return Unauthorized();
            }

            var caching = new CacheUtils(_cacheManager);
            return caching.GetProcess(request.Guid);
        }
    }
}
