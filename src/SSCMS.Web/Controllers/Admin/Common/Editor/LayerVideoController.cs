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
        private const string Route = "common/editor/layerVideo";
        private const string RouteUploadVideo = "common/editor/layerVideo/actions/uploadVideo";
        private const string RouteUploadImage = "common/editor/layerVideo/actions/uploadImage";

        private readonly IPathManager _pathManager;

        private readonly IVodManager _vodManager;
        private readonly IStorageManager _storageManager;
        private readonly ISiteRepository _siteRepository;

        public LayerVideoController(IPathManager pathManager, IVodManager vodManager, IStorageManager storageManager, ISiteRepository siteRepository)
        {
            _pathManager = pathManager;
            _vodManager = vodManager;
            _storageManager = storageManager;
            _siteRepository = siteRepository;
        }

        public class GetResult
        {
            public string RootUrl { get; set; }
            public string SiteUrl { get; set; }
            public bool IsCloudVod { get; set; }
        }

        public class UploadResult
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public string Url { get; set; }
            public string CoverUrl { get; set; }
        }
    }
}
