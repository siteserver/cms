using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
            foreach (var pluginInfo in PluginManager.PluginInfoListRunnable)
            {
                if (!pluginInfo.IsDisabled)
                {
                    pluginIds.Add(pluginInfo.Id);
                }
            }
            return Ok(new
            {
                DateTime = DateTime.Now,
                Plugins = pluginIds
            });
        }

        private readonly HttpClient _httpClient = new HttpClient();

        [HttpGet, Route("test/count")]
        public async Task<IHttpActionResult> GetDotNetCountAsync()
        {
            // Suspends GetDotNetCountAsync() to allow the caller (the web server)
            // to accept another request, rather than blocking on this one.
            var html = await _httpClient.GetStringAsync("http://dotnetfoundation.org");

            return Ok(new
            {
                Regex.Matches(html, @"\.NET").Count
            });
        }
    }
}
