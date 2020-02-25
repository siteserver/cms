using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Core;
using SS.CMS.Framework;
using SS.CMS.Core.Serialization;
using SS.CMS.Web.Extensions;

namespace SS.CMS.Web.Controllers.Home
{
    [Route("home/contentsLayerImport")]
    public partial class ContentsLayerImportController : ControllerBase
    {
        private const string Route = "";
        private const string RouteUpload = "actions/upload";

        private readonly IAuthManager _authManager;

        public ContentsLayerImportController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] ChannelRequest request)
        {
            var auth = await _authManager.GetUserAsync();
            if (!auth.IsUserLoggin ||
                !await auth.UserPermissions.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentAdd))
            {
                return Unauthorized();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await DataProvider.ChannelRepository.GetAsync(request.ChannelId);
            if (channel == null) return NotFound();

            var (isChecked, checkedLevel) = await CheckManager.GetUserCheckLevelAsync(auth.AdminPermissions, site, request.SiteId);
            var checkedLevels = CheckManager.GetCheckedLevels(site, isChecked, checkedLevel, true);

            return new GetResult
            {
                CheckedLevel = checkedLevel,
                CheckedLevels = checkedLevels
            };
        }

        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<UploadResult>> Upload([FromBody]UploadRequest request)
        {
            var auth = await _authManager.GetUserAsync();
            if (!auth.IsUserLoggin ||
                !await auth.UserPermissions.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentAdd))
            {
                return Unauthorized();
            }

            if (request.File == null)
            {
                return this.Error("请选择有效的文件上传");
            }

            var fileName = Path.GetFileName(request.File.FileName);
            if (!PathUtils.IsExtension(PathUtils.GetExtension(fileName), ".zip", ".csv", ".txt"))
            {
                return this.Error("请选择有效的文件上传!");
            }

            var filePath = PathUtility.GetTemporaryFilesPath(fileName);
            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            request.File.CopyTo(new FileStream(filePath, FileMode.Create));

            FileInfo fileInfo = null;
            if (!string.IsNullOrEmpty(filePath))
            {
                fileInfo = new FileInfo(filePath);
            }
            if (fileInfo != null)
            {
                return new UploadResult
                {
                    FileName = fileName,
                    Length = fileInfo.Length,
                    Ret = 1
                };
            }

            return new UploadResult
            {
                Ret = 0
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody]SubmitRequest request)
        {
            var auth = await _authManager.GetUserAsync();
            if (!auth.IsUserLoggin ||
                !await auth.UserPermissions.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentAdd))
            {
                return Unauthorized();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await DataProvider.ChannelRepository.GetAsync(request.ChannelId);
            if (channel == null) return NotFound();

            var isChecked = request.CheckedLevel >= site.CheckContentLevel;

            if (request.ImportType == "zip")
            {
                foreach (var fileName in request.FileNames)
                {
                    var localFilePath = PathUtility.GetTemporaryFilesPath(fileName);

                    if (!FileUtils.IsType(FileType.Zip, PathUtils.GetExtension(localFilePath)))
                        continue;

                    var importObject = new ImportObject(site, auth.AdminId);
                    await importObject.ImportContentsByZipFileAsync(channel, localFilePath, request.IsOverride, isChecked, request.CheckedLevel, auth.AdminId, auth.UserId, SourceManager.User);
                }
            }

            else if (request.ImportType == "csv")
            {
                foreach (var fileName in request.FileNames)
                {
                    var localFilePath = PathUtility.GetTemporaryFilesPath(fileName);

                    if (!FileUtils.IsType(FileType.Csv, PathUtils.GetExtension(localFilePath)))
                        continue;

                    var importObject = new ImportObject(site, auth.AdminId);
                    await importObject.ImportContentsByCsvFileAsync(channel, localFilePath, request.IsOverride, isChecked, request.CheckedLevel, auth.AdminId, auth.UserId, SourceManager.User);
                }
            }
            else if (request.ImportType == "txt")
            {
                foreach (var fileName in request.FileNames)
                {
                    var localFilePath = PathUtility.GetTemporaryFilesPath(fileName);
                    if (!FileUtils.IsType(FileType.Txt, PathUtils.GetExtension(localFilePath)))
                        continue;

                    var importObject = new ImportObject(site, auth.AdminId);
                    await importObject.ImportContentsByTxtFileAsync(channel, localFilePath, request.IsOverride, isChecked, request.CheckedLevel, auth.AdminId, auth.UserId, SourceManager.User);
                }
            }

            await auth.AddSiteLogAsync(request.SiteId, request.ChannelId, 0, "导入内容", string.Empty);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
