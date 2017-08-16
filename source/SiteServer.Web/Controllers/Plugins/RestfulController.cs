using System;
using System.Web.Http;
using SiteServer.CMS.Controllers.Plugins;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin;
using SiteServer.Plugin.Features;

namespace SiteServer.API.Controllers.Plugins
{
    [RoutePrefix("api")]
    public class RestfulController : ApiController
    {
        [HttpGet, Route(Restful.Route)]
        public IHttpActionResult Get(string pluginId)
        {
            try
            {
                var restful = PluginCache.GetEnabledFeature<IRestful>(pluginId);

                if (restful == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(restful.RestfulGet(body));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet, Route(Restful.RouteName)]
        public IHttpActionResult Get(string pluginId, string name)
        {
            try
            {
                var restful = PluginCache.GetEnabledFeature<IRestful>(pluginId);

                if (restful == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(restful.RestfulGet(body, name));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet, Route(Restful.RouteNameAndId)]
        public IHttpActionResult Get(string pluginId, string name, int id)
        {
            try
            {
                var restful = PluginCache.GetEnabledFeature<IRestful>(pluginId);

                if (restful == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(restful.RestfulGet(body, name, id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Route(Restful.Route)]
        public IHttpActionResult Post(string pluginId)
        {
            try
            {
                var restful = PluginCache.GetEnabledFeature<IRestful>(pluginId);

                if (restful == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(restful.RestfulPost(body));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Route(Restful.RouteName)]
        public IHttpActionResult Post(string pluginId, string name)
        {
            try
            {
                var restful = PluginCache.GetEnabledFeature<IRestful>(pluginId);

                if (restful == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(restful.RestfulPost(body, name));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Route(Restful.RouteNameAndId)]
        public IHttpActionResult Post(string pluginId, string name, int id)
        {
            try
            {
                var restful = PluginCache.GetEnabledFeature<IRestful>(pluginId);

                if (restful == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(restful.RestfulPost(body, name, id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut, Route(Restful.Route)]
        public IHttpActionResult Put(string pluginId)
        {
            try
            {
                var restful = PluginCache.GetEnabledFeature<IRestful>(pluginId);

                if (restful == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(restful.RestfulPut(body));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut, Route(Restful.RouteName)]
        public IHttpActionResult Put(string pluginId, string name)
        {
            try
            {
                var restful = PluginCache.GetEnabledFeature<IRestful>(pluginId);

                if (restful == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(restful.RestfulPut(body, name));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut, Route(Restful.RouteNameAndId)]
        public IHttpActionResult Put(string pluginId, string name, int id)
        {
            try
            {
                var restful = PluginCache.GetEnabledFeature<IRestful>(pluginId);

                if (restful == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(restful.RestfulPut(body, name, id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete, Route(Restful.Route)]
        public IHttpActionResult Delete(string pluginId)
        {
            try
            {
                var restful = PluginCache.GetEnabledFeature<IRestful>(pluginId);

                if (restful == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(restful.RestfulDelete(body));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete, Route(Restful.RouteName)]
        public IHttpActionResult Delete(string pluginId, string name)
        {
            try
            {
                var restful = PluginCache.GetEnabledFeature<IRestful>(pluginId);

                if (restful == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(restful.RestfulDelete(body, name));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete, Route(Restful.RouteNameAndId)]
        public IHttpActionResult Delete(string pluginId, string name, int id)
        {
            try
            {
                var restful = PluginCache.GetEnabledFeature<IRestful>(pluginId);

                if (restful == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(restful.RestfulDelete(body, name, id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch, Route(Restful.Route)]
        public IHttpActionResult Patch(string pluginId)
        {
            try
            {
                var restful = PluginCache.GetEnabledFeature<IRestful>(pluginId);

                if (restful == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(restful.RestfulPatch(body));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch, Route(Restful.RouteName)]
        public IHttpActionResult Patch(string pluginId, string name)
        {
            try
            {
                var restful = PluginCache.GetEnabledFeature<IRestful>(pluginId);

                if (restful == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(restful.RestfulPatch(body, name));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch, Route(Restful.RouteNameAndId)]
        public IHttpActionResult Patch(string pluginId, string name, int id)
        {
            try
            {
                var restful = PluginCache.GetEnabledFeature<IRestful>(pluginId);

                if (restful == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(restful.RestfulPatch(body, name, id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
