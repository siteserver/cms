using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Common.Form
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LayerImageUploadController : ControllerBase
    {
        private const string Route = "common/form/layerImageUpload";
        private const string RouteUpload = "common/form/layerImageUpload/actions/upload";

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
            public bool IsMaterial { get; set; }
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
            public string ImageVirtualUrl { get; set; }
            public string PreviewUrl { get; set; }
            public string PreviewVirtualUrl { get; set; }
        }
    }
}
