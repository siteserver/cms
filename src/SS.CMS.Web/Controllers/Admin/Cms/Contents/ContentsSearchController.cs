using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;

namespace SS.CMS.Web.Controllers.Admin.Cms.Contents
{
    [Route("admin/cms/contents/contentsSearch")]
    public partial class ContentsSearchController : ControllerBase
    {
        private const string RouteList = "actions/list";
        private const string RouteTree = "actions/tree";
        private const string RouteCreate = "actions/create";
        private const string RouteColumns = "actions/columns";

        private readonly IAuthManager _authManager;
        private readonly ICreateManager _createManager;

        public ContentsSearchController(IAuthManager authManager, ICreateManager createManager)
        {
            _authManager = authManager;
            _createManager = createManager;
        }
    }
}
