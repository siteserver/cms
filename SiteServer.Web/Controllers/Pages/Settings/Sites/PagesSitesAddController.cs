using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Dto;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Settings.Sites
{
    [RoutePrefix("pages/settings/sitesAdd")]
    public partial class PagesSitesAddController : ApiController
    {
        private const string Route = "";
        private const string RouteProcess = "actions/process";

        [HttpGet, Route(Route)]
        public async Task<GetResult> Get()
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSitesAdd))
            {
                return Request.Unauthorized<GetResult>();
            }

            var siteTemplates = SiteTemplateManager.Instance.GetSiteTemplateInfoList();

            var tableNameList = await DataProvider.SiteRepository.GetSiteTableNamesAsync();

            var rootExists = await DataProvider.SiteRepository.GetSiteByIsRootAsync() != null;

            var sites = await DataProvider.SiteRepository.GetCascadeChildrenAsync(0);
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
        public async Task<IntResult> Submit([FromBody] SubmitRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSitesAdd))
            {
                return Request.Unauthorized<IntResult>();
            }

            if (!request.Root)
            {
                if (WebUtils.IsSystemDirectory(request.SiteDir))
                {
                    return Request.BadRequest<IntResult>("文件夹名称不能为系统文件夹名称，请更改文件夹名称！");
                }
                if (!DirectoryUtils.IsDirectoryNameCompliant(request.SiteDir))
                {
                    return Request.BadRequest<IntResult>("文件夹名称不符合系统要求，请更改文件夹名称！");
                }
                var list = await DataProvider.SiteRepository.GetLowerSiteDirListAsync(request.ParentId);
                if (list.Contains(request.SiteDir.ToLower()))
                {
                    return Request.BadRequest<IntResult>("已存在相同的发布路径，请更改文件夹名称！");
                }
            }

            var channelInfo = new Channel();

            channelInfo.ChannelName = channelInfo.IndexName = "首页";
            channelInfo.ParentId = 0;
            channelInfo.ContentModelPluginId = string.Empty;

            var tableName = string.Empty;
            if (request.TableRule == ETableRule.Choose)
            {
                tableName = request.TableChoose;
            }
            else if (request.TableRule == ETableRule.HandWrite)
            {
                tableName = request.TableHandWrite;

                if (!await WebConfigUtils.Database.IsTableExistsAsync(tableName))
                {
                    await DataProvider.ContentRepository.CreateContentTableAsync(tableName, DataProvider.ContentRepository.GetDefaultTableColumns(tableName));
                }
                else
                {
                    await WebConfigUtils.Database.AlterTableAsync(tableName, DataProvider.ContentRepository.GetDefaultTableColumns(tableName));
                }
            }

            var siteId = await DataProvider.ChannelRepository.InsertSiteAsync(channelInfo, new Site
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
                await DataProvider.ContentRepository.CreateContentTableAsync(tableName, DataProvider.ContentRepository.GetDefaultTableColumns(tableName));
                await DataProvider.SiteRepository.UpdateTableNameAsync(siteId, tableName);
            }

            if (await auth.AdminPermissionsImpl.IsSiteAdminAsync() && !await auth.AdminPermissionsImpl.IsSuperAdminAsync())
            {
                var siteIdList = await auth.AdminPermissionsImpl.GetSiteIdListAsync() ?? new List<int>();
                siteIdList.Add(siteId);
                var adminInfo = await DataProvider.AdministratorRepository.GetByUserIdAsync(auth.AdminId);
                await DataProvider.AdministratorRepository.UpdateSiteIdsAsync(adminInfo, siteIdList);
            }

            var site = await DataProvider.SiteRepository.GetAsync(siteId);

            Caching.SetProcess(request.Guid, "任务初始化...");

            if (request.CreateType == "local")
            {
                await SiteTemplateManager.Instance.ImportSiteTemplateToEmptySiteAsync(site, request.CreateTemplateId, request.IsImportContents, request.IsImportTableStyles, auth.AdminId, request.Guid);

                Caching.SetProcess(request.Guid, "生成站点页面...");
                await CreateManager.CreateByAllAsync(site.Id);

                Caching.SetProcess(request.Guid, "清除系统缓存...");
                CacheUtils.ClearAll();
            }
            else if (request.CreateType == "cloud")
            {
                Caching.SetProcess(request.Guid, "开始下载模板压缩包，可能需要几分钟，请耐心等待...");

                var filePath = PathUtility.GetSiteTemplatesPath($"T_{request.CreateTemplateId}.zip");
                FileUtils.DeleteFileIfExists(filePath);
                var downloadUrl = OnlineTemplateManager.GetDownloadUrl(request.CreateTemplateId);
                WebClientUtils.SaveRemoteFileToLocal(downloadUrl, filePath);

                Caching.SetProcess(request.Guid, "模板压缩包下载成功，开始解压缩，可能需要几分钟，请耐心等待...");

                var siteTemplateDir = $"T_{request.CreateTemplateId}";
                var directoryPath = PathUtility.GetSiteTemplatesPath(siteTemplateDir);
                DirectoryUtils.DeleteDirectoryIfExists(directoryPath);
                ZipUtils.ExtractZip(filePath, directoryPath);

                Caching.SetProcess(request.Guid, "模板压缩包解压成功，正在导入数据...");

                await SiteTemplateManager.Instance.ImportSiteTemplateToEmptySiteAsync(site, siteTemplateDir, request.IsImportContents, request.IsImportTableStyles, auth.AdminId, request.Guid);

                Caching.SetProcess(request.Guid, "生成站点页面...");
                await CreateManager.CreateByAllAsync(site.Id);

                Caching.SetProcess(request.Guid, "清除系统缓存...");
                CacheUtils.ClearAll();
            }

            return new IntResult
            {
                Value = siteId
            };
        }

        [HttpPost, Route(RouteProcess)]
        public async Task<Caching.Process> Process([FromBody] ProcessRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSitesAdd))
            {
                return Request.Unauthorized<Caching.Process>();
            }

            return Caching.GetProcess(request.Guid);
        }
    }
}
