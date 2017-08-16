using System.Web;
using System.Web.Http;
using SiteServer.CMS.Controllers.Plugins;
using SiteServer.CMS.Plugin;
using SiteServer.Plugin.Features;

namespace SiteServer.API.Controllers.Plugins
{
    [RoutePrefix("api")]
    public class HttpController : ApiController
    {
        [HttpGet, Route(Http.Route)]
        public void Get(string pluginId)
        {
            var http = PluginCache.GetEnabledFeature<IHttp>(pluginId);
            http?.HttpGet(HttpContext.Current.Request, HttpContext.Current.Response);
        }

        [HttpGet, Route(Http.RouteName)]
        public void Get(string pluginId, string name)
        {
            var http = PluginCache.GetEnabledFeature<IHttp>(pluginId);
            http?.HttpGet(HttpContext.Current.Request, HttpContext.Current.Response, name);
        }

        [HttpGet, Route(Http.RouteNameAndId)]
        public void Get(string pluginId, string name, int id)
        {
            var http = PluginCache.GetEnabledFeature<IHttp>(pluginId);
            http?.HttpGet(HttpContext.Current.Request, HttpContext.Current.Response, name, id);
        }

        [HttpPost, Route(Http.Route)]
        public void Post(string pluginId)
        {
            var http = PluginCache.GetEnabledFeature<IHttp>(pluginId);
            http?.HttpPost(HttpContext.Current.Request, HttpContext.Current.Response);
        }

        [HttpPost, Route(Http.RouteName)]
        public void Post(string pluginId, string name)
        {
            var http = PluginCache.GetEnabledFeature<IHttp>(pluginId);
            http?.HttpPost(HttpContext.Current.Request, HttpContext.Current.Response, name);
        }

        [HttpPost, Route(Http.RouteNameAndId)]
        public void Post(string pluginId, string name, int id)
        {
            var http = PluginCache.GetEnabledFeature<IHttp>(pluginId);
            http?.HttpPost(HttpContext.Current.Request, HttpContext.Current.Response, name, id);
        }

        [HttpPut, Route(Http.Route)]
        public void Put(string pluginId)
        {
            var http = PluginCache.GetEnabledFeature<IHttp>(pluginId);
            http?.HttpPut(HttpContext.Current.Request, HttpContext.Current.Response);
        }

        [HttpPut, Route(Http.RouteName)]
        public void Put(string pluginId, string name)
        {
            var http = PluginCache.GetEnabledFeature<IHttp>(pluginId);
            http?.HttpPut(HttpContext.Current.Request, HttpContext.Current.Response, name);
        }

        [HttpPut, Route(Http.RouteNameAndId)]
        public void Put(string pluginId, string name, int id)
        {
            var http = PluginCache.GetEnabledFeature<IHttp>(pluginId);
            http?.HttpPut(HttpContext.Current.Request, HttpContext.Current.Response, name, id);
        }

        [HttpDelete, Route(Http.Route)]
        public void Delete(string pluginId)
        {
            var http = PluginCache.GetEnabledFeature<IHttp>(pluginId);
            http?.HttpDelete(HttpContext.Current.Request, HttpContext.Current.Response);
        }

        [HttpDelete, Route(Http.RouteName)]
        public void Delete(string pluginId, string name)
        {
            var http = PluginCache.GetEnabledFeature<IHttp>(pluginId);
            http?.HttpDelete(HttpContext.Current.Request, HttpContext.Current.Response, name);
        }

        [HttpDelete, Route(Http.RouteNameAndId)]
        public void Delete(string pluginId, string name, int id)
        {
            var http = PluginCache.GetEnabledFeature<IHttp>(pluginId);
            http?.HttpDelete(HttpContext.Current.Request, HttpContext.Current.Response, name, id);
        }

        [HttpPatch, Route(Http.Route)]
        public void Patch(string pluginId)
        {
            var http = PluginCache.GetEnabledFeature<IHttp>(pluginId);
            http?.HttpPatch(HttpContext.Current.Request, HttpContext.Current.Response);
        }

        [HttpPatch, Route(Http.RouteName)]
        public void Patch(string pluginId, string name)
        {
            var http = PluginCache.GetEnabledFeature<IHttp>(pluginId);
            http?.HttpPatch(HttpContext.Current.Request, HttpContext.Current.Response, name);
        }

        [HttpPatch, Route(Http.RouteNameAndId)]
        public void Patch(string pluginId, string name, int id)
        {
            var http = PluginCache.GetEnabledFeature<IHttp>(pluginId);
            http?.HttpPatch(HttpContext.Current.Request, HttpContext.Current.Response, name, id);
        }
    }
}
