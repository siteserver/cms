using Microsoft.AspNetCore.Http;
using SS.CMS.Plugin;

namespace SS.CMS.Api.Common
{
    public partial class Response : IResponse
    {
        private readonly HttpContext _context;

        public Response(HttpContext context)
        {
            _context = context;
        }

        public IResponseCookies Cookies => _context.Response.Cookies;
        public IHeaderDictionary Headers => _context.Response.Headers;
    }
}
