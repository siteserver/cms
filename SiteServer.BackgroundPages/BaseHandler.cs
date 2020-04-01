using System.Web;
using SiteServer.CMS.Core;
using SiteServer.Utils;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Plugin.Impl;

namespace SiteServer.BackgroundPages
{
    public abstract class BaseHandler : IHttpHandler
    {
        protected AuthenticatedRequest AuthRequest { get; private set; }

        public void ProcessRequest(HttpContext context)
        {
            AuthRequest = new AuthenticatedRequest(context.Request);

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
