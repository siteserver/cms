using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Core.StlParser.Model;
using SSCMS.Core.StlParser.StlElement;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Stl
{
    [OpenApiIgnore]
    [Route(Constants.ApiPrefix + Constants.ApiStlPrefix)]
    public partial class ActionsDynamicController : ControllerBase
    {
        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IParseManager _parseManager;

        public ActionsDynamicController(ISettingsManager settingsManager, IAuthManager authManager, IParseManager parseManager)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _parseManager = parseManager;
        }

        [HttpPost, Route(Constants.RouteStlActionsDynamic)]
        public async Task<SubmitResult> Submit([FromBody]SubmitRequest request)
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
