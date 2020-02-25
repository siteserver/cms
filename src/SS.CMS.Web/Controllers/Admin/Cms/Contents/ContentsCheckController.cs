using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;

namespace SS.CMS.Web.Controllers.Admin.Cms.Contents
{
    [Route("admin/cms/contents/contentsCheck")]
    public partial class ContentsCheckController : ControllerBase
    {
        private const string RouteList = "actions/list";
        private const string RouteTree = "actions/tree";
        private const string RouteColumns = "actions/columns";

        private readonly IAuthManager _authManager;

        public ContentsCheckController(IAuthManager authManager)
        {
            _authManager = authManager;
        }
    }
}