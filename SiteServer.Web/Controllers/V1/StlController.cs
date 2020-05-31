using System;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Api.V1;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parsers;

namespace SiteServer.API.Controllers.V1
{
    [RoutePrefix("v1/stl")]
    public class StlController : ApiController
    {
        private const string Route = "{elementName}";

        [OpenApiOperation("STL 模板语言 API", "https://sscms.com/docs/v6/api/guide/stl/")]
        [HttpGet, Route(Route)]
        public IHttpActionResult Get(string elementName)
        {
            try
            {
                var stlRequest = new StlRequest();

                if (!stlRequest.IsApiAuthorized)
                {
                    return Unauthorized();
                }

                var siteInfo = stlRequest.SiteInfo;

                if (siteInfo == null)
                {
                    return NotFound();
                }

                elementName = $"stl:{elementName.ToLower()}";

                object value = null;

                if (StlElementParser.ElementsToParseDic.ContainsKey(elementName))
                {
                    Func<PageInfo, ContextInfo, object> func;
                    if (StlElementParser.ElementsToParseDic.TryGetValue(elementName, out func))
                    {
                        var obj = func(stlRequest.PageInfo, stlRequest.ContextInfo);

                        if (obj is string)
                        {
                            value = (string)obj;
                        }
                        else
                        {
                            value = obj;
                        }
                    }
                }

                return Ok(new
                {
                    Value = value
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }
    }
}
