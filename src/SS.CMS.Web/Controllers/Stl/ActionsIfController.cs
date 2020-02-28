using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Api.Stl;
using SS.CMS.StlParser.Model;
using SS.CMS.StlParser.StlElement;

namespace SS.CMS.Web.Controllers.Stl
{
    public partial class ActionsIfController : ControllerBase
    {
        private readonly IAuthManager _authManager;
        private readonly IParseManager _parseManager;

        public ActionsIfController(IAuthManager authManager, IParseManager parseManager)
        {
            _authManager = authManager;
            _parseManager = parseManager;
        }

        [HttpPost, Route(ApiRouteActionsIf.Route)]
        public async Task<SubmitResult> Submit([FromBody]SubmitRequest request)
        {
            var auth = await _authManager.GetUserAsync();

            var dynamicInfo = DynamicInfo.GetDynamicInfo(request.Value, request.Page, auth.User, Request.Path + Request.QueryString);
            var ifInfo = TranslateUtils.JsonDeserialize<DynamicInfo.IfInfo>(dynamicInfo.ElementValues);

            var isSuccess = false;
            var html = string.Empty;

            if (ifInfo != null)
            {
                if (StringUtils.EqualsIgnoreCase(ifInfo.Type, StlIf.TypeIsUserLoggin))
                {
                    isSuccess = auth.IsUserLoggin;
                }
                else if (StringUtils.EqualsIgnoreCase(ifInfo.Type, StlIf.TypeIsAdministratorLoggin))
                {
                    isSuccess = auth.IsAdminLoggin;
                }
                else if (StringUtils.EqualsIgnoreCase(ifInfo.Type, StlIf.TypeIsUserOrAdministratorLoggin))
                {
                    isSuccess = auth.IsUserLoggin || auth.IsAdminLoggin;
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
