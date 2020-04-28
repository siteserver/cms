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
    public partial class FileLayerSelectController : ControllerBase
    {
        private const string Route = "shared/fileLayerSelect";
        private const string RouteSelect = "shared/fileLayerSelect/actions/select";

        private readonly ISettingsManager _settingsManager;
        private readonly IPathManager _pathManager;
        private readonly ILibraryGroupRepository _libraryGroupRepository;
        private readonly ILibraryFileRepository _libraryFileRepository;
        private readonly ISiteRepository _siteRepository;

        public FileLayerSelectController(ISettingsManager settingsManager, IPathManager pathManager, ILibraryGroupRepository libraryGroupRepository, ILibraryFileRepository libraryFileRepository, ISiteRepository siteRepository)
        {
            _settingsManager = settingsManager;
            _pathManager = pathManager;
            _libraryGroupRepository = libraryGroupRepository;
            _libraryFileRepository = libraryFileRepository;
            _siteRepository = siteRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<QueryResult>> List([FromQuery]QueryRequest request)
        {
            var groups = await _libraryGroupRepository.GetAllAsync(LibraryType.File);
            groups.Insert(0, new LibraryGroup
            {
                Id = 0,
                GroupName = "全部文件"
            });
            var count = await _libraryFileRepository.GetCountAsync(request.GroupId, request.Keyword);
            var items = await _libraryFileRepository.GetAllAsync(request.GroupId, request.Keyword, request.Page, request.PerPage);

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
            var library = await _libraryFileRepository.GetAsync(request.LibraryId);

            var libraryFilePath = PathUtils.Combine(_settingsManager.WebRootPath, library.Url);
            if (!FileUtils.IsFileExists(libraryFilePath))
            {
                return this.Error("文件不存在，请重新选择");
            }

            var localDirectoryPath = await _pathManager.GetUploadDirectoryPathAsync(site, UploadType.File);
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
