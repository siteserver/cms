using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Settings.Site
{
    [OpenApiIgnore]
    [RoutePrefix("pages/settings/siteTemplates")]
    public class PagesSiteTemplatesController : ApiController
    {
        private const string Route = "";
        private const string RouteZip = "actions/zip";
        private const string RouteUnZip = "actions/unZip";
        private const string RouteUpload = "actions/upload";

        private IHttpActionResult GetListResult()
        {
            var sortedList = SiteTemplateManager.Instance.GetSiteTemplateSortedList();
            var siteTemplateInfoList = new List<SiteTemplateInfo>();
            foreach (string directoryName in sortedList.Keys)
            {
                var directoryPath = PathUtility.GetSiteTemplatesPath(directoryName);
                var dirInfo = new DirectoryInfo(directoryPath);
                var siteTemplateInfo = sortedList[dirInfo.Name] as SiteTemplateInfo;
                if (string.IsNullOrEmpty(siteTemplateInfo?.SiteTemplateName)) continue;

                siteTemplateInfo.DirectoryName = directoryName;
                var filePath = PathUtility.GetSiteTemplatesPath(dirInfo.Name + ".zip");
                siteTemplateInfo.FileExists = FileUtils.IsFileExists(filePath);
                siteTemplateInfoList.Add(siteTemplateInfo);
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

            return Ok(new
            {
                Value = true,
                SiteTemplateInfoList = siteTemplateInfoList,
                FileNameList = fileNameList,
                SiteTemplateUrl = siteTemplateUrl
            });
        }

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> GetList()
        {
            try
            {
                var request = await AuthenticatedRequest.GetRequestAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(ConfigManager.SettingsPermissions.Site))
                {
                    return Unauthorized();
                }

                return GetListResult();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteZip)]
        public async Task<IHttpActionResult> Zip()
        {
            try
            {
                var request = await AuthenticatedRequest.GetRequestAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(ConfigManager.SettingsPermissions.Site))
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
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteUnZip)]
        public async Task<IHttpActionResult> UnZip()
        {
            try
            {
                var request = await AuthenticatedRequest.GetRequestAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(ConfigManager.SettingsPermissions.Site))
                {
                    return Unauthorized();
                }

                var fileNameToUnZip = request.GetPostString("fileName");

                var directoryPathToUnZip = PathUtility.GetSiteTemplatesPath(PathUtils.GetFileNameWithoutExtension(fileNameToUnZip));
                var zipFilePath = PathUtility.GetSiteTemplatesPath(fileNameToUnZip);

                ZipUtils.ExtractZip(zipFilePath, directoryPathToUnZip);

                return GetListResult();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete, Route(Route)]
        public async Task<IHttpActionResult> Delete()
        {
            try
            {
                var request = await AuthenticatedRequest.GetRequestAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(ConfigManager.SettingsPermissions.Site))
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
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteUpload)]
        public async Task<IHttpActionResult> Upload()
        {
            try
            {
                var request = await AuthenticatedRequest.GetRequestAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(ConfigManager.SettingsPermissions.Site))
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

                return GetListResult();
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }
    }
}
