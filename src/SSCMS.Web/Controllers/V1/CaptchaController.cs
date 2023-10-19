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
        private readonly ICacheManager _cacheManager;

        public CaptchaController(ISettingsManager settingsManager, ICacheManager cacheManager)
        {
            _settingsManager = settingsManager;
            _cacheManager = cacheManager;
        }

        public class CheckRequest
        {
            public string Captcha { get; set; }
            public string Value { get; set; }
        }
    }
}
