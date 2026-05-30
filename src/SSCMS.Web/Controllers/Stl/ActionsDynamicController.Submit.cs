using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.StlParser.StlElement;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Stl
{
    public partial class ActionsDynamicController
    {
        [HttpPost, Route(Constants.RouteStlActionsDynamic)]
        public async Task<ActionResult<SubmitResult>> Submit([FromBody] SubmitRequest request)
        {
            if (request == null)
            {
                return this.Error(Constants.ErrorNotFound);
            }

            if (!TryConsumeRequestQuota(PageUtils.GetIpAddress(Request), out var retryAfterSeconds))
            {
                return this.Error($"请求过于频繁，请在{retryAfterSeconds}秒后重试");
            }

            var user = await _authManager.GetUserAsync();

            var dynamicInfo = StlDynamic.GetDynamicInfo(_settingsManager, request.Value, request.Page, user, Request.Path + Request.QueryString);
            var template = dynamicInfo.YesTemplate;

            return new SubmitResult
            {
                Value = true,
                Html = await StlDynamic.ParseDynamicAsync(_parseManager, dynamicInfo, template)
            };
        }
    }
}
