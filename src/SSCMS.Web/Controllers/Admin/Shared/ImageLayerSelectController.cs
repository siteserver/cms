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
    public partial class ImageLayerSelectController : ControllerBase
    {
        private const string Route = "shared/imageLayerSelect";
        private const string RouteSelect = "shared/imageLayerSelect/actions/select";

        private readonly ISettingsManager _settingsManager;
        private readonly IPathManager _pathManager;
        private readonly ILibraryGroupRepository _libraryGroupRepository;
        private readonly ILibraryImageRepository _libraryImageRepository;
        private readonly ISiteRepository _siteRepository;

        public ImageLayerSelectController(ISettingsManager settingsManager, IPathManager pathManager, ILibraryGroupRepository libraryGroupRepository, ILibraryImageRepository libraryImageRepository, ISiteRepository siteRepository)
        {
            _settingsManager = settingsManager;
            _pathManager = pathManager;
            _libraryGroupRepository = libraryGroupRepository;
            _libraryImageRepository = libraryImageRepository;
            _siteRepository = siteRepository;
        }


        [HttpGet, Route(Route)]
        public async Task<ActionResult<QueryResult>> List([FromQuery]QueryRequest request)
        {
            var groups = await _libraryGroupRepository.GetAllAsync(LibraryType.Image);
            groups.Insert(0, new LibraryGroup
            {
                Id = 0,
                GroupName = "全部图片"
            });
            var count = await _libraryImageRepository.GetCountAsync(request.GroupId, request.Keyword);
            var items = await _libraryImageRepository.GetAllAsync(request.GroupId, request.Keyword, request.Page, request.PerPage);

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
            var library = await _libraryImageRepository.GetAsync(request.LibraryId);

            var libraryFilePath = PathUtils.Combine(_settingsManager.WebRootPath, library.Url);
            if (!FileUtils.IsFileExists(libraryFilePath))
            {
                return this.Error("图片文件不存在，请重新选择");
            }

            var localDirectoryPath = await _pathManager.GetUploadDirectoryPathAsync(site, UploadType.Image);
            var filePath = PathUtils.Combine(localDirectoryPath, _pathManager.GetUploadFileName(site, libraryFilePath));

            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            FileUtils.CopyFile(libraryFilePath, filePath);

            var imageUrl = await _pathManager.GetSiteUrlByPhysicalPathAsync(site, filePath, true);

            return new StringResult
            {
                Value = imageUrl
            };
        }
    }
}
