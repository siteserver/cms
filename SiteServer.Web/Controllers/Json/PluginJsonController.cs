using System;
using System.Web.Http;
using BaiRong.Core;
using SiteServer.CMS.Controllers.Json;
using SiteServer.CMS.Plugin;
using SiteServer.Plugin.Features;

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
                var webApi = PluginManager.GetEnabledFeature<IWebApi>(pluginId);

                if (webApi?.JsonGet == null)
                {
                    return NotFound();
                }

                return Ok(webApi.JsonGet(context));
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
                var webApi = PluginManager.GetEnabledFeature<IWebApi>(pluginId);

                if (webApi?.JsonGetWithName == null)
                {
                    return NotFound();
                }

                return Ok(webApi.JsonGetWithName(context, name));
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
                var webApi = PluginManager.GetEnabledFeature<IWebApi>(pluginId);

                if (webApi?.JsonGetWithNameAndId == null)
                {
                    return NotFound();
                }

                return Ok(webApi.JsonGetWithNameAndId(context, name, id));
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
                var webApi = PluginManager.GetEnabledFeature<IWebApi>(pluginId);

                if (webApi?.JsonPost == null)
                {
                    return NotFound();
                }

                return Ok(webApi.JsonPost(context));
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
                var webApi = PluginManager.GetEnabledFeature<IWebApi>(pluginId);

                if (webApi?.JsonPostWithName == null)
                {
                    return NotFound();
                }

                return Ok(webApi.JsonPostWithName(context, name));
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
                var webApi = PluginManager.GetEnabledFeature<IWebApi>(pluginId);

                if (webApi?.JsonPostWithNameAndId == null)
                {
                    return NotFound();
                }

                return Ok(webApi.JsonPostWithNameAndId(context, name, id));
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
                var webApi = PluginManager.GetEnabledFeature<IWebApi>(pluginId);

                if (webApi?.JsonPut == null)
                {
                    return NotFound();
                }

                return Ok(webApi.JsonPut(context));
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
                var webApi = PluginManager.GetEnabledFeature<IWebApi>(pluginId);

                if (webApi?.JsonPutWithName == null)
                {
                    return NotFound();
                }

                return Ok(webApi.JsonPutWithName(context, name));
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
                var webApi = PluginManager.GetEnabledFeature<IWebApi>(pluginId);

                if (webApi?.JsonPutWithNameAndId == null)
                {
                    return NotFound();
                }

                return Ok(webApi.JsonPutWithNameAndId(context, name, id));
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
                var webApi = PluginManager.GetEnabledFeature<IWebApi>(pluginId);

                if (webApi?.JsonDelete == null)
                {
                    return NotFound();
                }

                return Ok(webApi.JsonDelete(context));
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
                var webApi = PluginManager.GetEnabledFeature<IWebApi>(pluginId);

                if (webApi?.JsonDeleteWithName == null)
                {
                    return NotFound();
                }

                return Ok(webApi.JsonDeleteWithName(context, name));
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
                var webApi = PluginManager.GetEnabledFeature<IWebApi>(pluginId);

                if (webApi?.JsonDeleteWithNameAndId == null)
                {
                    return NotFound();
                }

                return Ok(webApi.JsonDeleteWithNameAndId(context, name, id));
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch, Route(PluginJsonApi.Route)]
        public IHttpActionResult Patch(string pluginId)
        {
            try
            {
                var context = new RequestContext();
                var webApi = PluginManager.GetEnabledFeature<IWebApi>(pluginId);

                if (webApi?.JsonPatch == null)
                {
                    return NotFound();
                }

                return Ok(webApi.JsonPatch(context));
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch, Route(PluginJsonApi.RouteName)]
        public IHttpActionResult Patch(string pluginId, string name)
        {
            try
            {
                var context = new RequestContext();
                var webApi = PluginManager.GetEnabledFeature<IWebApi>(pluginId);

                if (webApi?.JsonPatchWithName == null)
                {
                    return NotFound();
                }

                return Ok(webApi.JsonPatchWithName(context, name));
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch, Route(PluginJsonApi.RouteNameAndId)]
        public IHttpActionResult Patch(string pluginId, string name, string id)
        {
            try
            {
                var context = new RequestContext();
                var webApi = PluginManager.GetEnabledFeature<IWebApi>(pluginId);

                if (webApi?.JsonPatchWithNameAndId == null)
                {
                    return NotFound();
                }

                return Ok(webApi.JsonPatchWithNameAndId(context, name, id));
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }
    }
}
