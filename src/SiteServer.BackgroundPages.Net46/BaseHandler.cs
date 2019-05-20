using System.Web;
using SiteServer.CMS.Core;
using SiteServer.Utils;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.BackgroundPages.Common;

namespace SiteServer.BackgroundPages
{
    public abstract class BaseHandler : IHttpHandler
    {
        protected Request AuthRequest { get; private set; }

        public void ProcessRequest(HttpContext context)
        {
            AuthRequest = Request.Current;

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
