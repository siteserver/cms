using System;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Owin;
using SiteServer.Plugin;
using HttpContext = System.Web.HttpContext;
using IHeaderDictionary = Microsoft.AspNetCore.Http.IHeaderDictionary;

namespace SiteServer.BackgroundPages.Common
{
    public partial class Response : IResponse
    {
        public Response(HttpContext httpContext, IOwinContext owinContext)
        {
            Cookies = new ResponseCookies(httpContext.Response.Cookies);
            Headers = new HeaderDictionary(owinContext.Response.Headers);
        }

        public static Response Current
        {
            get
            {
                var httpContext = HttpContext.Current;
                if (httpContext == null) return null;

                var owinContext = httpContext.GetOwinContext();
                var response = owinContext.Get<Response>("SiteServer.BackgroundPages.Common.Response");
                if (response != null) return response;

                response = new Response(httpContext, owinContext);
                owinContext.Set("SiteServer.BackgroundPages.Common.Response", response);
                return response;
            }
        }

        public IResponseCookies Cookies { get; }

        public IHeaderDictionary Headers { get; }
    }
}
