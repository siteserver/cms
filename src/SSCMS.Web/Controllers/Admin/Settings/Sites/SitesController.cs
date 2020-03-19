using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS;
using SSCMS.Core.Extensions;
using SSCMS.Core.Utils;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    [Route("admin/settings/sites")]
    public partial class SitesController : ControllerBase
    {
        private const string Route = "";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IPluginManager _pluginManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IContentRepository _contentRepository;

        public SitesController(ISettingsManager settingsManager, IAuthManager authManager, IPathManager pathManager, IPluginManager pluginManager, ISiteRepository siteRepository,
            IContentRepository contentRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _pluginManager = pluginManager;
            _siteRepository = siteRepository;
            _contentRepository = contentRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSites))
            {
                return Unauthorized();
            }

            var rootSiteId = await _siteRepository.GetIdByIsRootAsync();
            //var siteIdList = await _siteRepository.GetSiteIdListOrderByLevelAsync();
            //var sites = new List<Site>();
            //foreach (var siteId in siteIdList)
            //{

            //    var site = await _siteRepository.GetAsync(siteId);
            //    if (string.IsNullOrEmpty(keyword) || site.SiteName.Contains(keyword) || site.TableName.Contains(keyword) || site.SiteDir.Contains(keyword))
            //    {
            //        sites.Add(site);
            //    }
            //}
            //var siteIdList = await _siteRepository.GetSiteIdListAsync(0);
            //var sites = new List<Site>();
            //foreach (var siteId in siteIdList)
            //{
            //    var site = await _siteRepository.GetAsync(siteId);
            //    if (site != null)
            //    {
            //        sites.Add(site);
            //    }
            //}

            var sites = await _siteRepository.GetSitesWithChildrenAsync(0, async x => new
            {
                SiteUrl = await _pathManager.GetSiteUrlAsync(x, false)
            });

            var tableNames = await _siteRepository.GetSiteTableNamesAsync(_pluginManager);

            return new GetResult
            {
                Sites = sites,
                RootSiteId = rootSiteId,
                TableNames = tableNames
            };
        }

        [HttpDelete, Route(Route)]
        public async Task<ActionResult<SitesResult>> Delete([FromBody]DeleteRequest request)
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSites))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (!StringUtils.EqualsIgnoreCase(site.SiteDir, request.SiteDir))
            {
                return this.Error("删除失败，请输入正确的文件夹名称");
            }
            if (site.Children != null && site.Children.Count > 0)
            {
                return this.Error("删除失败，不允许删除父站点，在删除父站点前请先删除子站点");
            }

            if (request.DeleteFiles)
            {
                await _pathManager.DeleteSiteFilesAsync(site);
            }
            await _authManager.AddAdminLogAsync("删除站点", $"站点:{site.SiteName}");
            await _siteRepository.DeleteAsync(request.SiteId);

            var siteIdList = await _siteRepository.GetSiteIdListAsync(0);
            var sites = new List<Site>();
            foreach (var id in siteIdList)
            {
                var info = await _siteRepository.GetAsync(id);
                if (info != null)
                {
                    sites.Add(info);
                }
            }

            return new SitesResult
            {
                Sites = sites
            };
        }

        [HttpPut, Route(Route)]
        public async Task<ActionResult<SitesResult>> Edit([FromBody]EditRequest request)
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSites))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            site.SiteName = request.SiteName;
            site.Taxis = request.Taxis;

            var tableName = string.Empty;
            if (request.TableRule == TableRule.Choose)
            {
                tableName = request.TableChoose;
            }
            else if (request.TableRule == TableRule.HandWrite)
            {
                if (string.IsNullOrEmpty(request.TableHandWrite))
                {
                    return this.Error("站点修改失败，请输入内容表名称");
                }
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

            if (!StringUtils.EqualsIgnoreCase(site.TableName, tableName))
            {
                site.TableName = tableName;
            }

            if (site.Root == false)
            {
                if (!StringUtils.EqualsIgnoreCase(PathUtils.GetDirectoryName(site.SiteDir, false), request.SiteDir))
                {
                    var list = await _siteRepository.GetSiteDirListAsync(site.ParentId);
                    if (StringUtils.ContainsIgnoreCase(list, request.SiteDir))
                    {
                        return this.Error("站点修改失败，已存在相同的发布路径！");
                    }

                    var parentPsPath = _settingsManager.WebRootPath;
                    if (site.ParentId > 0)
                    {
                        var parentSite = await _siteRepository.GetAsync(site.ParentId);
                        parentPsPath = await _pathManager.GetSitePathAsync(parentSite);
                    }
                    DirectoryUtility.ChangeSiteDir(parentPsPath, site.SiteDir, request.SiteDir);
                }

                if (site.ParentId != request.ParentId)
                {
                    var list = await _siteRepository.GetSiteDirListAsync(request.ParentId);
                    if (StringUtils.ContainsIgnoreCase(list, request.SiteDir))
                    {
                        return this.Error("站点修改失败，已存在相同的发布路径！");
                    }

                    await _pathManager.ChangeParentSiteAsync(site.ParentId, request.ParentId, request.SiteId, request.SiteDir);
                    site.ParentId = request.ParentId;
                }

                site.SiteDir = request.SiteDir;
            }

            await _siteRepository.UpdateAsync(site);

            await _authManager.AddAdminLogAsync("修改站点属性", $"站点:{site.SiteName}");

            var siteIdList = await _siteRepository.GetSiteIdListAsync(0);
            var sites = new List<Site>();
            foreach (var id in siteIdList)
            {
                var info = await _siteRepository.GetAsync(id);
                if (info != null)
                {
                    sites.Add(info);
                }
            }

            return new SitesResult
            {
                Sites = sites
            };
        }
    }
}
