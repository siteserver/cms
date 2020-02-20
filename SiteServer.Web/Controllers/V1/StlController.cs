using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.API.Context;
using SiteServer.CMS.Framework;
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
            var request = await AuthenticatedRequest.GetAuthAsync();
            var isApiAuthorized = request.IsApiAuthenticated && await DataProvider.AccessTokenRepository.IsScopeAsync(request.ApiToken, Constants.ScopeStl);

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
    }
}
