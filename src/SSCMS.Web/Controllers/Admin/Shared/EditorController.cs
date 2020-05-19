using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Shared
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class EditorController : ControllerBase
    {
        private const string RouteActionsConfig = "shared/editor/actions/config";
        private const string RouteActionsUploadImage = "shared/editor/actions/uploadImage";
        private const string RouteActionsListImage = "shared/editor/actions/listImage";
        private const string RouteActionsUploadFile = "shared/editor/actions/uploadFile";
        private const string RouteActionsListFile = "shared/editor/actions/listFile";
        private const string RouteActionsUploadVideo = "shared/editor/actions/uploadVideo";
        private const string RouteActionsUploadScrawl = "shared/editor/actions/uploadScrawl";

        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;

        public EditorController(IPathManager pathManager, ISiteRepository siteRepository)
        {
            _pathManager = pathManager;
            _siteRepository = siteRepository;
        }
    }
}