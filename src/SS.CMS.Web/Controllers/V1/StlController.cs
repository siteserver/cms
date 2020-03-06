using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;

namespace SS.CMS.Web.Controllers.V1
{
    [Route("v1/stl")]
    public partial class StlController : ControllerBase
    {
        private const string Route = "{elementName}";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IParseManager _parseManager;
        private readonly IConfigRepository _configRepository;
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly ISiteRepository _siteRepository;

        public StlController(IAuthManager authManager, IPathManager pathManager, IParseManager parseManager, IConfigRepository configRepository, IAccessTokenRepository accessTokenRepository, ISiteRepository siteRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _parseManager = parseManager;
            _configRepository = configRepository;
            _accessTokenRepository = accessTokenRepository;
            _siteRepository = siteRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery]GetRequest request)
        {
            var auth = await _authManager.GetApiAsync();

            var isApiAuthorized = auth.IsApiAuthenticated && await _accessTokenRepository.IsScopeAsync(auth.ApiToken, Constants.ScopeStl);

            var stlRequest = new StlRequest();
            await stlRequest.LoadAsync(auth, _pathManager, _configRepository, _siteRepository, isApiAuthorized, request);

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
