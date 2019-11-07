using System;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.CMS.Api.V1;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parsers;

namespace SiteServer.API.Controllers.V1
{
    [RoutePrefix("v1/stl")]
    public class StlController : ApiController
    {
        private const string Route = "{elementName}";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> Get(string elementName)
        {
            try
            {
                var request = new AuthenticatedRequest();
                var isApiAuthorized = request.IsApiAuthenticated && await AccessTokenManager.IsScopeAsync(request.ApiToken, AccessTokenManager.ScopeStl);

                var stlRequest = new StlRequest(request, isApiAuthorized);

                if (!stlRequest.IsApiAuthorized)
                {
                    return Unauthorized();
                }

                var site = stlRequest.Site;

                if (site == null)
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
