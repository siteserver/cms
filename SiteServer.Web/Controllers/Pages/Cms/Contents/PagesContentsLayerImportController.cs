using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Dto.Request;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.ImportExport;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Contents
{
    [RoutePrefix("pages/cms/contents/contentsLayerImport")]
    public partial class PagesContentsLayerImportController : ApiController
    {
        private const string Route = "";
        private const string RouteUpload = "actions/upload";

        [HttpGet, Route(Route)]
        public async Task<GetResult> Get([FromUri] ChannelRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Contents) ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentAdd))
            {
                return Request.Unauthorized<GetResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<GetResult>();

            var channelInfo = await DataProvider.ChannelRepository.GetAsync(request.ChannelId);
            if (channelInfo == null) return Request.BadRequest<GetResult>("无法确定内容对应的栏目");

            var (isChecked, checkedLevel) = await CheckManager.GetUserCheckLevelAsync(auth.AdminPermissionsImpl, site, request.SiteId);
            var checkedLevels = CheckManager.GetCheckedLevels(site, isChecked, checkedLevel, true);

            return new GetResult
            {
                Value = checkedLevel,
                CheckedLevels = checkedLevels
            };
        }

        [HttpPost, Route(RouteUpload)]
        public async Task<UploadResult> Upload([FromUri] ChannelRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Contents) ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentAdd))
            {
                return Request.Unauthorized<UploadResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);

            var fileName = auth.HttpRequest["fileName"];
            var fileCount = auth.HttpRequest.Files.Count;
            if (fileCount == 0)
            {
                return Request.BadRequest<UploadResult>("请选择有效的文件上传");
            }

            var file = auth.HttpRequest.Files[0];

            if (string.IsNullOrEmpty(fileName)) fileName = Path.GetFileName(file.FileName);

            var extendName = fileName.Substring(fileName.LastIndexOf(".", StringComparison.Ordinal)).ToLower();
            if (!StringUtils.EqualsIgnoreCase(extendName, ".zip") && !StringUtils.EqualsIgnoreCase(extendName, ".csv") && !StringUtils.EqualsIgnoreCase(extendName, ".txt"))
            {
                return Request.BadRequest<UploadResult>("请选择有效的文件上传!");
            }

            var filePath = PathUtils.GetTemporaryFilesPath(fileName);
            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            file.SaveAs(filePath);

            var url = await PageUtility.GetSiteUrlByPhysicalPathAsync(site, filePath, true);

            return new UploadResult
            {
                Name = fileName,
                Url = url
            };
        }

        [HttpPost, Route(Route)]
        public async Task<BoolResult> Submit([FromBody] SubmitRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Contents) ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentAdd))
            {
                return Request.Unauthorized<BoolResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<BoolResult>();

            var channelInfo = await DataProvider.ChannelRepository.GetAsync(request.ChannelId);
            if (channelInfo == null) return Request.BadRequest<BoolResult>("无法确定内容对应的栏目");

            var isChecked = request.CheckedLevel >= site.CheckContentLevel;

            var contentIdList = new List<int>();

            if (request.ImportType == "zip")
            {
                foreach (var fileName in request.FileNames)
                {
                    var localFilePath = PathUtils.GetTemporaryFilesPath(fileName);

                    if (!EFileSystemTypeUtils.Equals(EFileSystemType.Zip, PathUtils.GetExtension(localFilePath)))
                        continue;

                    var importObject = new ImportObject(request.SiteId, auth.AdminName);
                    contentIdList.AddRange(await importObject.ImportContentsByZipFileAsync(channelInfo, localFilePath, request.IsOverride, isChecked, request.CheckedLevel, auth.AdminId, 0, SourceManager.Default));
                }
            }
            else if (request.ImportType == "csv")
            {
                foreach (var fileName in request.FileNames)
                {
                    var localFilePath = PathUtils.GetTemporaryFilesPath(fileName);

                    if (!EFileSystemTypeUtils.Equals(EFileSystemType.Csv, PathUtils.GetExtension(localFilePath)))
                        continue;

                    var importObject = new ImportObject(request.SiteId, auth.AdminName);
                    contentIdList.AddRange(await importObject.ImportContentsByCsvFileAsync(channelInfo, localFilePath, request.IsOverride, isChecked, request.CheckedLevel, auth.AdminId, 0, SourceManager.Default));
                }
            }
            else if (request.ImportType == "txt")
            {
                foreach (var fileName in request.FileNames)
                {
                    var localFilePath = PathUtils.GetTemporaryFilesPath(fileName);
                    if (!EFileSystemTypeUtils.Equals(EFileSystemType.Txt, PathUtils.GetExtension(localFilePath)))
                        continue;

                    var importObject = new ImportObject(request.SiteId, auth.AdminName);
                    contentIdList.AddRange(await importObject.ImportContentsByTxtFileAsync(channelInfo, localFilePath, request.IsOverride, isChecked, request.CheckedLevel, auth.AdminId, 0, SourceManager.Default));
                }
            }

            foreach (var contentId in contentIdList)
            {
                await CreateManager.CreateContentAsync(request.SiteId, channelInfo.Id, contentId);
            }
            await CreateManager.CreateChannelAsync(request.SiteId, channelInfo.Id);

            await auth.AddSiteLogAsync(request.SiteId, request.ChannelId, 0, "导入内容", string.Empty);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
