using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using SiteServer.CMS.Plugin;

namespace SiteServer.API.Controllers.V1
{
    [RoutePrefix("v1/test")]
    public class V1TestController : ApiController
    {
        [HttpGet]
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

        [HttpGet, Route("any")]
        public HttpResponseMessage GetAny()
        {
            //return Content(HttpStatusCode.Created, new
            //{
            //    IsOk = true
            //});

            //var content = ;

            var response = Request.CreateResponse(HttpStatusCode.NotFound);

            response.Content = new StringContent("yes, yes", Encoding.UTF8);

            return response;
        }

        [HttpGet, Route("string")]
        public IHttpActionResult GetString()
        {
            return Ok("Hello");
        }

        //private readonly HttpClient _httpClient = new HttpClient();

        //[HttpGet, Route("test/count")]
        //public async Task<IHttpActionResult> GetDotNetCountAsync()
        //{
        //    // Suspends GetDotNetCountAsync() to allow the caller (the web server)
        //    // to accept another request, rather than blocking on this one.
        //    var html = await _httpClient.GetStringAsync("http://dotnetfoundation.org");

        //    return Ok(new
        //    {
        //        Regex.Matches(html, @"\.NET").Count
        //    });
        //}
    }
}
