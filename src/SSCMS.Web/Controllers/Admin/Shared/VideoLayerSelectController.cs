using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Core.Extensions;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Shared
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class VideoLayerSelectController : ControllerBase
    {
        private const string Route = "shared/videoLayerSelect";
        private const string RouteSelect = "shared/videoLayerSelect/actions/select";

        private readonly ISettingsManager _settingsManager;
        private readonly IPathManager _pathManager;
        private readonly ILibraryGroupRepository _libraryGroupRepository;
        private readonly ILibraryVideoRepository _libraryVideoRepository;
        private readonly ISiteRepository _siteRepository;

        public VideoLayerSelectController(ISettingsManager settingsManager, IPathManager pathManager, ILibraryGroupRepository libraryGroupRepository, ILibraryVideoRepository libraryVideoRepository, ISiteRepository siteRepository)
        {
            _settingsManager = settingsManager;
            _pathManager = pathManager;
            _libraryGroupRepository = libraryGroupRepository;
            _libraryVideoRepository = libraryVideoRepository;
            _siteRepository = siteRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<QueryResult>> List([FromQuery]QueryRequest request)
        {
            var groups = await _libraryGroupRepository.GetAllAsync(LibraryType.Video);
            groups.Insert(0, new LibraryGroup
            {
                Id = 0,
                GroupName = "全部视频"
            });
            var count = await _libraryVideoRepository.GetCountAsync(request.GroupId, request.Keyword);
            var items = await _libraryVideoRepository.GetAllAsync(request.GroupId, request.Keyword, request.Page, request.PerPage);

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
            var library = await _libraryVideoRepository.GetAsync(request.LibraryId);

            var libraryFilePath = PathUtils.Combine(_settingsManager.WebRootPath, library.Url);
            if (!FileUtils.IsFileExists(libraryFilePath))
            {
                return this.Error("视频不存在，请重新选择");
            }

            var localDirectoryPath = await _pathManager.GetUploadDirectoryPathAsync(site, UploadType.Video);
            var filePath = PathUtils.Combine(localDirectoryPath, _pathManager.GetUploadFileName(site, libraryFilePath));

            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            FileUtils.CopyFile(libraryFilePath, filePath);

            var fileUrl = await _pathManager.GetSiteUrlByPhysicalPathAsync(site, filePath, true);

            return new StringResult
            {
                Value = fileUrl
            };
        }
    }
}
