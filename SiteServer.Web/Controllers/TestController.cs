using System;
using System.Web.Http;
using SiteServer.CMS.Caches;

namespace SiteServer.API.Controllers
{
    [RoutePrefix("test")]
    public class TestController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetAdminOnly()
        {
            try
            {
                var rest = new Rest(Request);

                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.Admin))
                {
                    return Unauthorized();
                }

                return Ok(new
                {
                    Value = true
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        //[HttpGet]
        //public IHttpActionResult Get()
        //{
        //    var pluginIds = new List<string>();
        //    foreach (var pluginInfo in PluginManager.PluginInfoListRunnable)
        //    {
        //        if (!pluginInfo.IsDisabled)
        //        {
        //            pluginIds.Add(pluginInfo.Id);
        //        }
        //    }
        //    return Ok(new
        //    {
        //        DateTime = DateTime.Now,
        //        Plugins = pluginIds
        //    });
        //}

        //[HttpGet, Route("any")]
        //public HttpResponseMessage GetAny()
        //{
        //    //return Content(HttpStatusCode.Created, new
        //    //{
        //    //    IsOk = true
        //    //});

        //    //var content = ;

        //    var response = Request.CreateResponse(HttpStatusCode.NotFound);

        //    response.Content = new StringContent("yes, yes", Encoding.UTF8);

        //    return response;
        //}

        //[HttpGet, Route("string")]
        //public IHttpActionResult GetString()
        //{
        //    return Ok("Hello");
        //}

        //private readonly HttpClient _httpClient = new HttpClient();

        //[HttpGet, Route("test/count")]
        //public async Task<IHttpActionResult> GetDotNetCountAsync()
        //{
        //    // Suspends GetDotNetCountAsync() to allow the caller (the web server)
        //    // to accept another rest, rather than blocking on this one.
        //    var html = await _httpClient.GetStringAsync("http://dotnetfoundation.org");

        //    return Ok(new
        //    {
        //        Regex.Matches(html, @"\.NET").Count
        //    });
        //}
    }
}
