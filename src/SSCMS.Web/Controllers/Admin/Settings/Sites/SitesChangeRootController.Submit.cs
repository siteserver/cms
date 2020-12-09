using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesChangeRootController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSites))
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
                var rootPath = _pathManager.GetRootPath();
                var directories = DirectoryUtils.GetDirectoryNames(rootPath);
                if (ListUtils.ContainsIgnoreCase(directories, request.SiteDir))
                {
                    return this.Error("已存在相同的文件夹，请更改文件夹名称！");
                }
                var list = await _siteRepository.GetSiteDirsAsync(0);
                if (ListUtils.ContainsIgnoreCase(list, request.SiteDir))
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
