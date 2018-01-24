using System;
using System.Web.Http;
using SiteServer.CMS.Controllers.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Plugin.Model;

namespace SiteServer.API.Controllers.Json
{
    [RoutePrefix("api")]
    public class PluginJsonController : ApiController
    {
        [HttpGet, Route(PluginJsonApi.Route)]
        public IHttpActionResult Get(string pluginId)
        {
            try
            {
                var context = new RequestContext();
                var service = PluginManager.GetService(pluginId);

                if (service?.JsonGet == null)
                {
                    return NotFound();
                }

                return Ok(service.JsonGet(context));
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet, Route(PluginJsonApi.RouteName)]
        public IHttpActionResult Get(string pluginId, string name)
        {
            try
            {
                var context = new RequestContext();
                var service = PluginManager.GetService(pluginId);

                if (service?.JsonGetWithName == null)
                {
                    return NotFound();
                }

                return Ok(service.JsonGetWithName(context, name));
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet, Route(PluginJsonApi.RouteNameAndId)]
        public IHttpActionResult Get(string pluginId, string name, string id)
        {
            try
            {
                var context = new RequestContext();
                var service = PluginManager.GetService(pluginId);

                if (service?.JsonGetWithNameAndId == null)
                {
                    return NotFound();
                }

                return Ok(service.JsonGetWithNameAndId(context, name, id));
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Route(PluginJsonApi.Route)]
        public IHttpActionResult Post(string pluginId)
        {
            try
            {
                var context = new RequestContext();
                var service = PluginManager.GetService(pluginId);

                if (service?.JsonPost == null)
                {
                    return NotFound();
                }

                return Ok(service.JsonPost(context));
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Route(PluginJsonApi.RouteName)]
        public IHttpActionResult Post(string pluginId, string name)
        {
            try
            {
                var context = new RequestContext();
                var service = PluginManager.GetService(pluginId);

                if (service?.JsonPostWithName == null)
                {
                    return NotFound();
                }

                return Ok(service.JsonPostWithName(context, name));
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Route(PluginJsonApi.RouteNameAndId)]
        public IHttpActionResult Post(string pluginId, string name, string id)
        {
            try
            {
                var context = new RequestContext();
                var service = PluginManager.GetService(pluginId);

                if (service?.JsonPostWithNameAndId == null)
                {
                    return NotFound();
                }

                return Ok(service.JsonPostWithNameAndId(context, name, id));
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut, Route(PluginJsonApi.Route)]
        public IHttpActionResult Put(string pluginId)
        {
            try
            {
                var context = new RequestContext();
                var service = PluginManager.GetService(pluginId);

                if (service?.JsonPut == null)
                {
                    return NotFound();
                }

                return Ok(service.JsonPut(context));
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut, Route(PluginJsonApi.RouteName)]
        public IHttpActionResult Put(string pluginId, string name)
        {
            try
            {
                var context = new RequestContext();
                var service = PluginManager.GetService(pluginId);

                if (service?.JsonPutWithName == null)
                {
                    return NotFound();
                }

                return Ok(service.JsonPutWithName(context, name));
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut, Route(PluginJsonApi.RouteNameAndId)]
        public IHttpActionResult Put(string pluginId, string name, string id)
        {
            try
            {
                var context = new RequestContext();
                var service = PluginManager.GetService(pluginId);

                if (service?.JsonPutWithNameAndId == null)
                {
                    return NotFound();
                }

                return Ok(service.JsonPutWithNameAndId(context, name, id));
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete, Route(PluginJsonApi.Route)]
        public IHttpActionResult Delete(string pluginId)
        {
            try
            {
                var context = new RequestContext();
                var service = PluginManager.GetService(pluginId);

                if (service?.JsonDelete == null)
                {
                    return NotFound();
                }

                return Ok(service.JsonDelete(context));
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete, Route(PluginJsonApi.RouteName)]
        public IHttpActionResult Delete(string pluginId, string name)
        {
            try
            {
                var context = new RequestContext();
                var service = PluginManager.GetService(pluginId);

                if (service?.JsonDeleteWithName == null)
                {
                    return NotFound();
                }

                return Ok(service.JsonDeleteWithName(context, name));
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete, Route(PluginJsonApi.RouteNameAndId)]
        public IHttpActionResult Delete(string pluginId, string name, string id)
        {
            try
            {
                var context = new RequestContext();
                var service = PluginManager.GetService(pluginId);

                if (service?.JsonDeleteWithNameAndId == null)
                {
                    return NotFound();
                }

                return Ok(service.JsonDeleteWithNameAndId(context, name, id));
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }        
    }
}
