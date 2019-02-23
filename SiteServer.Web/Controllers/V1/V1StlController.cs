using System;
using System.Web;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.RestRoutes.V1;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parsers;

namespace SiteServer.API.Controllers.V1
{
    [RoutePrefix("v1/stl")]
    public class V1StlController : ApiController
    {
        private const string Route = "{elementName}";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get(string elementName)
        {
            try
            {
#pragma warning disable CS0612 // '“RequestImpl”已过时
                var request = new RequestImpl(HttpContext.Current.Request);
#pragma warning restore CS0612 // '“RequestImpl”已过时

                var stlRequest = new StlRequest(request);

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
