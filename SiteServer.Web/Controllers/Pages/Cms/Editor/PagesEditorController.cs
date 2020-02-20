using System.Web.Http;
using SiteServer.Abstractions;

namespace SiteServer.API.Controllers.Pages.Cms.Editor
{
    [RoutePrefix("pages/cms/editor/editor")]
    public partial class PagesEditorController : ApiController
    {
        private const string Route = "";
        private const string RoutePreview = "actions/preview";

        private readonly ICreateManager _createManager;

        public PagesEditorController(ICreateManager createManager)
        {
            _createManager = createManager;
        }
    }
}
