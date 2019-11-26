using System;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.CMS.Api.V1;
using SiteServer.CMS.Caching;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.StlParser.Parsers;
using SiteServer.Utils;

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
                var request = await AuthenticatedRequest.GetAuthAsync();
                var isApiAuthorized = request.IsApiAuthenticated && await DataProvider.AccessTokenDao.IsScopeAsync(request.ApiToken, Constants.ScopeStl);

                var stlRequest = new StlRequest();
                await stlRequest.LoadAsync(request, isApiAuthorized);

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
                    if (StlElementParser.ElementsToParseDic.TryGetValue(elementName, out var func))
                    {
                        var obj = await func(stlRequest.PageInfo, stlRequest.ContextInfo);

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
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }
    }
}
