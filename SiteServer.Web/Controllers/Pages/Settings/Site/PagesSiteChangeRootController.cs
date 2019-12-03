using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Settings.Site
{
    
    [RoutePrefix("pages/settings/siteChangeRoot")]
    public class PagesSiteChangeRootController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> GetConfig()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.SettingsPermissions.Site))
                {
                    return Unauthorized();
                }

                var siteId = request.GetQueryInt("siteId");
                var site = await DataProvider.SiteRepository.GetAsync(siteId);

                if (!site.Root && await DataProvider.SiteRepository.IsRootExistsAsync())
                {
                    return BadRequest($"根目录站点已经存在，站点{site.SiteName}不能转移到根目录");
                }

                var directories = new List<string>();
                var files = new List<string>();

                var fileSystems = FileManager.GetFileSystemInfoExtendCollection(WebConfigUtils.PhysicalApplicationPath, true);
                var siteDirList = await DataProvider.SiteRepository.GetSiteDirListAsync(0);
                foreach (FileSystemInfoExtend fileSystem in fileSystems)
                {
                    if (fileSystem.IsDirectory)
                    {
                        if (!WebUtils.IsSystemDirectory(fileSystem.Name) && !siteDirList.Contains(fileSystem.Name.ToLower()))
                        {
                            directories.Add(fileSystem.Name);
                        }
                    }
                    else
                    {
                        if (!PathUtility.IsSystemFileForChangeSiteType(fileSystem.Name))
                        {
                            files.Add(fileSystem.Name);
                        }
                    }
                }

                return Ok(new
                {
                    Value = site,
                    Directories = directories,
                    Files = files
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> Submit()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.SettingsPermissions.Site))
                {
                    return Unauthorized();
                }

                var siteId = request.GetPostInt("siteId");
                var siteDir = request.GetPostString("siteDir");
                var checkedDirectories = request.GetPostObject<IList<string>>("checkedDirectories");
                var checkedFiles = request.GetPostObject<IList<string>>("checkedFiles");
                var isMoveFiles = request.GetPostBool("isMoveFiles");

                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                var root = site.Root;

                if (root)
                {
                    var siteDirList = await DataProvider.SiteRepository.GetSiteDirListAsync(site.ParentId);
                    if (StringUtils.ContainsIgnoreCase(siteDirList, siteDir))
                    {
                        return BadRequest("操作失败，已存在相同的站点文件夹");
                    }
                    if (!DirectoryUtils.IsDirectoryNameCompliant(siteDir))
                    {
                        return BadRequest("操作失败，站点文件夹名称不符合要求");
                    }
                    await DirectoryUtility.ChangeToSubSiteAsync(site, siteDir, checkedDirectories, checkedFiles);
                }
                else
                {
                    await DirectoryUtility.ChangeToRootAsync(site, isMoveFiles);
                }

                request.AddAdminLogAsync(root ? "转移到子目录" : "转移到根目录",
                    $"站点:{site.SiteName}").GetAwaiter().GetResult();

                return Ok(new
                {
                    Value = true
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
