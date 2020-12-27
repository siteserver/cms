using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.StlParser.Model;
using SSCMS.Core.StlParser.StlElement;

namespace SSCMS.Web.Controllers.Stl
{
    public partial class ActionsDynamicController
    {
        [HttpPost, Route(Constants.RouteStlActionsDynamic)]
        public async Task<SubmitResult> Submit([FromBody] SubmitRequest request)
        {
            var user = await _authManager.GetUserAsync();

            var dynamicInfo = DynamicInfo.GetDynamicInfo(_settingsManager, request.Value, request.Page, user, Request.Path + Request.QueryString);

            return new SubmitResult
            {
                Value = true,
                Html = await StlDynamic.ParseDynamicContentAsync(_parseManager, dynamicInfo, dynamicInfo.SuccessTemplate)
            };
        }
    }
}
