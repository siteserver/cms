using System;
using System.Web.Http;
using SiteServer.CMS.Api.V1;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parsers;

namespace SiteServer.API.Controllers.V1
{
    [RoutePrefix("api")]
    public class StlController : ApiController
    {
        [HttpGet, Route(ApiStlRoute.Route)]
        public IHttpActionResult Get(string elementName)
        {
            try
            {
                var manager = new ApiStlManager();

                if (!manager.IsAuthorized)
                {
                    return Unauthorized();
                }

                var siteInfo = manager.SiteInfo;

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
                        var obj = func(manager.PageInfo, manager.ContextInfo);

                        if (obj == null)
                        {
                            value = string.Empty;
                        }
                        else if (obj is string)
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
                return BadRequest(ex.Message);
            }
        }
    }
}
