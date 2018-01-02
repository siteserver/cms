using System;
using System.Collections.Generic;
using System.Web.Http;
using SiteServer.CMS.Plugin;

namespace SiteServer.API.Controllers
{
    [RoutePrefix("api")]
    public class TestController : ApiController
    {
        [HttpGet, Route("test")]
        public IHttpActionResult Get()
        {
            var pluginIds = new List<string>();
            foreach (var pluginPair in PluginManager.AllPluginPairs)
            {
                if (!pluginPair.Metadata.Disabled)
                {
                    pluginIds.Add(pluginPair.Metadata.Id);
                }
            }
            return Ok(new
            {
                DateTime = DateTime.Now,
                Plugins = pluginIds
            });
        }
    }
}
