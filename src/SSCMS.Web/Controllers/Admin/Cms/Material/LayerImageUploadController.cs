using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LayerImageUploadController : ControllerBase
    {
        private const string Route = "cms/material/layerImageUpload";
        private const string RouteUpload = "cms/material/layerImageUpload/actions/upload";

        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IMaterialImageRepository _materialImageRepository;

        public LayerImageUploadController(IPathManager pathManager, ISiteRepository siteRepository, IMaterialImageRepository materialImageRepository)
        {
            _pathManager = pathManager;
            _siteRepository = siteRepository;
            _materialImageRepository = materialImageRepository;
        }

        public class Options
        {
            public bool IsEditor { get; set; }
            public bool IsLibrary { get; set; }
            public bool IsThumb { get; set; }
            public int ThumbWidth { get; set; }
            public int ThumbHeight { get; set; }
            public bool IsLinkToOriginal { get; set; }
        }

        public class SubmitRequest : Options
        {
            public int SiteId { get; set; }
            public List<string> FilePaths { get; set; }
        }

        public class UploadResult
        {
            public string Name { get; set; }
            public string Path { get; set; }
        }

        public class SubmitResult
        {
            public string ImageUrl { get; set; }
            public string PreviewUrl { get; set; }
        }
    }
}
