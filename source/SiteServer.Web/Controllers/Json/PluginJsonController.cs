using System;
using System.Web.Http;
using BaiRong.Core;
using SiteServer.CMS.Controllers.Json;
using SiteServer.CMS.Core;
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
                var body = new RequestBody();
                var webApi = PluginCache.GetEnabledFeature<IWebApi>(pluginId);

                if (webApi?.JsonGet == null)
                {
                    return NotFound();
                }

                return Ok(webApi.JsonGet(body));
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
                var body = new RequestBody();
                var webApi = PluginCache.GetEnabledFeature<IWebApi>(pluginId);

                if (webApi?.JsonGetWithName == null)
                {
                    return NotFound();
                }

                return Ok(webApi.JsonGetWithName(body, name));
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
                var body = new RequestBody();
                var webApi = PluginCache.GetEnabledFeature<IWebApi>(pluginId);

                if (webApi?.JsonGetWithNameAndId == null)
                {
                    return NotFound();
                }

                return Ok(webApi.JsonGetWithNameAndId(body, name, id));
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
                var body = new RequestBody();
                var webApi = PluginCache.GetEnabledFeature<IWebApi>(pluginId);

                if (webApi?.JsonPost == null)
                {
                    return NotFound();
                }

                return Ok(webApi.JsonPost(body));
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
                var body = new RequestBody();
                var webApi = PluginCache.GetEnabledFeature<IWebApi>(pluginId);

                if (webApi?.JsonPostWithName == null)
                {
                    return NotFound();
                }

                return Ok(webApi.JsonPostWithName(body, name));
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
                var body = new RequestBody();
                var webApi = PluginCache.GetEnabledFeature<IWebApi>(pluginId);

                if (webApi?.JsonPostWithNameAndId == null)
                {
                    return NotFound();
                }

                return Ok(webApi.JsonPostWithNameAndId(body, name, id));
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
                var body = new RequestBody();
                var webApi = PluginCache.GetEnabledFeature<IWebApi>(pluginId);

                if (webApi?.JsonPut == null)
                {
                    return NotFound();
                }

                return Ok(webApi.JsonPut(body));
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
                var body = new RequestBody();
                var webApi = PluginCache.GetEnabledFeature<IWebApi>(pluginId);

                if (webApi?.JsonPutWithName == null)
                {
                    return NotFound();
                }

                return Ok(webApi.JsonPutWithName(body, name));
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
                var body = new RequestBody();
                var webApi = PluginCache.GetEnabledFeature<IWebApi>(pluginId);

                if (webApi?.JsonPutWithNameAndId == null)
                {
                    return NotFound();
                }

                return Ok(webApi.JsonPutWithNameAndId(body, name, id));
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
                var body = new RequestBody();
                var webApi = PluginCache.GetEnabledFeature<IWebApi>(pluginId);

                if (webApi?.JsonDelete == null)
                {
                    return NotFound();
                }

                return Ok(webApi.JsonDelete(body));
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
                var body = new RequestBody();
                var webApi = PluginCache.GetEnabledFeature<IWebApi>(pluginId);

                if (webApi?.JsonDeleteWithName == null)
                {
                    return NotFound();
                }

                return Ok(webApi.JsonDeleteWithName(body, name));
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
                var body = new RequestBody();
                var webApi = PluginCache.GetEnabledFeature<IWebApi>(pluginId);

                if (webApi?.JsonDeleteWithNameAndId == null)
                {
                    return NotFound();
                }

                return Ok(webApi.JsonDeleteWithNameAndId(body, name, id));
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
                var body = new RequestBody();
                var webApi = PluginCache.GetEnabledFeature<IWebApi>(pluginId);

                if (webApi?.JsonPatch == null)
                {
                    return NotFound();
                }

                return Ok(webApi.JsonPatch(body));
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
                var body = new RequestBody();
                var webApi = PluginCache.GetEnabledFeature<IWebApi>(pluginId);

                if (webApi?.JsonPatchWithName == null)
                {
                    return NotFound();
                }

                return Ok(webApi.JsonPatchWithName(body, name));
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
                var body = new RequestBody();
                var webApi = PluginCache.GetEnabledFeature<IWebApi>(pluginId);

                if (webApi?.JsonPatchWithNameAndId == null)
                {
                    return NotFound();
                }

                return Ok(webApi.JsonPatchWithNameAndId(body, name, id));
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return BadRequest(ex.Message);
            }
        }
    }
}
