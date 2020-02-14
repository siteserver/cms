using System.Web.Http;

namespace SiteServer.API.Controllers.Pages.Cms.Contents
{
    [RoutePrefix("pages/cms/contents/contentsRecycle")]
    public partial class PagesContentsRecycleController : ApiController
    {
        private const string Route = "";
        private const string RouteList = "actions/list";
        private const string RouteTree = "actions/tree";
        private const string RouteColumns = "actions/columns";
    }
}