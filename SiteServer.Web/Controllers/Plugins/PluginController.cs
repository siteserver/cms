using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using SiteServer.CMS.Api;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;

namespace SiteServer.API.Controllers.Plugins
{
    public class PluginController : ApiController
    {
        [HttpGet, Route(ApiRoutePlugin.Route)]
        public IHttpActionResult Get(string pluginId)
        {
            try
            {
                var request = new RequestImpl();
                var service = PluginManager.GetService(pluginId);

                return GetHttpActionResult(service.OnRestApiGet(new RestApiEventArgs(request, ApiRoutePlugin.GetRoute(null, null, null), null, null, null)));
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet, Route(ApiRoutePlugin.RouteAction)]
        public IHttpActionResult GetAction(string pluginId, string routeAction)
        {
            try
            {
                var request = new RequestImpl();
                var service = PluginManager.GetService(pluginId);

                return GetHttpActionResult(service.OnRestApiGet(new RestApiEventArgs(request, ApiRoutePlugin.GetRoute(null, null, routeAction), null, null, routeAction)));
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet, Route(ApiRoutePlugin.RouteResource)]
        public IHttpActionResult GetResource(string pluginId, string routeResource)
        {
            try
            {
                var request = new RequestImpl();
                var service = PluginManager.GetService(pluginId);

                return GetHttpActionResult(service.OnRestApiGet(new RestApiEventArgs(request, ApiRoutePlugin.GetRoute(routeResource, null, null), routeResource, null, null)));
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet, Route(ApiRoutePlugin.RouteResourceAction)]
        public IHttpActionResult GetResourceAction(string pluginId, string routeResource, string routeAction)
        {
            try
            {
                var request = new RequestImpl();
                var service = PluginManager.GetService(pluginId);

                return GetHttpActionResult(service.OnRestApiGet(new RestApiEventArgs(request, ApiRoutePlugin.GetRoute(routeResource, null, routeAction), routeResource, null, routeAction)));
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet, Route(ApiRoutePlugin.RouteResourceId)]
        public IHttpActionResult GetResourceId(string pluginId, string routeResource, string routeId)
        {
            try
            {
                var request = new RequestImpl();
                var service = PluginManager.GetService(pluginId);

                return GetHttpActionResult(service.OnRestApiGet(new RestApiEventArgs(request, ApiRoutePlugin.GetRoute(routeResource, routeId, null), routeResource, routeId, null)));
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet, Route(ApiRoutePlugin.RouteResourceIdAction)]
        public IHttpActionResult GetResourceIdAction(string pluginId, string routeResource, string routeId, string routeAction)
        {
            try
            {
                var request = new RequestImpl();
                var service = PluginManager.GetService(pluginId);

                return GetHttpActionResult(service.OnRestApiGet(new RestApiEventArgs(request, ApiRoutePlugin.GetRoute(routeResource, routeId, routeAction), routeResource, routeId, routeAction)));
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Route(ApiRoutePlugin.Route)]
        public IHttpActionResult Post(string pluginId)
        {
            try
            {
                var request = new RequestImpl();
                var service = PluginManager.GetService(pluginId);

                return GetHttpActionResult(service.OnRestApiPost(new RestApiEventArgs(request, ApiRoutePlugin.GetRoute(null, null, null), null, null, null)));
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Route(ApiRoutePlugin.RouteAction)]
        public IHttpActionResult PostAction(string pluginId, string routeAction)
        {
            try
            {
                var request = new RequestImpl();
                var service = PluginManager.GetService(pluginId);

                return GetHttpActionResult(service.OnRestApiPost(new RestApiEventArgs(request, ApiRoutePlugin.GetRoute(null, null, routeAction), null, null, routeAction)));
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Route(ApiRoutePlugin.RouteResource)]
        public IHttpActionResult PostResource(string pluginId, string routeResource)
        {
            try
            {
                var request = new RequestImpl();
                var service = PluginManager.GetService(pluginId);

                return GetHttpActionResult(service.OnRestApiPost(new RestApiEventArgs(request, ApiRoutePlugin.GetRoute(routeResource, null, null), routeResource, null, null)));
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Route(ApiRoutePlugin.RouteResourceAction)]
        public IHttpActionResult PostResourceAction(string pluginId, string routeResource, string routeAction)
        {
            try
            {
                var request = new RequestImpl();
                var service = PluginManager.GetService(pluginId);

                return GetHttpActionResult(service.OnRestApiPost(new RestApiEventArgs(request, ApiRoutePlugin.GetRoute(routeResource, null, routeAction), routeResource, null, routeAction)));
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Route(ApiRoutePlugin.RouteResourceId)]
        public IHttpActionResult PostResourceId(string pluginId, string routeResource, string routeId)
        {
            try
            {
                var request = new RequestImpl();
                var service = PluginManager.GetService(pluginId);

                return GetHttpActionResult(service.OnRestApiPost(new RestApiEventArgs(request, ApiRoutePlugin.GetRoute(routeResource, routeId, null), routeResource, routeId, null)));
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Route(ApiRoutePlugin.RouteResourceIdAction)]
        public IHttpActionResult PostResourceIdAction(string pluginId, string routeResource, string routeId, string routeAction)
        {
            try
            {
                var request = new RequestImpl();
                var service = PluginManager.GetService(pluginId);

                return GetHttpActionResult(service.OnRestApiPost(new RestApiEventArgs(request, ApiRoutePlugin.GetRoute(routeResource, routeId, routeAction), routeResource, routeId, routeAction)));
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut, Route(ApiRoutePlugin.Route)]
        public IHttpActionResult Put(string pluginId)
        {
            try
            {
                var request = new RequestImpl();
                var service = PluginManager.GetService(pluginId);

                return GetHttpActionResult(service.OnRestApiPut(new RestApiEventArgs(request, ApiRoutePlugin.GetRoute(null, null, null), null, null, null)));
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut, Route(ApiRoutePlugin.RouteAction)]
        public IHttpActionResult PutAction(string pluginId, string routeAction)
        {
            try
            {
                var request = new RequestImpl();
                var service = PluginManager.GetService(pluginId);

                return GetHttpActionResult(service.OnRestApiPut(new RestApiEventArgs(request, ApiRoutePlugin.GetRoute(null, null, routeAction), null, null, routeAction)));
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut, Route(ApiRoutePlugin.RouteResource)]
        public IHttpActionResult PutResource(string pluginId, string routeResource)
        {
            try
            {
                var request = new RequestImpl();
                var service = PluginManager.GetService(pluginId);

                return GetHttpActionResult(service.OnRestApiPut(new RestApiEventArgs(request, ApiRoutePlugin.GetRoute(routeResource, null, null), routeResource, null, null)));
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut, Route(ApiRoutePlugin.RouteResourceAction)]
        public IHttpActionResult PutResourceAction(string pluginId, string routeResource, string routeAction)
        {
            try
            {
                var request = new RequestImpl();
                var service = PluginManager.GetService(pluginId);

                return GetHttpActionResult(service.OnRestApiPut(new RestApiEventArgs(request, ApiRoutePlugin.GetRoute(routeResource, null, routeAction), routeResource, null, routeAction)));
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut, Route(ApiRoutePlugin.RouteResourceId)]
        public IHttpActionResult PutResourceId(string pluginId, string routeResource, string routeId)
        {
            try
            {
                var request = new RequestImpl();
                var service = PluginManager.GetService(pluginId);

                return GetHttpActionResult(service.OnRestApiPut(new RestApiEventArgs(request, ApiRoutePlugin.GetRoute(routeResource, routeId, null), routeResource, routeId, null)));
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut, Route(ApiRoutePlugin.RouteResourceIdAction)]
        public IHttpActionResult PutResourceIdAction(string pluginId, string routeResource, string routeId, string routeAction)
        {
            try
            {
                var request = new RequestImpl();
                var service = PluginManager.GetService(pluginId);

                return GetHttpActionResult(service.OnRestApiPut(new RestApiEventArgs(request, ApiRoutePlugin.GetRoute(routeResource, routeId, routeAction), routeResource, routeId, routeAction)));
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete, Route(ApiRoutePlugin.Route)]
        public IHttpActionResult Delete(string pluginId)
        {
            try
            {
                var request = new RequestImpl();
                var service = PluginManager.GetService(pluginId);

                return GetHttpActionResult(service.OnRestApiDelete(new RestApiEventArgs(request, ApiRoutePlugin.GetRoute(null, null, null), null, null, null)));
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete, Route(ApiRoutePlugin.RouteAction)]
        public IHttpActionResult DeleteAction(string pluginId, string routeAction)
        {
            try
            {
                var request = new RequestImpl();
                var service = PluginManager.GetService(pluginId);

                return GetHttpActionResult(service.OnRestApiDelete(new RestApiEventArgs(request, ApiRoutePlugin.GetRoute(null, null, routeAction), null, null, routeAction)));
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete, Route(ApiRoutePlugin.RouteResource)]
        public IHttpActionResult DeleteResource(string pluginId, string routeResource)
        {
            try
            {
                var request = new RequestImpl();
                var service = PluginManager.GetService(pluginId);

                return GetHttpActionResult(service.OnRestApiDelete(new RestApiEventArgs(request, ApiRoutePlugin.GetRoute(routeResource, null, null), routeResource, null, null)));
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete, Route(ApiRoutePlugin.RouteResourceAction)]
        public IHttpActionResult DeleteResourceAction(string pluginId, string routeResource, string routeAction)
        {
            try
            {
                var request = new RequestImpl();
                var service = PluginManager.GetService(pluginId);

                return GetHttpActionResult(service.OnRestApiDelete(new RestApiEventArgs(request, ApiRoutePlugin.GetRoute(routeResource, null, routeAction), routeResource, null, routeAction)));
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete, Route(ApiRoutePlugin.RouteResourceId)]
        public IHttpActionResult DeleteResourceId(string pluginId, string routeResource, string routeId)
        {
            try
            {
                var request = new RequestImpl();
                var service = PluginManager.GetService(pluginId);

                return GetHttpActionResult(service.OnRestApiDelete(new RestApiEventArgs(request, ApiRoutePlugin.GetRoute(routeResource, routeId, null), routeResource, routeId, null)));
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete, Route(ApiRoutePlugin.RouteResourceIdAction)]
        public IHttpActionResult DeleteResourceIdAction(string pluginId, string routeResource, string routeId, string routeAction)
        {
            try
            {
                var request = new RequestImpl();
                var service = PluginManager.GetService(pluginId);

                return GetHttpActionResult(service.OnRestApiDelete(new RestApiEventArgs(request, ApiRoutePlugin.GetRoute(routeResource, routeId, routeAction), routeResource, routeId, routeAction)));
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        private IHttpActionResult GetHttpActionResult(object retval)
        {
            if (retval == null)
            {
                return NotFound();
            }

            switch (retval.GetType().Name)
            {
                case nameof(String):
                    var response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent((string)retval)
                    };
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
                    return ResponseMessage(response);
                case nameof(IHttpActionResult):
                    return (IHttpActionResult)retval;
                case nameof(HttpResponseMessage):
                    return ResponseMessage((HttpResponseMessage)retval);
                default:
                    return Ok(retval);
            }
        }
    }
}
