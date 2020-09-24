using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Repositories;

namespace SSCMS.Web.Controllers.Home
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.User)]
    [Route(Constants.ApiHomePrefix)]
    public partial class DashboardController : ControllerBase
    {
        private const string Route = "dashboard";

        private readonly IConfigRepository _configRepository;

        public DashboardController(IConfigRepository configRepository)
        {
            _configRepository = configRepository;
        }

        public class GetResult
        {
            public string HomeWelcomeHtml { get; set; }
        }
    }
}
