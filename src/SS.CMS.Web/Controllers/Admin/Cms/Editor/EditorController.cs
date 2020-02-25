using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;

namespace SS.CMS.Web.Controllers.Admin.Cms.Editor
{
    [Route("admin/cms/editor/editor")]
    public partial class EditorController : ControllerBase
    {
        private const string Route = "";
        private const string RoutePreview = "actions/preview";

        private readonly IAuthManager _authManager;
        private readonly ICreateManager _createManager;

        public EditorController(IAuthManager authManager, ICreateManager createManager)
        {
            _authManager = authManager;
            _createManager = createManager;
        }
    }
}
