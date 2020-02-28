using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Core;
using SS.CMS.Repositories;
using SS.CMS.Web.Extensions;

namespace SS.CMS.Web.Controllers.Admin.Settings.Sites
{
    [Route(Constants.ApiRoute)]
    public partial class SitesAddController : ControllerBase
    {
        public const string Route = "settings/sitesAdd";
        private const string RouteProcess = "settings/sitesAdd/actions/process";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ICreateManager _createManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IAdministratorRepository _administratorRepository;

        public SitesAddController(ISettingsManager settingsManager, IAuthManager authManager, IPathManager pathManager, ICreateManager createManager, IDatabaseManager databaseManager, ISiteRepository siteRepository, IContentRepository contentRepository, IAdministratorRepository administratorRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _createManager = createManager;
            _databaseManager = databaseManager;
            _siteRepository = siteRepository;
            _contentRepository = contentRepository;
            _administratorRepository = administratorRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSitesAdd))
            {
                return Unauthorized();
            }

            var manager = new SiteTemplateManager(_pathManager, _databaseManager);
            var siteTemplates = manager.GetSiteTemplateInfoList();

            var tableNameList = await _siteRepository.GetSiteTableNamesAsync();

            var rootExists = await _siteRepository.GetSiteByIsRootAsync() != null;

            var sites = await _siteRepository.GetCascadeChildrenAsync(0);
            sites.Insert(0, new Cascade<int>
            {
                Value = 0,
                Label = "<无上级站点>"
            });

            return new GetResult
            {
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
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSitesAdd))
            {
                return Unauthorized();
            }

            if (!request.Root)
            {
                if (WebUtils.IsSystemDirectory(request.SiteDir))
                {
                    return this.Error("文件夹名称不能为系统文件夹名称，请更改文件夹名称！");
                }
                if (!DirectoryUtils.IsDirectoryNameCompliant(request.SiteDir))
                {
                    return this.Error("文件夹名称不符合系统要求，请更改文件夹名称！");
                }
                var list = await _siteRepository.GetLowerSiteDirListAsync(request.ParentId);
                if (list.Contains(request.SiteDir.ToLower()))
                {
                    return this.Error("已存在相同的发布路径，请更改文件夹名称！");
                }
            }

            var channelInfo = new Channel();

            channelInfo.ChannelName = channelInfo.IndexName = "首页";
            channelInfo.ParentId = 0;
            channelInfo.ContentModelPluginId = string.Empty;

            var tableName = string.Empty;
            if (request.TableRule == TableRule.Choose)
            {
                tableName = request.TableChoose;
            }
            else if (request.TableRule == TableRule.HandWrite)
            {
                tableName = request.TableHandWrite;

                if (!await _settingsManager.Database.IsTableExistsAsync(tableName))
                {
                    await _contentRepository.CreateContentTableAsync(tableName, _contentRepository.GetDefaultTableColumns(tableName));
                }
                else
                {
                    await _settingsManager.Database.AlterTableAsync(tableName, _contentRepository.GetDefaultTableColumns(tableName));
                }
            }

            var siteId = await _siteRepository.InsertSiteAsync(_pathManager, channelInfo, new Site
            {
                SiteName = request.SiteName,
                SiteDir = request.SiteDir,
                TableName = tableName,
                ParentId = request.ParentId,
                Root = request.Root
            }, auth.AdminId);

            if (string.IsNullOrEmpty(tableName))
            {
                tableName = ContentRepository.GetContentTableName(siteId);
                await _contentRepository.CreateContentTableAsync(tableName, _contentRepository.GetDefaultTableColumns(tableName));
                await _siteRepository.UpdateTableNameAsync(siteId, tableName);
            }

            if (await auth.AdminPermissions.IsSiteAdminAsync() && !await auth.AdminPermissions.IsSuperAdminAsync())
            {
                var siteIdList = await auth.AdminPermissions.GetSiteIdListAsync() ?? new List<int>();
                siteIdList.Add(siteId);
                var adminInfo = await _administratorRepository.GetByUserIdAsync(auth.AdminId);
                await _administratorRepository.UpdateSiteIdsAsync(adminInfo, siteIdList);
            }

            var site = await _siteRepository.GetAsync(siteId);

            Caching.SetProcess(request.Guid, "任务初始化...");

            if (request.CreateType == "local")
            {
                var manager = new SiteTemplateManager(_pathManager, _databaseManager);
                await manager.ImportSiteTemplateToEmptySiteAsync(site, request.CreateTemplateId, request.IsImportContents, request.IsImportTableStyles, auth.AdminId, request.Guid);

                Caching.SetProcess(request.Guid, "生成站点页面...");
                await _createManager.CreateByAllAsync(site.Id);

                Caching.SetProcess(request.Guid, "清除系统缓存...");
                CacheUtils.ClearAll();
            }
            else if (request.CreateType == "cloud")
            {
                Caching.SetProcess(request.Guid, "开始下载模板压缩包，可能需要几分钟，请耐心等待...");

                var filePath = _pathManager.GetSiteTemplatesPath($"T_{request.CreateTemplateId}.zip");
                FileUtils.DeleteFileIfExists(filePath);
                var downloadUrl = OnlineTemplateManager.GetDownloadUrl(request.CreateTemplateId);
                WebClientUtils.SaveRemoteFileToLocal(downloadUrl, filePath);

                Caching.SetProcess(request.Guid, "模板压缩包下载成功，开始解压缩，可能需要几分钟，请耐心等待...");

                var siteTemplateDir = $"T_{request.CreateTemplateId}";
                var directoryPath = _pathManager.GetSiteTemplatesPath(siteTemplateDir);
                DirectoryUtils.DeleteDirectoryIfExists(directoryPath);
                ZipUtils.ExtractZip(filePath, directoryPath);

                Caching.SetProcess(request.Guid, "模板压缩包解压成功，正在导入数据...");

                var manager = new SiteTemplateManager(_pathManager, _databaseManager);
                await manager.ImportSiteTemplateToEmptySiteAsync(site, siteTemplateDir, request.IsImportContents, request.IsImportTableStyles, auth.AdminId, request.Guid);

                Caching.SetProcess(request.Guid, "生成站点页面...");
                await _createManager.CreateByAllAsync(site.Id);

                Caching.SetProcess(request.Guid, "清除系统缓存...");
                CacheUtils.ClearAll();
            }

            return new IntResult
            {
                Value = siteId
            };
        }

        [HttpPost, Route(RouteProcess)]
        public async Task<ActionResult<Caching.Process>> Process([FromBody] ProcessRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSitesAdd))
            {
                return Unauthorized();
            }

            return Caching.GetProcess(request.Guid);
        }
    }
}
