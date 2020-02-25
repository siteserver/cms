using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Core;
using SS.CMS.Framework;
using SS.CMS.Web.Extensions;

namespace SS.CMS.Web.Controllers.Admin.Settings.Sites
{
    [Route("admin/settings/sitesChangeRoot")]
    public partial class SitesChangeRootController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;

        public SitesChangeRootController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] SiteRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSites))
            {
                return Unauthorized();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);

            if (!site.Root && await DataProvider.SiteRepository.IsRootExistsAsync())
            {
                return this.Error($"根目录站点已经存在，站点{site.SiteName}不能转移到根目录");
            }

            var directories = new List<string>();
            var files = new List<string>();

            var fileSystems = FileUtility.GetFileSystemInfoExtendCollection(WebConfigUtils.PhysicalApplicationPath);
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

            return new GetResult
            {
                Site = site,
                Directories = directories,
                Files = files
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody]SubmitRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSites))
            {
                return Unauthorized();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            var root = site.Root;

            if (root)
            {
                var siteDirList = await DataProvider.SiteRepository.GetSiteDirListAsync(site.ParentId);
                if (StringUtils.ContainsIgnoreCase(siteDirList, request.SiteDir))
                {
                    return this.Error("操作失败，已存在相同的站点文件夹");
                }
                if (!DirectoryUtils.IsDirectoryNameCompliant(request.SiteDir))
                {
                    return this.Error("操作失败，站点文件夹名称不符合要求");
                }
                await DirectoryUtility.ChangeToSubSiteAsync(site, request.SiteDir, request.CheckedDirectories, request.CheckedFiles);
            }
            else
            {
                await DirectoryUtility.ChangeToRootAsync(site, request.IsMoveFiles);
            }

            await auth.AddAdminLogAsync(root ? "转移到子目录" : "转移到根目录",
                $"站点:{site.SiteName}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
