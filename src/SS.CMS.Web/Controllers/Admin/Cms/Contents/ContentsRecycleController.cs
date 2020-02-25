using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;

namespace SS.CMS.Web.Controllers.Admin.Cms.Contents
{
    [Route("admin/cms/contents/contentsRecycle")]
    public partial class ContentsRecycleController : ControllerBase
    {
        private const string Route = "";
        private const string RouteList = "actions/list";
        private const string RouteTree = "actions/tree";
        private const string RouteColumns = "actions/columns";

        private readonly IAuthManager _authManager;

        public ContentsRecycleController(IAuthManager authManager)
        {
            _authManager = authManager;
        }
    }
}