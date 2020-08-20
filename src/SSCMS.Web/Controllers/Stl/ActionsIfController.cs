using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Core.StlParser.Model;
using SSCMS.Core.StlParser.StlElement;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Stl
{
    [OpenApiIgnore]
    [Route(Constants.ApiPrefix + Constants.ApiStlPrefix)]
    public partial class ActionsIfController : ControllerBase
    {
        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IParseManager _parseManager;

        public ActionsIfController(ISettingsManager settingsManager, IAuthManager authManager, IParseManager parseManager)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _parseManager = parseManager;
        }

        [HttpPost, Route(Constants.RouteStlRouteActionsIf)]
        public async Task<SubmitResult> Submit([FromBody]SubmitRequest request)
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
                else if (StringUtils.EqualsIgnoreCase(ifInfo.Type, StlIf.TypeIsAdministratorLoggin))
                {
                    isSuccess = _authManager.IsAdmin;
                }
                else if (StringUtils.EqualsIgnoreCase(ifInfo.Type, StlIf.TypeIsUserOrAdministratorLoggin))
                {
                    isSuccess = _authManager.IsUser || _authManager.IsAdmin;
                }

                var template = isSuccess ? dynamicInfo.SuccessTemplate : dynamicInfo.FailureTemplate;
                html = await StlDynamic.ParseDynamicContentAsync(_parseManager, dynamicInfo, template);
            }

            return new SubmitResult
            {
                Value = isSuccess,
                Html = html
            };
        }
    }
}
