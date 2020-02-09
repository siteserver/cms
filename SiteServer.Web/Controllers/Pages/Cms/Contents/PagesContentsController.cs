using System.Web.Http;

namespace SiteServer.API.Controllers.Pages.Cms.Contents
{
    [RoutePrefix("pages/cms/contents/contents")]
    public partial class PagesContentsController : ApiController
    {
        private const string RouteList = "actions/list";
        private const string RouteTree = "actions/tree";
        private const string RouteCreate = "actions/create";
        private const string RouteColumns = "actions/columns";
        private const string RouteAll = "actions/all";
    }
}
