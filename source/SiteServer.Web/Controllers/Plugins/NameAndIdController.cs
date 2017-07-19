using System;
using System.Web.Http;
using SiteServer.CMS.Controllers.Plugins;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin;
using SiteServer.Plugin.Hooks;

namespace SiteServer.API.Controllers.Plugins
{
    [RoutePrefix("api")]
    public class NameAndIdController : ApiController
    {
        [HttpGet, Route(NameAndId.Route)]
        public IHttpActionResult Get(string pluginId, string name, int id)
        {
            try
            {
                var restful = PluginManager.GetHook<IRestful>(pluginId);

                if (restful == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(restful.Get(body, name, id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Route(NameAndId.Route)]
        public IHttpActionResult Post(string pluginId, string name, int id)
        {
            try
            {
                var restful = PluginManager.GetHook<IRestful>(pluginId);

                if (restful == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(restful.Post(body, name, id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut, Route(NameAndId.Route)]
        public IHttpActionResult Put(string pluginId, string name, int id)
        {
            try
            {
                var restful = PluginManager.GetHook<IRestful>(pluginId);

                if (restful == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(restful.Put(body, name, id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete, Route(NameAndId.Route)]
        public IHttpActionResult Delete(string pluginId, string name, int id)
        {
            try
            {
                var restful = PluginManager.GetHook<IRestful>(pluginId);

                if (restful == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(restful.Delete(body, name, id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch, Route(NameAndId.Route)]
        public IHttpActionResult Patch(string pluginId, string name, int id)
        {
            try
            {
                var restful = PluginManager.GetHook<IRestful>(pluginId);

                if (restful == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(restful.Patch(body, name, id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
