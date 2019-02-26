using System.Web;
using SiteServer.Utils;
using SiteServer.CMS.Plugin.Impl;

namespace SiteServer.BackgroundPages
{
    public abstract class BaseHandler : IHttpHandler
    {
#pragma warning disable CS0612 // '“RequestImpl”已过时
        protected RequestImpl AuthRequest { get; private set; }
#pragma warning restore CS0612 // '“RequestImpl”已过时

        public void ProcessRequest(HttpContext context)
        {
#pragma warning disable CS0612 // '“RequestImpl”已过时
            AuthRequest = new RequestImpl(context.Request);
#pragma warning restore CS0612 // '“RequestImpl”已过时

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
