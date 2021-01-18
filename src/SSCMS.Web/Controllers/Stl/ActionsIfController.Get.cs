using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.StlParser.Models;
using SSCMS.Core.StlParser.StlElement;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Stl
{
    public partial class ActionsIfController
    {
        [HttpPost, Route(Constants.RouteStlRouteActionsIf)]
        public async Task<GetResult> Get([FromBody] GetRequest request)
        {
            var user = await _authManager.GetUserAsync();

            var dynamicInfo = StlDynamic.GetDynamicInfo(_settingsManager, request.Value, request.Page, user, Request.Path + Request.QueryString);
            var ifInfo = TranslateUtils.JsonDeserialize<DynamicIfInfo>(dynamicInfo.Settings);

            var isSuccess = false;
            var html = string.Empty;

            if (ifInfo != null)
            {
                if (StringUtils.EqualsIgnoreCase(ifInfo.Type, StlIf.TypeIsUserLoggin))
                {
                    isSuccess = _authManager.IsUser;
                }

                var template = isSuccess ? dynamicInfo.YesTemplate : dynamicInfo.NoTemplate;
                html = await StlDynamic.ParseDynamicAsync(_parseManager, dynamicInfo, template);
            }

            return new GetResult
            {
                Value = isSuccess,
                Html = html
            };
        }
    }
}
