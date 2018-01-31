using System;
using System.Web.Http;
using SiteServer.CMS.Controllers;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin;
using SiteServer.Plugin;

namespace SiteServer.API.Controllers
{
    [RoutePrefix("api")]
    public class PluginController : ApiController
    {
        [HttpGet, Route(ApiRoutePlugin.Route)]
        public IHttpActionResult Get(string pluginId)
        {
            try
            {
                var request = new Request();
                var service = PluginManager.GetService(pluginId);

                var retval = service.OnApiGet(new ApiEventArgs(request, null, null));
                return retval == null ? (IHttpActionResult) NotFound() : Ok(retval);
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet, Route(ApiRoutePlugin.RouteAction)]
        public IHttpActionResult Get(string pluginId, string name)
        {
            try
            {
                var request = new Request();
                var service = PluginManager.GetService(pluginId);

                var retval = service.OnApiGet(new ApiEventArgs(request, name, null));
                return retval == null ? (IHttpActionResult)NotFound() : Ok(retval);
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet, Route(ApiRoutePlugin.RouteActionAndId)]
        public IHttpActionResult Get(string pluginId, string name, string id)
        {
            try
            {
                var request = new Request();
                var service = PluginManager.GetService(pluginId);

                var retval = service.OnApiGet(new ApiEventArgs(request, name, id));
                return retval == null ? (IHttpActionResult)NotFound() : Ok(retval);
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Route(ApiRoutePlugin.Route)]
        public IHttpActionResult Post(string pluginId)
        {
            try
            {
                var request = new Request();
                var service = PluginManager.GetService(pluginId);

                var retval = service.OnApiPost(new ApiEventArgs(request, null, null));
                return retval == null ? (IHttpActionResult)NotFound() : Ok(retval);
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Route(ApiRoutePlugin.RouteAction)]
        public IHttpActionResult Post(string pluginId, string name)
        {
            try
            {
                var request = new Request();
                var service = PluginManager.GetService(pluginId);

                var retval = service.OnApiPost(new ApiEventArgs(request, name, null));
                return retval == null ? (IHttpActionResult)NotFound() : Ok(retval);
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Route(ApiRoutePlugin.RouteActionAndId)]
        public IHttpActionResult Post(string pluginId, string name, string id)
        {
            try
            {
                var request = new Request();
                var service = PluginManager.GetService(pluginId);

                var retval = service.OnApiPost(new ApiEventArgs(request, name, id));
                return retval == null ? (IHttpActionResult)NotFound() : Ok(retval);
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut, Route(ApiRoutePlugin.Route)]
        public IHttpActionResult Put(string pluginId)
        {
            try
            {
                var request = new Request();
                var service = PluginManager.GetService(pluginId);

                var retval = service.OnApiPut(new ApiEventArgs(request, null, null));
                return retval == null ? (IHttpActionResult)NotFound() : Ok(retval);
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut, Route(ApiRoutePlugin.RouteAction)]
        public IHttpActionResult Put(string pluginId, string name)
        {
            try
            {
                var request = new Request();
                var service = PluginManager.GetService(pluginId);

                var retval = service.OnApiPut(new ApiEventArgs(request, name, null));
                return retval == null ? (IHttpActionResult)NotFound() : Ok(retval);
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut, Route(ApiRoutePlugin.RouteActionAndId)]
        public IHttpActionResult Put(string pluginId, string name, string id)
        {
            try
            {
                var request = new Request();
                var service = PluginManager.GetService(pluginId);

                var retval = service.OnApiPut(new ApiEventArgs(request, name, id));
                return retval == null ? (IHttpActionResult)NotFound() : Ok(retval);
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete, Route(ApiRoutePlugin.Route)]
        public IHttpActionResult Delete(string pluginId)
        {
            try
            {
                var request = new Request();
                var service = PluginManager.GetService(pluginId);

                var retval = service.OnApiDelete(new ApiEventArgs(request, null, null));
                return retval == null ? (IHttpActionResult)NotFound() : Ok(retval);
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete, Route(ApiRoutePlugin.RouteAction)]
        public IHttpActionResult Delete(string pluginId, string name)
        {
            try
            {
                var request = new Request();
                var service = PluginManager.GetService(pluginId);

                var retval = service.OnApiDelete(new ApiEventArgs(request, name, null));
                return retval == null ? (IHttpActionResult)NotFound() : Ok(retval);
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete, Route(ApiRoutePlugin.RouteActionAndId)]
        public IHttpActionResult Delete(string pluginId, string name, string id)
        {
            try
            {
                var request = new Request();
                var service = PluginManager.GetService(pluginId);

                var retval = service.OnApiDelete(new ApiEventArgs(request, name, id));
                return retval == null ? (IHttpActionResult)NotFound() : Ok(retval);
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }        
    }
}
