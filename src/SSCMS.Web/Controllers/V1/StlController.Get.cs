using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.V1
{
    public partial class StlController
    {
        [OpenApiOperation("STL 模板语言 API", "使用GET发起请求，请求地址为/api/v1/stl/{elementName}，其中{elementName}为STL标签名称")]
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromRoute] string elementName, [FromQuery]GetRequest request)
        {
            if (!await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeStl))
            {
                return Unauthorized();
            }

            request.InitialParameters();

            var stlRequest = new StlRequest();
            await stlRequest.LoadAsync(_authManager, _pathManager, _configRepository, _siteRepository, request);

            var site = stlRequest.Site;

            if (site == null)
            {
                return NotFound();
            }

            _parseManager.ContextInfo = stlRequest.ContextInfo;
            _parseManager.PageInfo = stlRequest.PageInfo;

            elementName = $"stl:{StringUtils.ToLower(elementName)}";

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
