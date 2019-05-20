using SiteServer.BackgroundPages.Common;
using System.Web.Http;

namespace SiteServer.API.Common
{
    public class ControllerBase : ApiController
    {
        //protected new Request Request => GetRequest();

        private Request _request;

        protected Request GetRequest()
        {
            if (_request != null) return _request;
            _request = BackgroundPages.Common.Request.Current;
            return _request;
        }
    }
}