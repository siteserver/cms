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
    public partial class LayerPreviewVideoController : ControllerBase
    {
        private const string Route = "common/editor/layerPreviewVideo";

        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;

        public LayerPreviewVideoController(IPathManager pathManager, ISiteRepository siteRepository)
        {
            _pathManager = pathManager;
            _siteRepository = siteRepository;
        }

        public class GetResult
        {
            public string RootUrl { get; set; }
            public string SiteUrl { get; set; }
        }
    }
}
