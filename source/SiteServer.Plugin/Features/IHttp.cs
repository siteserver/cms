using System.Web;

namespace SiteServer.Plugin.Features
{
    public interface IHttp : IPlugin
    {
        void HttpGet(HttpRequest request, HttpResponse response);
        void HttpGet(HttpRequest request, HttpResponse response, string name);
        void HttpGet(HttpRequest request, HttpResponse response, string name, int id);

        void HttpPost(HttpRequest request, HttpResponse response);
        void HttpPost(HttpRequest request, HttpResponse response, string name);
        void HttpPost(HttpRequest request, HttpResponse response, string name, int id);

        void HttpPut(HttpRequest request, HttpResponse response);
        void HttpPut(HttpRequest request, HttpResponse response, string name);
        void HttpPut(HttpRequest request, HttpResponse response, string name, int id);

        void HttpDelete(HttpRequest request, HttpResponse response);
        void HttpDelete(HttpRequest request, HttpResponse response, string name);
        void HttpDelete(HttpRequest request, HttpResponse response, string name, int id);

        void HttpPatch(HttpRequest request, HttpResponse response);
        void HttpPatch(HttpRequest request, HttpResponse response, string name);
        void HttpPatch(HttpRequest request, HttpResponse response, string name, int id);
    }
}
