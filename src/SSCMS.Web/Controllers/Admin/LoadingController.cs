using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Dto;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LoadingController : ControllerBase
    {
        private const string Route = "loading";

        private readonly ISettingsManager _settingsManager;

        public LoadingController(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }

        [HttpPost, Route(Route)]
        public ActionResult<StringResult> Submit([FromBody] SubmitRequest request)
        {
            return new StringResult
            {
                Value = _settingsManager.Decrypt(request.RedirectUrl)
            };
        }
    }
}