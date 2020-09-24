using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Common.Editor
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LayerVideoController : ControllerBase
    {
        private const string RouteUploadVideo = "common/editor/layerVideo/actions/uploadVideo";
        private const string RouteUploadImage = "common/editor/layerVideo/actions/uploadImage";

        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;

        public LayerVideoController(IPathManager pathManager, ISiteRepository siteRepository)
        {
            _pathManager = pathManager;
            _siteRepository = siteRepository;
        }

        public class UploadResult
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public string Url { get; set; }
        }
    }
}
