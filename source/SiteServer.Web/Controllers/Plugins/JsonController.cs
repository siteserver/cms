using System;
using System.Web.Http;
using SiteServer.CMS.Controllers.Plugins;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin;
using SiteServer.Plugin.Features;

namespace SiteServer.API.Controllers.Plugins
{
    [RoutePrefix("api")]
    public class JsonController : ApiController
    {
        [HttpGet, Route(JsonApi.Route)]
        public IHttpActionResult Get(string pluginId)
        {
            try
            {
                var jsonApi = PluginCache.GetEnabledFeature<IJsonApi>(pluginId);

                if (jsonApi?.JsonGet == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(jsonApi.JsonGet(body));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet, Route(JsonApi.RouteName)]
        public IHttpActionResult Get(string pluginId, string name)
        {
            try
            {
                var jsonApi = PluginCache.GetEnabledFeature<IJsonApi>(pluginId);

                if (jsonApi?.JsonGetWithName == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(jsonApi.JsonGetWithName(body, name));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet, Route(JsonApi.RouteNameAndId)]
        public IHttpActionResult Get(string pluginId, string name, int id)
        {
            try
            {
                var jsonApi = PluginCache.GetEnabledFeature<IJsonApi>(pluginId);

                if (jsonApi?.JsonGetWithNameAndId == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(jsonApi.JsonGetWithNameAndId(body, name, id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Route(JsonApi.Route)]
        public IHttpActionResult Post(string pluginId)
        {
            try
            {
                var jsonApi = PluginCache.GetEnabledFeature<IJsonApi>(pluginId);

                if (jsonApi?.JsonPost == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(jsonApi.JsonPost(body));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Route(JsonApi.RouteName)]
        public IHttpActionResult Post(string pluginId, string name)
        {
            try
            {
                var jsonApi = PluginCache.GetEnabledFeature<IJsonApi>(pluginId);

                if (jsonApi?.JsonPostWithName == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(jsonApi.JsonPostWithName(body, name));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Route(JsonApi.RouteNameAndId)]
        public IHttpActionResult Post(string pluginId, string name, int id)
        {
            try
            {
                var jsonApi = PluginCache.GetEnabledFeature<IJsonApi>(pluginId);

                if (jsonApi?.JsonPostWithNameAndId == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(jsonApi.JsonPostWithNameAndId(body, name, id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut, Route(JsonApi.Route)]
        public IHttpActionResult Put(string pluginId)
        {
            try
            {
                var jsonApi = PluginCache.GetEnabledFeature<IJsonApi>(pluginId);

                if (jsonApi?.JsonPut == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(jsonApi.JsonPut(body));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut, Route(JsonApi.RouteName)]
        public IHttpActionResult Put(string pluginId, string name)
        {
            try
            {
                var jsonApi = PluginCache.GetEnabledFeature<IJsonApi>(pluginId);

                if (jsonApi?.JsonPutWithName == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(jsonApi.JsonPutWithName(body, name));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut, Route(JsonApi.RouteNameAndId)]
        public IHttpActionResult Put(string pluginId, string name, int id)
        {
            try
            {
                var jsonApi = PluginCache.GetEnabledFeature<IJsonApi>(pluginId);

                if (jsonApi?.JsonPutWithNameAndId == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(jsonApi.JsonPutWithNameAndId(body, name, id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete, Route(JsonApi.Route)]
        public IHttpActionResult Delete(string pluginId)
        {
            try
            {
                var jsonApi = PluginCache.GetEnabledFeature<IJsonApi>(pluginId);

                if (jsonApi?.JsonDelete == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(jsonApi.JsonDelete(body));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete, Route(JsonApi.RouteName)]
        public IHttpActionResult Delete(string pluginId, string name)
        {
            try
            {
                var jsonApi = PluginCache.GetEnabledFeature<IJsonApi>(pluginId);

                if (jsonApi?.JsonDeleteWithName == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(jsonApi.JsonDeleteWithName(body, name));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete, Route(JsonApi.RouteNameAndId)]
        public IHttpActionResult Delete(string pluginId, string name, int id)
        {
            try
            {
                var jsonApi = PluginCache.GetEnabledFeature<IJsonApi>(pluginId);

                if (jsonApi?.JsonDeleteWithNameAndId == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(jsonApi.JsonDeleteWithNameAndId(body, name, id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch, Route(JsonApi.Route)]
        public IHttpActionResult Patch(string pluginId)
        {
            try
            {
                var jsonApi = PluginCache.GetEnabledFeature<IJsonApi>(pluginId);

                if (jsonApi?.JsonPatch == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(jsonApi.JsonPatch(body));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch, Route(JsonApi.RouteName)]
        public IHttpActionResult Patch(string pluginId, string name)
        {
            try
            {
                var jsonApi = PluginCache.GetEnabledFeature<IJsonApi>(pluginId);

                if (jsonApi?.JsonPatchWithName == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(jsonApi.JsonPatchWithName(body, name));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch, Route(JsonApi.RouteNameAndId)]
        public IHttpActionResult Patch(string pluginId, string name, int id)
        {
            try
            {
                var jsonApi = PluginCache.GetEnabledFeature<IJsonApi>(pluginId);

                if (jsonApi?.JsonPatchWithNameAndId == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(jsonApi.JsonPatchWithNameAndId(body, name, id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
