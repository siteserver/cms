using System;
using System.Collections.Generic;
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

namespace SSCMS.Web.Controllers.Admin.Cms.Contents
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ContentsLayerImportController : ControllerBase
    {
        private const string Route = "cms/contents/contentsLayerImport";
        private const string RouteUpload = "cms/contents/contentsLayerImport/actions/upload";

        private readonly ICacheManager<CacheUtils.Process> _cacheManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ICreateManager _createManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IOldPluginManager _pluginManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;

        public ContentsLayerImportController(ICacheManager<CacheUtils.Process> cacheManager, IAuthManager authManager, IPathManager pathManager, ICreateManager createManager, IDatabaseManager databaseManager, IOldPluginManager pluginManager, ISiteRepository siteRepository, IChannelRepository channelRepository)
        {
            _cacheManager = cacheManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _createManager = createManager;
            _databaseManager = databaseManager;
            _pluginManager = pluginManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] ChannelRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.Contents) ||
                !await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, AuthTypes.ContentPermissions.Add))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channelInfo = await _channelRepository.GetAsync(request.ChannelId);
            if (channelInfo == null) return this.Error("无法确定内容对应的栏目");

            var (isChecked, checkedLevel) = await CheckManager.GetUserCheckLevelAsync(_authManager, site, request.SiteId);
            var checkedLevels = CheckManager.GetCheckedLevels(site, isChecked, checkedLevel, true);

            return new GetResult
            {
                Value = checkedLevel,
                CheckedLevels = checkedLevels
            };
        }

        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<UploadResult>> Upload([FromQuery] ChannelRequest request, [FromForm] IFormFile file)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.Contents) ||
                !await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, AuthTypes.ContentPermissions.Add))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            if (file == null)
            {
                return this.Error("请选择有效的文件上传");
            }

            var fileName = Path.GetFileName(file.FileName);

            var extendName = fileName.Substring(fileName.LastIndexOf(".", StringComparison.Ordinal));
            if (!StringUtils.EqualsIgnoreCase(extendName, ".zip") && !StringUtils.EqualsIgnoreCase(extendName, ".csv") && !StringUtils.EqualsIgnoreCase(extendName, ".txt"))
            {
                return this.Error("请选择有效的文件上传!");
            }

            var filePath = _pathManager.GetTemporaryFilesPath(fileName);

            await _pathManager.UploadAsync(file, filePath);

            var url = await _pathManager.GetSiteUrlByPhysicalPathAsync(site, filePath, true);

            return new UploadResult
            {
                Name = fileName,
                Url = url
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.Contents) ||
                !await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, AuthTypes.ContentPermissions.Add))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channelInfo = await _channelRepository.GetAsync(request.ChannelId);
            if (channelInfo == null) return this.Error("无法确定内容对应的栏目");

            var caching = new CacheUtils(_cacheManager);
            var isChecked = request.CheckedLevel >= site.CheckContentLevel;

            var contentIdList = new List<int>();

            var adminId = _authManager.AdminId;
            if (request.ImportType == "zip")
            {
                foreach (var fileName in request.FileNames)
                {
                    var localFilePath = _pathManager.GetTemporaryFilesPath(fileName);

                    if (!FileUtils.IsType(FileType.Zip, PathUtils.GetExtension(localFilePath)))
                        continue;

                    var importObject = new ImportObject(_pathManager, _pluginManager, _databaseManager, caching, site, adminId);
                    contentIdList.AddRange(await importObject.ImportContentsByZipFileAsync(channelInfo, localFilePath, request.IsOverride, isChecked, request.CheckedLevel, adminId, 0, SourceManager.Default));
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
                    contentIdList.AddRange(await importObject.ImportContentsByCsvFileAsync(channelInfo, localFilePath, request.IsOverride, isChecked, request.CheckedLevel, adminId, 0, SourceManager.Default));
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
                    contentIdList.AddRange(await importObject.ImportContentsByTxtFileAsync(channelInfo, localFilePath, request.IsOverride, isChecked, request.CheckedLevel, adminId, 0, SourceManager.Default));
                }
            }

            foreach (var contentId in contentIdList)
            {
                await _createManager.CreateContentAsync(request.SiteId, channelInfo.Id, contentId);
            }
            await _createManager.CreateChannelAsync(request.SiteId, channelInfo.Id);

            await _authManager.AddSiteLogAsync(request.SiteId, request.ChannelId, 0, "导入内容", string.Empty);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
