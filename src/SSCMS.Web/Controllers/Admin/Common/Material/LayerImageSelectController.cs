using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Extensions;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Common.Material
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LayerImageSelectController : ControllerBase
    {
        private const string Route = "common/material/layerImageSelect";
        private const string RouteSelect = "common/material/layerImageSelect/actions/select";

        private readonly ISettingsManager _settingsManager;
        private readonly IPathManager _pathManager;
        private readonly IMaterialGroupRepository _materialGroupRepository;
        private readonly IMaterialImageRepository _materialImageRepository;
        private readonly ISiteRepository _siteRepository;

        public LayerImageSelectController(ISettingsManager settingsManager, IPathManager pathManager, IMaterialGroupRepository materialGroupRepository, IMaterialImageRepository materialImageRepository, ISiteRepository siteRepository)
        {
            _settingsManager = settingsManager;
            _pathManager = pathManager;
            _materialGroupRepository = materialGroupRepository;
            _materialImageRepository = materialImageRepository;
            _siteRepository = siteRepository;
        }


        [HttpPost, Route(Route)]
        public async Task<ActionResult<QueryResult>> List([FromBody] QueryRequest request)
        {
            var groups = await _materialGroupRepository.GetAllAsync(MaterialType.Image);
            var count = await _materialImageRepository.GetCountAsync(request.GroupId, request.Keyword);
            var items = await _materialImageRepository.GetAllAsync(request.GroupId, request.Keyword, request.Page, request.PerPage);

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
            var image = await _materialImageRepository.GetAsync(request.LibraryId);

            var materialFilePath = PathUtils.Combine(_settingsManager.WebRootPath, image.Url);
            if (!FileUtils.IsFileExists(materialFilePath))
            {
                return this.Error("图片文件不存在，请重新选择");
            }

            var localDirectoryPath = await _pathManager.GetUploadDirectoryPathAsync(site, UploadType.Image);
            var filePath = PathUtils.Combine(localDirectoryPath, _pathManager.GetUploadFileName(site, materialFilePath));

            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            FileUtils.CopyFile(materialFilePath, filePath);

            var imageUrl = await _pathManager.GetVirtualUrlByPhysicalPathAsync(site, filePath);

            return new StringResult
            {
                Value = imageUrl
            };
        }
    }
}
