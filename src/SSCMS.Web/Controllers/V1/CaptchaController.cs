using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.V1
{
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route(Constants.ApiV1Prefix)]
    public partial class CaptchaController : ControllerBase
    {
        private const string Route = "captcha";
        private const string RouteValue = "captcha/{value}";
        private const string RouteActionsCheck = "captcha/actions/check";

        private readonly ISettingsManager _settingsManager;

        public CaptchaController(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }

        public class CheckRequest
        {
            public string Captcha { get; set; }
            public string Value { get; set; }
        }
    }
}
