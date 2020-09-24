using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LoadingController : ControllerBase
    {
        private const string Route = "loading";

        private readonly ISettingsManager _settingsManager;

        public LoadingController(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }

        public class SubmitRequest
        {
            public string RedirectUrl { get; set; }
        }
    }
}