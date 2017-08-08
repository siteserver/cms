using System;
using System.Web.Http;
using SiteServer.CMS.Controllers.Plugins;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin;
using SiteServer.Plugin.Hooks;

namespace SiteServer.API.Controllers.Plugins
{
    [RoutePrefix("api")]
    public class NameController : ApiController
    {
        [HttpGet, Route(Name.Route)]
        public IHttpActionResult Get(string pluginId, string name)
        {
            try
            {
                var restful = PluginCache.GetHook<IRestful>(pluginId);

                if (restful == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(restful.Get(body, name));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Route(Name.Route)]
        public IHttpActionResult Post(string pluginId, string name)
        {
            try
            {
                var restful = PluginCache.GetHook<IRestful>(pluginId);

                if (restful == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(restful.Post(body, name));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut, Route(Name.Route)]
        public IHttpActionResult Put(string pluginId, string name)
        {
            try
            {
                var restful = PluginCache.GetHook<IRestful>(pluginId);

                if (restful == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(restful.Put(body, name));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete, Route(Name.Route)]
        public IHttpActionResult Delete(string pluginId, string name)
        {
            try
            {
                var restful = PluginCache.GetHook<IRestful>(pluginId);

                if (restful == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(restful.Delete(body, name));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch, Route(Name.Route)]
        public IHttpActionResult Patch(string pluginId, string name)
        {
            try
            {
                var restful = PluginCache.GetHook<IRestful>(pluginId);

                if (restful == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(restful.Patch(body, name));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
