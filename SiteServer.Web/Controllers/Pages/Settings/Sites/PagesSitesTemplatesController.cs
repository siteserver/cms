using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.API.Context;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Pages.Settings.Sites
{
    [RoutePrefix("pages/settings/sitesTemplates")]
    public class PagesSitesTemplatesController : ApiController
    {
        private const string Route = "";
        private const string RouteZip = "actions/zip";
        private const string RouteUnZip = "actions/unZip";
        private const string RouteUpload = "actions/upload";

        private async Task<IHttpActionResult> GetListResultAsync(AuthenticatedRequest request)
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
                await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSitesAdd);

            return Ok(new
            {
                Value = true,
                SiteTemplateInfoList = siteTemplateInfoList,
                FileNameList = fileNameList,
                SiteTemplateUrl = siteTemplateUrl,
                SiteAddPermission = siteAddPermission
            });
        }

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> GetList()
        {
            var request = await AuthenticatedRequest.GetAuthAsync();
            if (!request.IsAdminLoggin ||
                !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSitesTemplates))
            {
                return Unauthorized();
            }

            return await GetListResultAsync(request);
        }

        [HttpPost, Route(RouteZip)]
        public async Task<IHttpActionResult> Zip()
        {
            var request = await AuthenticatedRequest.GetAuthAsync();
            if (!request.IsAdminLoggin ||
                !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSitesTemplates))
            {
                return Unauthorized();
            }

            var directoryName = request.GetPostString("directoryName");

            directoryName = PathUtils.RemoveParentPath(directoryName);
            var fileName = directoryName + ".zip";
            var filePath = PathUtility.GetSiteTemplatesPath(fileName);
            var directoryPath = PathUtility.GetSiteTemplatesPath(directoryName);

            FileUtils.DeleteFileIfExists(filePath);

            ZipUtils.CreateZip(filePath, directoryPath);

            return Ok(new
            {
                Value = PageUtils.GetSiteTemplatesUrl(fileName)
            });
        }

        [HttpPost, Route(RouteUnZip)]
        public async Task<IHttpActionResult> UnZip()
        {
            var request = await AuthenticatedRequest.GetAuthAsync();
            if (!request.IsAdminLoggin ||
                !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSitesTemplates))
            {
                return Unauthorized();
            }

            var fileNameToUnZip = request.GetPostString("fileName");

            var directoryPathToUnZip = PathUtility.GetSiteTemplatesPath(PathUtils.GetFileNameWithoutExtension(fileNameToUnZip));
            var zipFilePath = PathUtility.GetSiteTemplatesPath(fileNameToUnZip);

            ZipUtils.ExtractZip(zipFilePath, directoryPathToUnZip);

            return await GetListResultAsync(request);
        }

        [HttpDelete, Route(Route)]
        public async Task<IHttpActionResult> Delete()
        {
            var request = await AuthenticatedRequest.GetAuthAsync();
            if (!request.IsAdminLoggin ||
                !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSitesTemplates))
            {
                return Unauthorized();
            }

            var directoryName = request.GetPostString("directoryName");
            var fileName = request.GetPostString("fileName");

            if (!string.IsNullOrEmpty(directoryName))
            {
                SiteTemplateManager.Instance.DeleteSiteTemplate(directoryName);
                await request.AddAdminLogAsync("删除站点模板", $"站点模板:{directoryName}");
            }
            if (!string.IsNullOrEmpty(fileName))
            {
                SiteTemplateManager.Instance.DeleteZipSiteTemplate(fileName);
                await request.AddAdminLogAsync("删除未解压站点模板", $"站点模板:{fileName}");
            }

            return Ok(new
            {
                Value = true
            });
        }

        [HttpPost, Route(RouteUpload)]
        public async Task<IHttpActionResult> Upload()
        {
            var request = await AuthenticatedRequest.GetAuthAsync();
            if (!request.IsAdminLoggin ||
                !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSitesTemplates))
            {
                return Unauthorized();
            }

            var fileName = request.HttpRequest["fileName"];
            var fileCount = request.HttpRequest.Files.Count;
            if (fileCount == 0)
            {
                return BadRequest("请选择有效的文件上传");
            }

            var file = request.HttpRequest.Files[0];
            if (string.IsNullOrEmpty(fileName)) fileName = Path.GetFileName(file.FileName);

            var sExt = PathUtils.GetExtension(fileName);
            if (!StringUtils.EqualsIgnoreCase(sExt, ".zip"))
            {
                return BadRequest("站点模板压缩包为zip格式，请选择有效的文件上传");
            }

            var directoryName = PathUtils.GetFileNameWithoutExtension(fileName);
            var directoryPath = PathUtility.GetSiteTemplatesPath(directoryName);
            if (DirectoryUtils.IsDirectoryExists(directoryPath))
            {
                return BadRequest($"站点模板导入失败，文件夹{directoryName}已存在");
            }
            var localFilePath = PathUtility.GetSiteTemplatesPath(directoryName + ".zip");
            FileUtils.DeleteFileIfExists(localFilePath);

            file.SaveAs(localFilePath);

            ZipUtils.ExtractZip(localFilePath, directoryPath);

            return await GetListResultAsync(request);
        }
    }
}
