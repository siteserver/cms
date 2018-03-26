using System.Web;
using SiteServer.Utils;
using SiteServer.CMS.Plugin;

namespace SiteServer.BackgroundPages
{
    public abstract class BaseHandler : IHttpHandler
    {
        protected AuthRequest AuthRequest { get; private set; }

        public void ProcessRequest(HttpContext context)
        {
            AuthRequest = new AuthRequest(context.Request);

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
