using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesChangeRootController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSites))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            if (!site.Root && await _siteRepository.IsRootExistsAsync())
            {
                return this.Error($"根目录站点已经存在，站点{site.SiteName}不能转移到根目录");
            }

            var siteDirList = await _siteRepository.GetSiteDirsAsync(0);
            var directories = new List<string>();
            var directoryNames = DirectoryUtils.GetDirectoryNames(_settingsManager.WebRootPath);
            foreach (var directoryName in directoryNames)
            {
                if (!_pathManager.IsSystemDirectory(directoryName) && !ListUtils.ContainsIgnoreCase(siteDirList, directoryName))
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
    }
}
