using SiteServer.CMS.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Api.V1
{
    public class ORequest
    {
        public AuthRequest AuthRequest { get; }

        //{"select", "ID,Name"},
        //{"expand", "ProductDetail"},
        //{"filter", "Categories/any(d:d/ID gt 1)"},
        //{"orderby", "ID desc"},
        //{"top", "10"},
        //{"skip", "20"},
        //{"count", "true"},
        //{"search", "tom"}

        public ORequest(string scope)
        {
            AuthRequest = new AuthRequest(scope);
        }

        public OFilter Filter => OUtils.ParseFilter(AuthRequest.QueryString["filter"]);

        public int Top
        {
            get
            {
                var top = TranslateUtils.ToInt(AuthRequest.QueryString["top"], 20);
                return top > 0 ? top : 20;
            }
        }

        public int Skip => TranslateUtils.ToInt(AuthRequest.QueryString["skip"]);

        public bool Count => TranslateUtils.ToBool(AuthRequest.QueryString["count"]);

        public string RawUrl => AuthRequest.HttpRequest.Url.AbsoluteUri;

        public bool IsApiAuthorized => AuthRequest.IsApiAuthorized;
    }
}
