using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Settings.Home
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class HomeConfigController : ControllerBase
    {
        private const string Route = "settings/homeConfig";
        private const string RouteUpload = "settings/homeConfig/actions/upload";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IConfigRepository _configRepository;
        private readonly ITableStyleRepository _tableStyleRepository;

        public HomeConfigController(IAuthManager authManager, IPathManager pathManager, IConfigRepository configRepository, ITableStyleRepository tableStyleRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _configRepository = configRepository;
            _tableStyleRepository = tableStyleRepository;
        }

        /// <summary>
        /// 仅返回页面所需字段，避免泄露 CloudToken 等敏感配置
        /// </summary>
        public class ConfigDto : SubmitRequest
        {
            public static ConfigDto From(Config c) => new ConfigDto
            {
                IsHomeClosed = c.IsHomeClosed, HomeTitle = c.HomeTitle, IsHomeLogo = c.IsHomeLogo,
                HomeLogoUrl = c.HomeLogoUrl, HomeDefaultAvatarUrl = c.HomeDefaultAvatarUrl,
                UserRegistrationAttributes = c.UserRegistrationAttributes,
                IsUserRegistrationMobile = c.IsUserRegistrationMobile, IsUserRegistrationEmail = c.IsUserRegistrationEmail,
                IsUserRegistrationGroup = c.IsUserRegistrationGroup, IsUserRegistrationDisplayName = c.IsUserRegistrationDisplayName,
                IsHomeAgreement = c.IsHomeAgreement, HomeAgreementHtml = c.HomeAgreementHtml, HomeWelcomeHtml = c.HomeWelcomeHtml
            };
        }

        public class GetResult
        {
            public ConfigDto Config { get; set; }
            public List<TableStyle> Styles { get; set; }
        }

        public class SubmitRequest
        {
            public bool IsHomeClosed { get; set; }
            public string HomeTitle { get; set; }
            public bool IsHomeLogo { get; set; }
            public string HomeLogoUrl { get; set; }
            public string HomeDefaultAvatarUrl { get; set; }
            public List<string> UserRegistrationAttributes { get; set; }
            public bool IsUserRegistrationMobile { get; set; }
            public bool IsUserRegistrationEmail { get; set; }
            public bool IsUserRegistrationGroup { get; set; }
            public bool IsUserRegistrationDisplayName { get; set; }
            public bool IsHomeAgreement { get; set; }
            public string HomeAgreementHtml { get; set; }
            public string HomeWelcomeHtml { get; set; }
        }
    }
}
