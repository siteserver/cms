using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Settings.Sites
{
    [RoutePrefix("pages/settings/sites")]
    public partial class PagesSitesController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<GetResult> Get()
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSites))
            {
                return Request.Unauthorized<GetResult>();
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
                SiteUrl = await PageUtility.GetSiteUrlAsync(x, true)
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
        public async Task<ActionResult> Delete()
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSites))
            {
                return Request.Unauthorized<ActionResult>();
            }

            var siteId = auth.GetPostInt("siteId");
            var siteDir = auth.GetPostString("siteDir");
            var deleteFiles = auth.GetPostBool("deleteFiles");

            var site = await DataProvider.SiteRepository.GetAsync(siteId);
            if (!StringUtils.EqualsIgnoreCase(site.SiteDir, siteDir))
            {
                return Request.BadRequest<ActionResult>("删除失败，请输入正确的文件夹名称");
            }
            if (site.Children != null && site.Children.Count > 0)
            {
                return Request.BadRequest<ActionResult>("删除失败，不允许删除父站点，在删除父站点前请先删除子站点");
            }

            if (deleteFiles)
            {
                await DirectoryUtility.DeleteSiteFilesAsync(site);
            }
            await auth.AddAdminLogAsync("删除站点", $"站点:{site.SiteName}");
            await DataProvider.SiteRepository.DeleteAsync(siteId);

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

            return new ActionResult
            {
                Sites = sites
            };
        }

        [HttpPut, Route(Route)]
        public async Task<ActionResult> Edit()
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSites))
            {
                return Request.Unauthorized<ActionResult>();
            }

            var siteId = auth.GetPostInt("siteId");
            var siteDir = auth.GetPostString("siteDir");
            var siteName = auth.GetPostString("siteName");
            var parentId = auth.GetPostInt("parentId");
            var taxis = auth.GetPostInt("taxis");
            var tableRule = ETableRuleUtils.GetEnumType(auth.GetPostString("tableRule"));
            var tableChoose = auth.GetPostString("tableChoose");
            var tableHandWrite = auth.GetPostString("tableHandWrite");

            var site = await DataProvider.SiteRepository.GetAsync(siteId);
            site.SiteName = siteName;
            site.Taxis = taxis;

            var tableName = string.Empty;
            if (tableRule == ETableRule.Choose)
            {
                tableName = tableChoose;
            }
            else if (tableRule == ETableRule.HandWrite)
            {
                if (string.IsNullOrEmpty(tableHandWrite))
                {
                    return Request.BadRequest<ActionResult>("站点修改失败，请输入内容表名称");
                }
                tableName = tableHandWrite;
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
                if (!StringUtils.EqualsIgnoreCase(PathUtils.GetDirectoryName(site.SiteDir, false), siteDir))
                {
                    var list = DataProvider.SiteRepository.GetLowerSiteDirListAsync(site.ParentId).GetAwaiter().GetResult();
                    if (list.Contains(siteDir.ToLower()))
                    {
                        return Request.BadRequest<ActionResult>("站点修改失败，已存在相同的发布路径！");
                    }

                    var parentPsPath = WebConfigUtils.PhysicalApplicationPath;
                    if (site.ParentId > 0)
                    {
                        var parentSite = await DataProvider.SiteRepository.GetAsync(site.ParentId);
                        parentPsPath = await PathUtility.GetSitePathAsync(parentSite);
                    }
                    DirectoryUtility.ChangeSiteDir(parentPsPath, site.SiteDir, siteDir);
                }

                if (site.ParentId != parentId)
                {
                    var list = await DataProvider.SiteRepository.GetLowerSiteDirListAsync(parentId);
                    if (list.Contains(siteDir.ToLower()))
                    {
                        return Request.BadRequest<ActionResult>("站点修改失败，已存在相同的发布路径！");
                    }

                    await DirectoryUtility.ChangeParentSiteAsync(site.ParentId, parentId, siteId, siteDir);
                    site.ParentId = parentId;
                }

                site.SiteDir = siteDir;
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

            return new ActionResult
            {
                Sites = sites
            };
        }
    }
}
