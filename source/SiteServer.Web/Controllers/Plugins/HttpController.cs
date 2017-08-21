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
        [HttpGet, Route(HttpApi.Route)]
        public void Get(string pluginId)
        {
            var httpApi = PluginCache.GetEnabledFeature<IHttpApi>(pluginId);
            httpApi?.HttpGet?.Invoke(HttpContext.Current.Request, HttpContext.Current.Response);
        }

        [HttpGet, Route(HttpApi.RouteName)]
        public void Get(string pluginId, string name)
        {
            var httpApi = PluginCache.GetEnabledFeature<IHttpApi>(pluginId);
            httpApi?.HttpGetWithName?.Invoke(HttpContext.Current.Request, HttpContext.Current.Response, name);
        }

        [HttpGet, Route(HttpApi.RouteNameAndId)]
        public void Get(string pluginId, string name, int id)
        {
            var httpApi = PluginCache.GetEnabledFeature<IHttpApi>(pluginId);
            httpApi?.HttpGetWithNameAndId?.Invoke(HttpContext.Current.Request, HttpContext.Current.Response, name, id);
        }

        [HttpPost, Route(HttpApi.Route)]
        public void Post(string pluginId)
        {
            var httpApi = PluginCache.GetEnabledFeature<IHttpApi>(pluginId);
            httpApi?.HttpPost?.Invoke(HttpContext.Current.Request, HttpContext.Current.Response);
        }

        [HttpPost, Route(HttpApi.RouteName)]
        public void Post(string pluginId, string name)
        {
            var httpApi = PluginCache.GetEnabledFeature<IHttpApi>(pluginId);
            httpApi?.HttpPostWithName?.Invoke(HttpContext.Current.Request, HttpContext.Current.Response, name);
        }

        [HttpPost, Route(HttpApi.RouteNameAndId)]
        public void Post(string pluginId, string name, int id)
        {
            var httpApi = PluginCache.GetEnabledFeature<IHttpApi>(pluginId);
            httpApi?.HttpPostWithNameAndId?.Invoke(HttpContext.Current.Request, HttpContext.Current.Response, name, id);
        }

        [HttpPut, Route(HttpApi.Route)]
        public void Put(string pluginId)
        {
            var httpApi = PluginCache.GetEnabledFeature<IHttpApi>(pluginId);
            httpApi?.HttpPut?.Invoke(HttpContext.Current.Request, HttpContext.Current.Response);
        }

        [HttpPut, Route(HttpApi.RouteName)]
        public void Put(string pluginId, string name)
        {
            var httpApi = PluginCache.GetEnabledFeature<IHttpApi>(pluginId);
            httpApi?.HttpPutWithName?.Invoke(HttpContext.Current.Request, HttpContext.Current.Response, name);
        }

        [HttpPut, Route(HttpApi.RouteNameAndId)]
        public void Put(string pluginId, string name, int id)
        {
            var httpApi = PluginCache.GetEnabledFeature<IHttpApi>(pluginId);
            httpApi?.HttpPutWithNameAndId?.Invoke(HttpContext.Current.Request, HttpContext.Current.Response, name, id);
        }

        [HttpDelete, Route(HttpApi.Route)]
        public void Delete(string pluginId)
        {
            var httpApi = PluginCache.GetEnabledFeature<IHttpApi>(pluginId);
            httpApi?.HttpDelete?.Invoke(HttpContext.Current.Request, HttpContext.Current.Response);
        }

        [HttpDelete, Route(HttpApi.RouteName)]
        public void Delete(string pluginId, string name)
        {
            var httpApi = PluginCache.GetEnabledFeature<IHttpApi>(pluginId);
            httpApi?.HttpDeleteWithName?.Invoke(HttpContext.Current.Request, HttpContext.Current.Response, name);
        }

        [HttpDelete, Route(HttpApi.RouteNameAndId)]
        public void Delete(string pluginId, string name, int id)
        {
            var httpApi = PluginCache.GetEnabledFeature<IHttpApi>(pluginId);
            httpApi?.HttpDeleteWithNameAndId?.Invoke(HttpContext.Current.Request, HttpContext.Current.Response, name, id);
        }

        [HttpPatch, Route(HttpApi.Route)]
        public void Patch(string pluginId)
        {
            var httpApi = PluginCache.GetEnabledFeature<IHttpApi>(pluginId);
            httpApi?.HttpPatch?.Invoke(HttpContext.Current.Request, HttpContext.Current.Response);
        }

        [HttpPatch, Route(HttpApi.RouteName)]
        public void Patch(string pluginId, string name)
        {
            var httpApi = PluginCache.GetEnabledFeature<IHttpApi>(pluginId);
            httpApi?.HttpPatchWithName?.Invoke(HttpContext.Current.Request, HttpContext.Current.Response, name);
        }

        [HttpPatch, Route(HttpApi.RouteNameAndId)]
        public void Patch(string pluginId, string name, int id)
        {
            var httpApi = PluginCache.GetEnabledFeature<IHttpApi>(pluginId);
            httpApi?.HttpPatchWithNameAndId?.Invoke(HttpContext.Current.Request, HttpContext.Current.Response, name, id);
        }
    }
}
