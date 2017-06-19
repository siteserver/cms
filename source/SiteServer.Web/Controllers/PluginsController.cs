using System;
using System.Linq;
using System.Web.Http;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Plugin;
using SiteServer.Plugin;

namespace SiteServer.API.Controllers
{
    [RoutePrefix("api")]
    public class PluginsController : ApiController
    {
        [HttpGet, Route(CMS.Controllers.Plugins.Route)]
        public IHttpActionResult Get(string pluginId)
        {
            try
            {
                var pluginPair = PluginManager.AllPlugins.FirstOrDefault(p => StringUtils.EqualsIgnoreCase(p.Metadata.Id, pluginId) && !p.Metadata.Disabled);
                var restful = pluginPair?.Plugin as IRestful;

                if (restful == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(restful.Get(body));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Route(CMS.Controllers.Plugins.Route)]
        public IHttpActionResult Post(string pluginId)
        {
            try
            {
                var pluginPair = PluginManager.AllPlugins.FirstOrDefault(p => StringUtils.EqualsIgnoreCase(p.Metadata.Id, pluginId) && !p.Metadata.Disabled);
                var restful = pluginPair?.Plugin as IRestful;

                if (restful == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(restful.Post(body));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut, Route(CMS.Controllers.Plugins.Route)]
        public IHttpActionResult Put(string pluginId)
        {
            try
            {
                var pluginPair = PluginManager.AllPlugins.FirstOrDefault(p => StringUtils.EqualsIgnoreCase(p.Metadata.Id, pluginId) && !p.Metadata.Disabled);
                var restful = pluginPair?.Plugin as IRestful;

                if (restful == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(restful.Put(body));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete, Route(CMS.Controllers.Plugins.Route)]
        public IHttpActionResult Delete(string pluginId)
        {
            try
            {
                var pluginPair = PluginManager.AllPlugins.FirstOrDefault(p => StringUtils.EqualsIgnoreCase(p.Metadata.Id, pluginId) && !p.Metadata.Disabled);
                var restful = pluginPair?.Plugin as IRestful;

                if (restful == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(restful.Delete(body));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch, Route(CMS.Controllers.Plugins.Route)]
        public IHttpActionResult Patch(string pluginId)
        {
            try
            {
                var pluginPair = PluginManager.AllPlugins.FirstOrDefault(p => StringUtils.EqualsIgnoreCase(p.Metadata.Id, pluginId) && !p.Metadata.Disabled);
                var restful = pluginPair?.Plugin as IRestful;

                if (restful == null)
                {
                    return NotFound();
                }

                var body = new RequestBody();

                return Ok(restful.Patch(body));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
