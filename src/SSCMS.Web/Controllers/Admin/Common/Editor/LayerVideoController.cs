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
        private readonly IMaterialVideoRepository _materialVideoRepository;

        public LayerVideoController(IPathManager pathManager, IVodManager vodManager, IStorageManager storageManager, ISiteRepository siteRepository, IMaterialVideoRepository materialVideoRepository)
        {
            _pathManager = pathManager;
            _vodManager = vodManager;
            _storageManager = storageManager;
            _siteRepository = siteRepository;
            _materialVideoRepository = materialVideoRepository;
        }

        public class Options
        {
            public bool IsChangeFileName { get; set; }
            public bool IsLibrary { get; set; }
        }

        public class GetResult : Options
        {
            public string RootUrl { get; set; }
            public string SiteUrl { get; set; }
            public bool IsCloudVod { get; set; }
            public string VideoUploadExtensions { get; set; }
        }

        public class UploadRequest : Options
        {
            public int SiteId { get; set; }
        }

        public class UploadImageResult
        {
            public string ImageUrl { get; set; }
            public string VirtualUrl { get; set; }
        }

        public class UploadVideoResult
        {
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
            public string CoverUrl { get; set; }
            public string PlayUrl { get; set; }
            public string VirtualUrl { get; set; }
        }
    }
}
