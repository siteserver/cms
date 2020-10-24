using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.StlParser.Model;
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

            var dynamicInfo = DynamicInfo.GetDynamicInfo(_settingsManager, request.Value, request.Page, user, Request.Path + Request.QueryString);
            var ifInfo = TranslateUtils.JsonDeserialize<DynamicInfo.IfInfo>(dynamicInfo.ElementValues);

            var isSuccess = false;
            var html = string.Empty;

            if (ifInfo != null)
            {
                if (StringUtils.EqualsIgnoreCase(ifInfo.Type, StlIf.TypeIsUserLoggin))
                {
                    isSuccess = _authManager.IsUser;
                }

                var template = isSuccess ? dynamicInfo.SuccessTemplate : dynamicInfo.FailureTemplate;
                html = await StlDynamic.ParseDynamicContentAsync(_parseManager, dynamicInfo, template);
            }

            return new GetResult
            {
                Value = isSuccess,
                Html = html
            };
        }
    }
}
