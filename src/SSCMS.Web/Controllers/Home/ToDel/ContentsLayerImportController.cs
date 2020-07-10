using System.IO;
using System.Threading.Tasks;
using CacheManager.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Core.Utils;
using SSCMS.Core.Utils.Serialization;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Extensions;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home.ToDel
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.User)]
    [Route(Constants.ApiHomePrefix + "todel/")]
    public partial class ContentsLayerImportController : ControllerBase
    {
        private const string Route = "contentsLayerImport";
        private const string RouteUpload = "contentsLayerImport/actions/upload";

        private readonly ICacheManager<CacheUtils.Process> _cacheManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IOldPluginManager _pluginManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;

        public ContentsLayerImportController(ICacheManager<CacheUtils.Process> cacheManager, IAuthManager authManager, IPathManager pathManager, IDatabaseManager databaseManager, IOldPluginManager pluginManager, ISiteRepository siteRepository, IChannelRepository channelRepository)
        {
            _cacheManager = cacheManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _pluginManager = pluginManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] ChannelRequest request)
        {
            if (!await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, AuthTypes.ContentPermissions.Add))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return NotFound();

            var (isChecked, checkedLevel) = await CheckManager.GetUserCheckLevelAsync(_authManager, site, request.SiteId);
            var checkedLevels = CheckManager.GetCheckedLevels(site, isChecked, checkedLevel, true);

            return new GetResult
            {
                CheckedLevel = checkedLevel,
                CheckedLevels = checkedLevels
            };
        }

        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<UploadResult>> Upload([FromQuery] ChannelRequest request, [FromForm] IFormFile file)
        {
            if (!await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, AuthTypes.ContentPermissions.Add))
            {
                return Unauthorized();
            }

            if (file == null)
            {
                return this.Error("请选择有效的文件上传");
            }

            var fileName = Path.GetFileName(file.FileName);
            if (!PathUtils.IsExtension(PathUtils.GetExtension(fileName), ".zip", ".csv", ".txt"))
            {
                return this.Error("请选择有效的文件上传!");
            }

            var filePath = _pathManager.GetTemporaryFilesPath(fileName);
            await _pathManager.UploadAsync(file, filePath);

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
            if (!await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, AuthTypes.ContentPermissions.Add))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return NotFound();

            var isChecked = request.CheckedLevel >= site.CheckContentLevel;
            var adminId = _authManager.AdminId;
            var userId = _authManager.UserId;

            var caching = new CacheUtils(_cacheManager);
            if (request.ImportType == "zip")
            {
                foreach (var fileName in request.FileNames)
                {
                    var localFilePath = _pathManager.GetTemporaryFilesPath(fileName);

                    if (!FileUtils.IsType(FileType.Zip, PathUtils.GetExtension(localFilePath)))
                        continue;

                    var importObject = new ImportObject(_pathManager, _pluginManager, _databaseManager, caching, site, adminId);
                    await importObject.ImportContentsByZipFileAsync(channel, localFilePath, request.IsOverride, isChecked, request.CheckedLevel, adminId, userId, SourceManager.User);
                }
            }

            else if (request.ImportType == "csv")
            {
                foreach (var fileName in request.FileNames)
                {
                    var localFilePath = _pathManager.GetTemporaryFilesPath(fileName);

                    if (!FileUtils.IsType(FileType.Csv, PathUtils.GetExtension(localFilePath)))
                        continue;

                    var importObject = new ImportObject(_pathManager, _pluginManager, _databaseManager, caching, site, adminId);
                    await importObject.ImportContentsByCsvFileAsync(channel, localFilePath, request.IsOverride, isChecked, request.CheckedLevel, adminId, userId, SourceManager.User);
                }
            }
            else if (request.ImportType == "txt")
            {
                foreach (var fileName in request.FileNames)
                {
                    var localFilePath = _pathManager.GetTemporaryFilesPath(fileName);
                    if (!FileUtils.IsType(FileType.Txt, PathUtils.GetExtension(localFilePath)))
                        continue;

                    var importObject = new ImportObject(_pathManager, _pluginManager, _databaseManager, caching, site, adminId);
                    await importObject.ImportContentsByTxtFileAsync(channel, localFilePath, request.IsOverride, isChecked, request.CheckedLevel, adminId, userId, SourceManager.User);
                }
            }

            await _authManager.AddSiteLogAsync(request.SiteId, request.ChannelId, 0, "导入内容", string.Empty);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
