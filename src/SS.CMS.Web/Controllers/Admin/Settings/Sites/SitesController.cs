using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Core;
using SS.CMS.Framework;
using SS.CMS.Web.Extensions;

namespace SS.CMS.Web.Controllers.Admin.Settings.Sites
{
    [Route("admin/settings/sites")]
    public partial class SitesController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;

        public SitesController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSites))
            {
                return Unauthorized();
            }

            var rootSiteId = await DataProvider.SiteRepository.GetIdByIsRootAsync();
            //var siteIdList = await DataProvider.SiteRepository.GetSiteIdListOrderByLevelAsync();
            //var sites = new List<Site>();
            //foreach (var siteId in siteIdList)
            //{

            //    var site = await DataProvider.SiteRepository.GetAsync(siteId);
            //    if (string.IsNullOrEmpty(keyword) || site.SiteName.Contains(keyword) || site.TableName.Contains(keyword) || site.SiteDir.Contains(keyword))
            //    {
            //        sites.Add(site);
            //    }
            //}
            //var siteIdList = await DataProvider.SiteRepository.GetSiteIdListAsync(0);
            //var sites = new List<Site>();
            //foreach (var siteId in siteIdList)
            //{
            //    var site = await DataProvider.SiteRepository.GetAsync(siteId);
            //    if (site != null)
            //    {
            //        sites.Add(site);
            //    }
            //}

            var sites = await DataProvider.SiteRepository.GetSitesWithChildrenAsync(0, async x => new
            {
                SiteUrl = await PageUtility.GetSiteUrlAsync(x, false)
            });

            var tableNames = await DataProvider.SiteRepository.GetSiteTableNamesAsync();

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
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSites))
            {
                return Unauthorized();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
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
                await DirectoryUtility.DeleteSiteFilesAsync(site);
            }
            await auth.AddAdminLogAsync("删除站点", $"站点:{site.SiteName}");
            await DataProvider.SiteRepository.DeleteAsync(request.SiteId);

            var siteIdList = await DataProvider.SiteRepository.GetSiteIdListAsync(0);
            var sites = new List<Site>();
            foreach (var id in siteIdList)
            {
                var info = await DataProvider.SiteRepository.GetAsync(id);
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
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSites))
            {
                return Unauthorized();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
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
                if (!await WebConfigUtils.Database.IsTableExistsAsync(tableName))
                {
                    await DataProvider.ContentRepository.CreateContentTableAsync(tableName, DataProvider.ContentRepository.GetDefaultTableColumns(tableName));
                }
                else
                {
                    await WebConfigUtils.Database.AlterTableAsync(tableName, DataProvider.ContentRepository.GetDefaultTableColumns(tableName));
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
                    var list = await DataProvider.SiteRepository.GetLowerSiteDirListAsync(site.ParentId);
                    if (list.Contains(request.SiteDir.ToLower()))
                    {
                        return this.Error("站点修改失败，已存在相同的发布路径！");
                    }

                    var parentPsPath = WebConfigUtils.PhysicalApplicationPath;
                    if (site.ParentId > 0)
                    {
                        var parentSite = await DataProvider.SiteRepository.GetAsync(site.ParentId);
                        parentPsPath = await PathUtility.GetSitePathAsync(parentSite);
                    }
                    DirectoryUtility.ChangeSiteDir(parentPsPath, site.SiteDir, request.SiteDir);
                }

                if (site.ParentId != request.ParentId)
                {
                    var list = await DataProvider.SiteRepository.GetLowerSiteDirListAsync(request.ParentId);
                    if (list.Contains(request.SiteDir.ToLower()))
                    {
                        return this.Error("站点修改失败，已存在相同的发布路径！");
                    }

                    await DirectoryUtility.ChangeParentSiteAsync(site.ParentId, request.ParentId, request.SiteId, request.SiteDir);
                    site.ParentId = request.ParentId;
                }

                site.SiteDir = request.SiteDir;
            }

            await DataProvider.SiteRepository.UpdateAsync(site);

            await auth.AddAdminLogAsync("修改站点属性", $"站点:{site.SiteName}");

            var siteIdList = await DataProvider.SiteRepository.GetSiteIdListAsync(0);
            var sites = new List<Site>();
            foreach (var id in siteIdList)
            {
                var info = await DataProvider.SiteRepository.GetAsync(id);
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
