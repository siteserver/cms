using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Settings.Utilities
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class UtilitiesConfigController : ControllerBase
    {
        private const string Route = "settings/utilitiesConfig";

        private readonly IAuthManager _authManager;
        private readonly IConfigRepository _configRepository;

        public UtilitiesConfigController(IAuthManager authManager, IConfigRepository configRepository)
        {
            _authManager = authManager;
            _configRepository = configRepository;
        }

        /// <summary>
        /// 仅返回页面所需字段，避免泄露 CloudToken 等敏感配置
        /// </summary>
        public class ConfigDto
        {
            public bool IsMaterialSiteOnly { get; set; }
        }

        public class GetResult
        {
            public ConfigDto Config { get; set; }
        }

        public class SubmitRequest
        {
            public bool IsMaterialSiteOnly { get; set; }
        }
    }
}
