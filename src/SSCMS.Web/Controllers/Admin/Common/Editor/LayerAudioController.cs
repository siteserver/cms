using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Extensions;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Common.Editor
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LayerAudioController : ControllerBase
    {
        private const string RouteUpload = "common/editor/layerAudio/actions/upload";

        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;

        public LayerAudioController(IPathManager pathManager, ISiteRepository siteRepository)
        {
            _pathManager = pathManager;
            _siteRepository = siteRepository;
        }

        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<UploadResult>> Upload([FromQuery] SiteRequest request, [FromForm] IFormFile file)
        {
            var site = await _siteRepository.GetAsync(request.SiteId);

            if (file == null)
            {
                return this.Error("请选择有效的文件上传");
            }

            var fileName = Path.GetFileName(file.FileName);

            if (!PathUtils.IsExtension(PathUtils.GetExtension(fileName), ".mp3"))
            {
                return this.Error("文件只能是音频格式，请选择有效的文件上传!");
            }

            var localDirectoryPath = await _pathManager.GetUploadDirectoryPathAsync(site, UploadType.Audio);
            var filePath = PathUtils.Combine(localDirectoryPath, _pathManager.GetUploadFileName(site, fileName));

            await _pathManager.UploadAsync(file, filePath);

            var imageUrl = await _pathManager.GetSiteUrlByPhysicalPathAsync(site, filePath, true);

            return new UploadResult
            {
                Name = fileName,
                Path = filePath,
                Url = imageUrl
            };
        }
    }
}
