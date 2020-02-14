using System.Web.Http;

namespace SiteServer.API.Controllers.Pages.Cms.Contents
{
    [RoutePrefix("pages/cms/contents/contentsCheck")]
    public partial class PagesContentsCheckController : ApiController
    {
        private const string RouteList = "actions/list";
        private const string RouteTree = "actions/tree";
        private const string RouteColumns = "actions/columns";
    }
}