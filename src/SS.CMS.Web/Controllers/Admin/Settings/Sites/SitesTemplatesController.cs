using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Core;
using SS.CMS.Web.Extensions;

namespace SS.CMS.Web.Controllers.Admin.Settings.Sites
{
    [Route("admin/settings/sitesTemplates")]
    public partial class SitesTemplatesController : ControllerBase
    {
        private const string Route = "";
        private const string RouteZip = "actions/zip";
        private const string RouteUnZip = "actions/unZip";
        private const string RouteUpload = "actions/upload";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;

        public SitesTemplatesController(IAuthManager authManager, IPathManager pathManager)
        {
            _authManager = authManager;
            _pathManager = pathManager;
        }

        private async Task<ListResult> GetListResultAsync()
        {
            var siteTemplates = SiteTemplateManager.Instance.GetSiteTemplateInfoList();
            var siteTemplateInfoList = new List<SiteTemplateInfo>();
            foreach (var siteTemplate in siteTemplates)
            {
                var directoryPath = PathUtility.GetSiteTemplatesPath(siteTemplate.DirectoryName);
                var dirInfo = new DirectoryInfo(directoryPath);
                if (string.IsNullOrEmpty(siteTemplate.SiteTemplateName)) continue;

                var filePath = PathUtility.GetSiteTemplatesPath(dirInfo.Name + ".zip");
                siteTemplate.FileExists = FileUtils.IsFileExists(filePath);
                siteTemplateInfoList.Add(siteTemplate);
            }

            var fileNames = SiteTemplateManager.Instance.GetZipSiteTemplateList();
            var fileNameList = new List<string>();
            foreach (var fileName in fileNames)
            {
                if (DirectoryUtils.IsDirectoryExists(
                    PathUtility.GetSiteTemplatesPath(PathUtils.GetFileNameWithoutExtension(fileName)))) continue;
                var filePath = PathUtility.GetSiteTemplatesPath(fileName);
                var fileInfo = new FileInfo(filePath);
                fileNameList.Add(fileInfo.Name);
            }

            var siteTemplateUrl = StringUtils.TrimSlash(PageUtils.GetSiteTemplatesUrl(string.Empty));
            var siteAddPermission =
                await _authManager.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSitesAdd);

            return new ListResult
            {
                SiteTemplateInfoList = siteTemplateInfoList,
                FileNameList = fileNameList,
                SiteTemplateUrl = siteTemplateUrl,
                SiteAddPermission = siteAddPermission
            };
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<ListResult>> GetList()
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSitesTemplates))
            {
                return Unauthorized();
            }

            return await GetListResultAsync();
        }

        [HttpPost, Route(RouteZip)]
        public async Task<ActionResult<StringResult>> Zip([FromBody]ZipRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSitesTemplates))
            {
                return Unauthorized();
            }

            var directoryName = PathUtils.RemoveParentPath(request.DirectoryName);
            var fileName = directoryName + ".zip";
            var filePath = PathUtility.GetSiteTemplatesPath(fileName);
            var directoryPath = PathUtility.GetSiteTemplatesPath(directoryName);

            FileUtils.DeleteFileIfExists(filePath);

            ZipUtils.CreateZip(filePath, directoryPath);

            return new StringResult
            {
                Value = PageUtils.GetSiteTemplatesUrl(fileName)
            };
        }

        [HttpPost, Route(RouteUnZip)]
        public async Task<ActionResult<ListResult>> UnZip([FromBody]UnZipRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSitesTemplates))
            {
                return Unauthorized();
            }

            var fileNameToUnZip = request.FileName;

            var directoryPathToUnZip = PathUtility.GetSiteTemplatesPath(PathUtils.GetFileNameWithoutExtension(fileNameToUnZip));
            var zipFilePath = PathUtility.GetSiteTemplatesPath(fileNameToUnZip);

            ZipUtils.ExtractZip(zipFilePath, directoryPathToUnZip);

            return await GetListResultAsync();
        }

        [HttpDelete, Route(Route)]
        public async Task<ActionResult<BoolResult>> Delete([FromBody]DeleteRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSitesTemplates))
            {
                return Unauthorized();
            }

            if (!string.IsNullOrEmpty(request.DirectoryName))
            {
                SiteTemplateManager.Instance.DeleteSiteTemplate(request.DirectoryName);
                await auth.AddAdminLogAsync("删除站点模板", $"站点模板:{request.DirectoryName}");
            }
            if (!string.IsNullOrEmpty(request.FileName))
            {
                SiteTemplateManager.Instance.DeleteZipSiteTemplate(request.FileName);
                await auth.AddAdminLogAsync("删除未解压站点模板", $"站点模板:{request.FileName}");
            }

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<ListResult>> Upload([FromForm]IFormFile file)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSitesTemplates))
            {
                return Unauthorized();
            }

            if (file == null) return this.Error("请选择有效的文件上传");
            var extension = PathUtils.GetExtension(file.FileName);
            if (!FileUtils.IsZip(extension))
            {
                return this.Error("站点模板压缩包为zip格式，请选择有效的文件上传!");
            }
            var directoryName = PathUtils.GetFileNameWithoutExtension(file.FileName);
            var directoryPath = _pathManager.GetSiteFilesPath(PathUtils.Combine(DirectoryUtils.SiteTemplates.DirectoryName, directoryName));
            if (DirectoryUtils.IsDirectoryExists(directoryPath))
            {
                return this.Error($"站点模板导入失败，文件夹{directoryName}已存在");
            }
            DirectoryUtils.CreateDirectoryIfNotExists(directoryPath);
            var filePath = _pathManager.GetSiteFilesPath(PathUtils.Combine(DirectoryUtils.SiteTemplates.DirectoryName, file.FileName));
            
            FileUtils.DeleteFileIfExists(filePath);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            ZipUtils.ExtractZip(filePath, directoryPath);

            return await GetListResultAsync();
        }
    }
}
