using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Common.Form
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LayerFileUploadController : ControllerBase
    {
        private const string Route = "common/form/layerFileUpload";
        private const string RouteUpload = "common/form/layerFileUpload/actions/upload";

        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IMaterialFileRepository _materialFileRepository;

        public LayerFileUploadController(IPathManager pathManager, ISiteRepository siteRepository, IMaterialFileRepository materialFileRepository)
        {
            _pathManager = pathManager;
            _siteRepository = siteRepository;
            _materialFileRepository = materialFileRepository;
        }

        public class Options
        {
            public bool IsChangeFileName { get; set; }
            public bool IsLibrary { get; set; }
        }

        public class UploadRequest : SiteRequest
        {
            public bool IsChangeFileName { get; set; }
        }

        public class UploadResult
        {
            public string Name { get; set; }
            public string Path { get; set; }
        }

        public class SubmitRequest : Options
        {
            public int SiteId { get; set; }
            public List<string> FilePaths { get; set; }
        }

        public class SubmitResult
        {
            public string FileUrl { get; set; }
            public string FileVirtualUrl { get; set; }
        }
    }
}
