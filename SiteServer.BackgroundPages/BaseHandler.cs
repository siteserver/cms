using System.Web;
using SiteServer.CMS.Core;
using SiteServer.Abstractions;

namespace SiteServer.BackgroundPages
{
    public abstract class BaseHandler : IHttpHandler
    {
        protected AuthenticatedRequest AuthRequest { get; private set; }

        public void ProcessRequest(HttpContext context)
        {
            AuthRequest = AuthenticatedRequest.GetAuthAsync().GetAwaiter().GetResult();

            if (!AuthRequest.IsAdminLoggin) return;

            Finish(Process());
        }

        protected abstract object Process();

        protected void Finish(object retval)
        {
            var response = HttpContext.Current.Response;

            response.ContentType = "application/json";
            response.Write(TranslateUtils.JsonSerialize(retval));
            response.End();
        }

        public bool IsReusable => false;
    }
}
