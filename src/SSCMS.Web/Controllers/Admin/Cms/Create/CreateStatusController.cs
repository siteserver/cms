using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Cms.Create
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class CreateStatusController : ControllerBase
    {
        private const string Route = "cms/create/createStatus";
        private const string RouteActionsCancel = "cms/create/createStatus/actions/cancel";

        private readonly ICreateManager _createManager;

        public CreateStatusController(ICreateManager createManager)
        {
            _createManager = createManager;
        }
    }
}