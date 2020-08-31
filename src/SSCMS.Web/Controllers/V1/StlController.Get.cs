using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.V1
{
    public partial class StlController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery]GetRequest request)
        {
            var isApiAuthorized = await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeStl);

            var stlRequest = new StlRequest();
            await stlRequest.LoadAsync(_authManager, _pathManager, _configRepository, _siteRepository, isApiAuthorized, request);

            if (!stlRequest.IsApiAuthorized)
            {
                return Unauthorized();
            }

            var site = stlRequest.Site;

            if (site == null)
            {
                return NotFound();
            }

            var elementName = $"stl:{StringUtils.ToLower(request.ElementName)}";

            object value = null;

            if (_parseManager.ElementsToParseDic.ContainsKey(elementName))
            {
                if (_parseManager.ElementsToParseDic.TryGetValue(elementName, out var func))
                {
                    var obj = await func(_parseManager);

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
