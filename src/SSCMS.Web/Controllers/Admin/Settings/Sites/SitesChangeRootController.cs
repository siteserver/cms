using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS;
using SSCMS.Dto.Request;
using SSCMS.Dto.Result;
using SSCMS.Core.Extensions;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
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
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSites))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            if (!site.Root && await _siteRepository.IsRootExistsAsync())
            {
                return this.Error($"根目录站点已经存在，站点{site.SiteName}不能转移到根目录");
            }

            var siteDirList = await _siteRepository.GetSiteDirListAsync(0);
            var directories = new List<string>();
            var directoryNames = DirectoryUtils.GetDirectoryNames(_settingsManager.WebRootPath);
            foreach (var directoryName in directoryNames)
            {
                if (!_pathManager.IsSystemDirectory(directoryName) && !StringUtils.ContainsIgnoreCase(siteDirList, directoryName))
                {
                    directories.Add(directoryName);
                }
            }
            var files = DirectoryUtils.GetFileNames(_settingsManager.WebRootPath);

            

            //var fileSystems = FileUtility.GetFileSystemInfoExtendCollection(_settingsManager.WebRootPath);
            
            //foreach (FileSystemInfoExtend fileSystem in fileSystems)
            //{
            //    if (fileSystem.IsDirectory)
            //    {
            //        if (!_pathManager.IsSystemDirectory(fileSystem.Name) && ! StringUtils.ContainsIgnoreCase(siteDirList, fileSystem.Name))
            //        {
            //            directories.Add(fileSystem.Name);
            //        }
            //    }
            //    else
            //    {
            //        files.Add(fileSystem.Name);
            //    }
            //}

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
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSites))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            var root = site.Root;

            if (root)
            {
                if (_pathManager.IsSystemDirectory(request.SiteDir))
                {
                    return this.Error("文件夹名称不能为系统文件夹名称，请更改文件夹名称！");
                }
                if (!DirectoryUtils.IsDirectoryNameCompliant(request.SiteDir))
                {
                    return this.Error("文件夹名称不符合系统要求，请更改文件夹名称！");
                }
                var rootPath = _pathManager.GetWebRootPath();
                var directories = DirectoryUtils.GetDirectoryNames(rootPath);
                if (StringUtils.ContainsIgnoreCase(directories, request.SiteDir))
                {
                    return this.Error("已存在相同的文件夹，请更改文件夹名称！");
                }
                var list = await _siteRepository.GetSiteDirListAsync(0);
                if (StringUtils.ContainsIgnoreCase(list, request.SiteDir))
                {
                    return this.Error("已存在相同的站点文件夹，请更改文件夹名称！");
                }
                await _pathManager.ChangeToSubSiteAsync(site, request.SiteDir, request.CheckedDirectories, request.CheckedFiles);
            }
            else
            {
                await _pathManager.ChangeToRootAsync(site, request.IsMoveFiles);
            }

            await _authManager.AddAdminLogAsync(root ? "转移到子目录" : "转移到根目录",
                $"站点:{site.SiteName}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
