using System;
using System.Web.Http;
using SiteServer.CMS.Controllers.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin;

namespace SiteServer.API.Controllers.Json
{
    [RoutePrefix("api")]
    public class PluginJsonController : ApiController
    {
        [HttpGet, Route(ApiRoutePluginJson.Route)]
        public IHttpActionResult Get(string pluginId)
        {
            try
            {
                var request = new Request();
                var service = PluginManager.GetService(pluginId);

                if (service?.JsonGet == null)
                {
                    return NotFound();
                }

                return Ok(service.JsonGet(request));
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet, Route(ApiRoutePluginJson.RouteName)]
        public IHttpActionResult Get(string pluginId, string name)
        {
            try
            {
                var request = new Request();
                var service = PluginManager.GetService(pluginId);

                if (service?.JsonGetWithName == null)
                {
                    return NotFound();
                }

                return Ok(service.JsonGetWithName(request, name));
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet, Route(ApiRoutePluginJson.RouteNameAndId)]
        public IHttpActionResult Get(string pluginId, string name, string id)
        {
            try
            {
                var request = new Request();
                var service = PluginManager.GetService(pluginId);

                if (service?.JsonGetWithNameAndId == null)
                {
                    return NotFound();
                }

                return Ok(service.JsonGetWithNameAndId(request, name, id));
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Route(ApiRoutePluginJson.Route)]
        public IHttpActionResult Post(string pluginId)
        {
            try
            {
                var request = new Request();
                var service = PluginManager.GetService(pluginId);

                if (service?.JsonPost == null)
                {
                    return NotFound();
                }

                return Ok(service.JsonPost(request));
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Route(ApiRoutePluginJson.RouteName)]
        public IHttpActionResult Post(string pluginId, string name)
        {
            try
            {
                var request = new Request();
                var service = PluginManager.GetService(pluginId);

                if (service?.JsonPostWithName == null)
                {
                    return NotFound();
                }

                return Ok(service.JsonPostWithName(request, name));
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Route(ApiRoutePluginJson.RouteNameAndId)]
        public IHttpActionResult Post(string pluginId, string name, string id)
        {
            try
            {
                var request = new Request();
                var service = PluginManager.GetService(pluginId);

                if (service?.JsonPostWithNameAndId == null)
                {
                    return NotFound();
                }

                return Ok(service.JsonPostWithNameAndId(request, name, id));
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut, Route(ApiRoutePluginJson.Route)]
        public IHttpActionResult Put(string pluginId)
        {
            try
            {
                var request = new Request();
                var service = PluginManager.GetService(pluginId);

                if (service?.JsonPut == null)
                {
                    return NotFound();
                }

                return Ok(service.JsonPut(request));
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut, Route(ApiRoutePluginJson.RouteName)]
        public IHttpActionResult Put(string pluginId, string name)
        {
            try
            {
                var request = new Request();
                var service = PluginManager.GetService(pluginId);

                if (service?.JsonPutWithName == null)
                {
                    return NotFound();
                }

                return Ok(service.JsonPutWithName(request, name));
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut, Route(ApiRoutePluginJson.RouteNameAndId)]
        public IHttpActionResult Put(string pluginId, string name, string id)
        {
            try
            {
                var request = new Request();
                var service = PluginManager.GetService(pluginId);

                if (service?.JsonPutWithNameAndId == null)
                {
                    return NotFound();
                }

                return Ok(service.JsonPutWithNameAndId(request, name, id));
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete, Route(ApiRoutePluginJson.Route)]
        public IHttpActionResult Delete(string pluginId)
        {
            try
            {
                var request = new Request();
                var service = PluginManager.GetService(pluginId);

                if (service?.JsonDelete == null)
                {
                    return NotFound();
                }

                return Ok(service.JsonDelete(request));
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete, Route(ApiRoutePluginJson.RouteName)]
        public IHttpActionResult Delete(string pluginId, string name)
        {
            try
            {
                var request = new Request();
                var service = PluginManager.GetService(pluginId);

                if (service?.JsonDeleteWithName == null)
                {
                    return NotFound();
                }

                return Ok(service.JsonDeleteWithName(request, name));
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete, Route(ApiRoutePluginJson.RouteNameAndId)]
        public IHttpActionResult Delete(string pluginId, string name, string id)
        {
            try
            {
                var request = new Request();
                var service = PluginManager.GetService(pluginId);

                if (service?.JsonDeleteWithNameAndId == null)
                {
                    return NotFound();
                }

                return Ok(service.JsonDeleteWithNameAndId(request, name, id));
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }        
    }
}
