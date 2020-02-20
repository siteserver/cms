using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Serialization;

namespace SiteServer.API.Controllers.Home
{
    
    [RoutePrefix("home/contentsLayerImport")]
    public class HomeContentsLayerImportController : ApiController
    {
        private const string Route = "";
        private const string RouteUpload = "actions/upload";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> GetConfig()
        {
            var request = await AuthenticatedRequest.GetAuthAsync();

            var siteId = request.GetQueryInt("siteId");
            var channelId = request.GetQueryInt("channelId");

            if (!request.IsUserLoggin ||
                !await request.UserPermissionsImpl.HasChannelPermissionsAsync(siteId, channelId,
                    Constants.ChannelPermissions.ContentAdd))
            {
                return Unauthorized();
            }

            var site = await DataProvider.SiteRepository.GetAsync(siteId);
            if (site == null) return BadRequest("无法确定内容对应的站点");

            var channelInfo = await DataProvider.ChannelRepository.GetAsync(channelId);
            if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

            var (isChecked, checkedLevel) = await CheckManager.GetUserCheckLevelAsync(request.AdminPermissionsImpl, site, siteId);
            var checkedLevels = CheckManager.GetCheckedLevels(site, isChecked, checkedLevel, true);

            return Ok(new
            {
                Value = checkedLevel,
                CheckedLevels = checkedLevels
            });
        }

        [HttpPost, Route(RouteUpload)]
        public async Task<IHttpActionResult> Upload()
        {
            var request = await AuthenticatedRequest.GetAuthAsync();

            var siteId = request.GetQueryInt("siteId");
            var channelId = request.GetQueryInt("channelId");

            if (!request.IsUserLoggin ||
                !await request.UserPermissionsImpl.HasChannelPermissionsAsync(siteId, channelId,
                    Constants.ChannelPermissions.ContentAdd))
            {
                return Unauthorized();
            }

            var fileName = request.HttpRequest["fileName"];
            var fileCount = request.HttpRequest.Files.Count;
            string filePath = null;

            if (fileCount > 0)
            {
                var file = request.HttpRequest.Files[0];

                if (string.IsNullOrEmpty(fileName)) fileName = Path.GetFileName(file.FileName);

                var extendName = fileName.Substring(fileName.LastIndexOf(".", StringComparison.Ordinal)).ToLower();
                if (extendName == ".zip" || extendName == ".csv" || extendName == ".txt")
                {
                    filePath = PathUtility.GetTemporaryFilesPath(fileName);
                    DirectoryUtils.CreateDirectoryIfNotExists(filePath);
                    file.SaveAs(filePath);
                }
            }

            FileInfo fileInfo = null;
            if (!string.IsNullOrEmpty(filePath))
            {
                fileInfo = new FileInfo(filePath);
            }
            if (fileInfo != null)
            {
                return Ok(new
                {
                    fileName,
                    length = fileInfo.Length,
                    ret = 1
                });
            }

            return Ok(new
            {
                ret = 0
            });
        }

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> Submit()
        {
            var request = await AuthenticatedRequest.GetAuthAsync();

            var siteId = request.GetPostInt("siteId");
            var channelId = request.GetPostInt("channelId");
            var importType = request.GetPostString("importType");
            var checkedLevel = request.GetPostInt("checkedLevel");
            var isOverride = request.GetPostBool("isOverride");
            var fileNames = request.GetPostObject<List<string>>("fileNames");

            if (!request.IsUserLoggin ||
                !await request.UserPermissionsImpl.HasChannelPermissionsAsync(siteId, channelId,
                    Constants.ChannelPermissions.ContentAdd))
            {
                return Unauthorized();
            }

            var site = await DataProvider.SiteRepository.GetAsync(siteId);
            if (site == null) return BadRequest("无法确定内容对应的站点");

            var channelInfo = await DataProvider.ChannelRepository.GetAsync(channelId);
            if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

            var isChecked = checkedLevel >= site.CheckContentLevel;

            if (importType == "zip")
            {
                foreach (var fileName in fileNames)
                {
                    var localFilePath = PathUtility.GetTemporaryFilesPath(fileName);

                    if (!FileUtils.IsType(FileType.Zip, PathUtils.GetExtension(localFilePath)))
                        continue;

                    var importObject = new ImportObject(site, request.AdminId);
                    await importObject.ImportContentsByZipFileAsync(channelInfo, localFilePath, isOverride, isChecked, checkedLevel, request.AdminId, request.UserId, SourceManager.User);
                }
            }

            else if (importType == "csv")
            {
                foreach (var fileName in fileNames)
                {
                    var localFilePath = PathUtility.GetTemporaryFilesPath(fileName);

                    if (!FileUtils.IsType(FileType.Csv, PathUtils.GetExtension(localFilePath)))
                        continue;

                    var importObject = new ImportObject(site, request.AdminId);
                    await importObject.ImportContentsByCsvFileAsync(channelInfo, localFilePath, isOverride, isChecked, checkedLevel, request.AdminId, request.UserId, SourceManager.User);
                }
            }
            else if (importType == "txt")
            {
                foreach (var fileName in fileNames)
                {
                    var localFilePath = PathUtility.GetTemporaryFilesPath(fileName);
                    if (!FileUtils.IsType(FileType.Txt, PathUtils.GetExtension(localFilePath)))
                        continue;

                    var importObject = new ImportObject(site, request.AdminId);
                    await importObject.ImportContentsByTxtFileAsync(channelInfo, localFilePath, isOverride, isChecked, checkedLevel, request.AdminId, request.UserId, SourceManager.User);
                }
            }

            await request.AddSiteLogAsync(siteId, channelId, 0, "导入内容", string.Empty);

            return Ok(new
            {
                Value = true
            });
        }
    }
}
