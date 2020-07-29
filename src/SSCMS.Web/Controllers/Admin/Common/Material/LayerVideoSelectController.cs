using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Extensions;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Common.Material
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LayerVideoSelectController : ControllerBase
    {
        private const string Route = "common/material/layerVideoSelect";
        private const string RouteSelect = "common/material/layerVideoSelect/actions/select";

        private readonly ISettingsManager _settingsManager;
        private readonly IPathManager _pathManager;
        private readonly IMaterialGroupRepository _materialGroupRepository;
        private readonly IMaterialVideoRepository _materialVideoRepository;
        private readonly ISiteRepository _siteRepository;

        public LayerVideoSelectController(ISettingsManager settingsManager, IPathManager pathManager, IMaterialGroupRepository materialGroupRepository, IMaterialVideoRepository materialVideoRepository, ISiteRepository siteRepository)
        {
            _settingsManager = settingsManager;
            _pathManager = pathManager;
            _materialGroupRepository = materialGroupRepository;
            _materialVideoRepository = materialVideoRepository;
            _siteRepository = siteRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<QueryResult>> List([FromQuery]QueryRequest request)
        {
            var groups = await _materialGroupRepository.GetAllAsync(MaterialType.Video);
            var count = await _materialVideoRepository.GetCountAsync(request.GroupId, request.Keyword);
            var items = await _materialVideoRepository.GetAllAsync(request.GroupId, request.Keyword, request.Page, request.PerPage);

            return new QueryResult
            {
                Groups = groups,
                Count = count,
                Items = items
            };
        }

        [HttpPost, Route(RouteSelect)]
        public async Task<ActionResult<StringResult>> Select([FromBody]SelectRequest request)
        {
            var site = await _siteRepository.GetAsync(request.SiteId);
            var video = await _materialVideoRepository.GetAsync(request.LibraryId);

            var materialFilePath = PathUtils.Combine(_settingsManager.WebRootPath, video.Url);
            if (!FileUtils.IsFileExists(materialFilePath))
            {
                return this.Error("视频不存在，请重新选择");
            }

            var localDirectoryPath = await _pathManager.GetUploadDirectoryPathAsync(site, UploadType.Video);
            var filePath = PathUtils.Combine(localDirectoryPath, _pathManager.GetUploadFileName(site, materialFilePath));

            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            FileUtils.CopyFile(materialFilePath, filePath);

            var fileUrl = await _pathManager.GetVirtualUrlByPhysicalPathAsync(site, filePath);

            return new StringResult
            {
                Value = fileUrl
            };
        }
    }
}
