using SiteServer.CMS.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Api.V1
{
    public class ORequest: AuthRequest
    {
        //{"select", "ID,Name"},
        //{"expand", "ProductDetail"},
        //{"filter", "Categories/any(d:d/ID gt 1)"},
        //{"orderby", "ID desc"},
        //{"top", "10"},
        //{"skip", "20"},
        //{"count", "true"},
        //{"search", "tom"}

        public ORequest(string scope) : base(scope)
        {
            
        }

        public OFilter Filter => OUtils.ParseFilter(QueryString["filter"]);

        public int Top
        {
            get
            {
                var top = TranslateUtils.ToInt(QueryString["top"], 20);
                return top > 0 ? top : 20;
            }
        }

        public int Skip => TranslateUtils.ToInt(QueryString["skip"]);

        public string Like => QueryString["like"];

        public string OrderBy => QueryString["orderBy"];

        public string RawUrl => HttpRequest.Url.AbsoluteUri;
    }
}
