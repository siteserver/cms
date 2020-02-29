using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Core;
using SS.CMS.Web.Extensions;

namespace SS.CMS.Web.Controllers.Admin.Settings.Sites
{
    [Route("admin/settings/sitesChangeRoot")]
    public partial class SitesChangeRootController : ControllerBase
    {
        private const string Route = "";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;

        public SitesChangeRootController(ISettingsManager settingsManager, IAuthManager authManager, IPathManager pathManager, ISiteRepository siteRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _siteRepository = siteRepository;
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

            var site = await _siteRepository.GetAsync(request.SiteId);

            if (!site.Root && await _siteRepository.IsRootExistsAsync())
            {
                return this.Error($"根目录站点已经存在，站点{site.SiteName}不能转移到根目录");
            }

            var directories = new List<string>();
            var files = new List<string>();

            var fileSystems = FileUtility.GetFileSystemInfoExtendCollection(_settingsManager.WebRootPath);
            var siteDirList = await _siteRepository.GetSiteDirListAsync(0);
            foreach (FileSystemInfoExtend fileSystem in fileSystems)
            {
                if (fileSystem.IsDirectory)
                {
                    if (!_pathManager.IsSystemDirectory(fileSystem.Name) && !siteDirList.Contains(fileSystem.Name.ToLower()))
                    {
                        directories.Add(fileSystem.Name);
                    }
                }
                else
                {
                    if (!_pathManager.IsSystemFileForChangeSiteType(fileSystem.Name))
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

            var site = await _siteRepository.GetAsync(request.SiteId);
            var root = site.Root;

            if (root)
            {
                var siteDirList = await _siteRepository.GetSiteDirListAsync(site.ParentId);
                if (StringUtils.ContainsIgnoreCase(siteDirList, request.SiteDir))
                {
                    return this.Error("操作失败，已存在相同的站点文件夹");
                }
                if (!DirectoryUtils.IsDirectoryNameCompliant(request.SiteDir))
                {
                    return this.Error("操作失败，站点文件夹名称不符合要求");
                }
                await _pathManager.ChangeToSubSiteAsync(site, request.SiteDir, request.CheckedDirectories, request.CheckedFiles);
            }
            else
            {
                await _pathManager.ChangeToRootAsync(site, request.IsMoveFiles);
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
