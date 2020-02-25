using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Api.Stl;
using SS.CMS.StlParser.Model;
using SS.CMS.StlParser.StlElement;

namespace SS.CMS.Web.Controllers.Stl
{
    public partial class ActionsDynamicController : ControllerBase
    {
        private readonly IAuthManager _authManager;

        public ActionsDynamicController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpPost, Route(ApiRouteActionsDynamic.Route)]
        public async Task<SubmitResult> Submit([FromBody]SubmitRequest request)
        {
            var auth = await _authManager.GetUserAsync();

            var dynamicInfo = DynamicInfo.GetDynamicInfo(request.Value, request.Page, auth.User, Request.Path + Request.QueryString);

            return new SubmitResult
            {
                Value = true,
                Html = await StlDynamic.ParseDynamicContentAsync(dynamicInfo, dynamicInfo.SuccessTemplate)
            };
        }
    }
}
