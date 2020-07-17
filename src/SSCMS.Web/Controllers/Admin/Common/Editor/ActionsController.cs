using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Common.Editor
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ActionsController : ControllerBase
    {
        private const string RouteActionsConfig = "common/editor/actions/config";
        private const string RouteActionsUploadImage = "common/editor/actions/uploadImage";
        private const string RouteActionsListImage = "common/editor/actions/listImage";
        private const string RouteActionsUploadFile = "common/editor/actions/uploadFile";
        private const string RouteActionsListFile = "common/editor/actions/listFile";
        private const string RouteActionsUploadVideo = "common/editor/actions/uploadVideo";
        private const string RouteActionsUploadScrawl = "common/editor/actions/uploadScrawl";

        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;

        public ActionsController(IPathManager pathManager, ISiteRepository siteRepository)
        {
            _pathManager = pathManager;
            _siteRepository = siteRepository;
        }
    }
}