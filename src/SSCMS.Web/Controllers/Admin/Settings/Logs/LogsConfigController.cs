using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Settings.Logs
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LogsConfigController : ControllerBase
    {
        private const string Route = "settings/logsConfig";

        private readonly IAuthManager _authManager;
        private readonly IConfigRepository _configRepository;

        public LogsConfigController(IAuthManager authManager, IConfigRepository configRepository)
        {
            _authManager = authManager;
            _configRepository = configRepository;
        }

        /// <summary>
        /// 仅返回页面所需字段，避免泄露 CloudToken 等敏感配置
        /// </summary>
        public class ConfigDto
        {
            public bool IsTimeThreshold { get; set; }
            public int TimeThreshold { get; set; }
            public bool IsLogSite { get; set; }
            public bool IsLogSiteCreate { get; set; }
            public bool IsLogAdmin { get; set; }
            public bool IsLogUser { get; set; }
            public bool IsLogError { get; set; }

            public static ConfigDto From(Config c) => new ConfigDto
            {
                IsTimeThreshold = c.IsTimeThreshold, TimeThreshold = c.TimeThreshold,
                IsLogSite = c.IsLogSite, IsLogSiteCreate = c.IsLogSiteCreate,
                IsLogAdmin = c.IsLogAdmin, IsLogUser = c.IsLogUser, IsLogError = c.IsLogError
            };
        }

        public class GetResult
        {
            public ConfigDto Config { get; set; }
        }

        public class SubmitRequest
        {
            public bool IsTimeThreshold { get; set; }
            public int TimeThreshold { get; set; }
            public bool IsLogSite { get; set; }
            public bool IsLogSiteCreate { get; set; }
            public bool IsLogAdmin { get; set; }
            public bool IsLogUser { get; set; }
            public bool IsLogError { get; set; }
        }
    }
}
