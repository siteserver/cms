using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Common.Form
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LayerFilePreviewController : ControllerBase
    {
        private const string Route = "common/form/layerFilePreview";

        private readonly IPathManager _pathManager;

        public LayerFilePreviewController(
            IPathManager pathManager
        )
        {
            _pathManager = pathManager;
        }

        public class GetRequest
        {
            public bool Localhost { get; set; }
            public string FileUrl { get; set; }
        }

        public class FileInfo
        {
            public string Name { get; set; }
            public string Size { get; set; }
        }

        public class GetResult
        {
            public bool IsImage { get; set;}
            public bool IsVideo { get; set;}
            public bool IsAudio { get; set;}
            public bool IsOffice { get; set; }
            public bool IsPdf { get; set; }
            public string Url { get; set; }
            public FileInfo File { get; set; }
        }
    }
}
