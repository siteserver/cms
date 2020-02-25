using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Framework;
using SS.CMS.StlParser.Parsers;

namespace SS.CMS.Web.Controllers.V1
{
    [Route("v1/stl")]
    public partial class StlController : ControllerBase
    {
        private const string Route = "{elementName}";

        private readonly IAuthManager _authManager;

        public StlController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery]GetRequest request)
        {
            var auth = await _authManager.GetApiAsync();

            var isApiAuthorized = auth.IsApiAuthenticated && await DataProvider.AccessTokenRepository.IsScopeAsync(auth.ApiToken, Constants.ScopeStl);

            var stlRequest = new StlRequest();
            await stlRequest.LoadAsync(auth, isApiAuthorized, request);

            if (!stlRequest.IsApiAuthorized)
            {
                return Unauthorized();
            }

            var site = stlRequest.Site;

            if (site == null)
            {
                return NotFound();
            }

            var elementName = $"stl:{request.ElementName.ToLower()}";

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

            return new GetResult
            {
                Value = value
            };
        }
    }
}
