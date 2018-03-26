using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SiteServer.CMS.Controllers.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin;

namespace SiteServer.API.Controllers.Http
{
    [RoutePrefix("api")]
    public class PluginHttpController : ApiController
    {
        [HttpGet, Route(ApiRoutePluginHttp.Route)]
        public HttpResponseMessage Get(string pluginId)
        {
            try
            {
                var request = new Request();
                var service = PluginManager.GetService(pluginId);

                return service?.HttpGet == null
                    ? Request.CreateResponse(HttpStatusCode.NotFound)
                    : service.HttpGet.Invoke(request);
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpGet, Route(ApiRoutePluginHttp.RouteName)]
        public HttpResponseMessage Get(string pluginId, string name)
        {
            try
            {
                var request = new Request();
                var service = PluginManager.GetService(pluginId);

                return service?.HttpGetWithName == null
                    ? Request.CreateResponse(HttpStatusCode.NotFound)
                    : service.HttpGetWithName.Invoke(request, name);
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpGet, Route(ApiRoutePluginHttp.RouteNameAndId)]
        public HttpResponseMessage Get(string pluginId, string name, string id)
        {
            try
            {
                var request = new Request();
                var service = PluginManager.GetService(pluginId);

                return service?.HttpGetWithNameAndId == null
                    ? Request.CreateResponse(HttpStatusCode.NotFound)
                    : service.HttpGetWithNameAndId.Invoke(request, name, id);
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPost, Route(ApiRoutePluginHttp.Route)]
        public HttpResponseMessage Post(string pluginId)
        {
            try
            {
                var request = new Request();
                var service = PluginManager.GetService(pluginId);

                return service?.HttpPost == null
                    ? Request.CreateResponse(HttpStatusCode.NotFound)
                    : service.HttpPost.Invoke(request);
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPost, Route(ApiRoutePluginHttp.RouteName)]
        public HttpResponseMessage Post(string pluginId, string name)
        {
            try
            {
                var request = new Request();
                var service = PluginManager.GetService(pluginId);

                return service?.HttpPostWithName == null
                    ? Request.CreateResponse(HttpStatusCode.NotFound)
                    : service.HttpPostWithName.Invoke(request, name);
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPost, Route(ApiRoutePluginHttp.RouteNameAndId)]
        public HttpResponseMessage Post(string pluginId, string name, string id)
        {
            try
            {
                var request = new Request();
                var service = PluginManager.GetService(pluginId);

                return service?.HttpPostWithNameAndId == null
                    ? Request.CreateResponse(HttpStatusCode.NotFound)
                    : service.HttpPostWithNameAndId.Invoke(request, name, id);
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPut, Route(ApiRoutePluginHttp.Route)]
        public HttpResponseMessage Put(string pluginId)
        {
            try
            {
                var request = new Request();
                var service = PluginManager.GetService(pluginId);

                return service?.HttpPut == null
                    ? Request.CreateResponse(HttpStatusCode.NotFound)
                    : service.HttpPut.Invoke(request);
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPut, Route(ApiRoutePluginHttp.RouteName)]
        public HttpResponseMessage Put(string pluginId, string name)
        {
            try
            {
                var request = new Request();
                var service = PluginManager.GetService(pluginId);

                return service?.HttpPutWithName == null
                    ? Request.CreateResponse(HttpStatusCode.NotFound)
                    : service.HttpPutWithName.Invoke(request, name);
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPut, Route(ApiRoutePluginHttp.RouteNameAndId)]
        public HttpResponseMessage Put(string pluginId, string name, string id)
        {
            try
            {
                var request = new Request();
                var service = PluginManager.GetService(pluginId);

                return service?.HttpPutWithNameAndId == null
                    ? Request.CreateResponse(HttpStatusCode.NotFound)
                    : service.HttpPutWithNameAndId.Invoke(request, name, id);
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpDelete, Route(ApiRoutePluginHttp.Route)]
        public HttpResponseMessage Delete(string pluginId)
        {
            try
            {
                var request = new Request();
                var service = PluginManager.GetService(pluginId);

                return service?.HttpDelete == null
                    ? Request.CreateResponse(HttpStatusCode.NotFound)
                    : service.HttpDelete.Invoke(request);
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpDelete, Route(ApiRoutePluginHttp.RouteName)]
        public HttpResponseMessage Delete(string pluginId, string name)
        {
            try
            {
                var request = new Request();
                var service = PluginManager.GetService(pluginId);

                return service?.HttpDeleteWithName == null
                    ? Request.CreateResponse(HttpStatusCode.NotFound)
                    : service.HttpDeleteWithName.Invoke(request, name);
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpDelete, Route(ApiRoutePluginHttp.RouteNameAndId)]
        public HttpResponseMessage Delete(string pluginId, string name, string id)
        {
            try
            {
                var request = new Request();
                var service = PluginManager.GetService(pluginId);

                return service?.HttpDeleteWithNameAndId == null
                    ? Request.CreateResponse(HttpStatusCode.NotFound)
                    : service.HttpDeleteWithNameAndId.Invoke(request, name, id);
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
    }
}
