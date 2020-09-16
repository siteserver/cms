using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Home
{
    [OpenApiIgnore]
    [Route(Constants.ApiHomePrefix)]
    public partial class RegisterController : ControllerBase
    {
        private const string Route = "register";
        private const string RouteCaptcha = "register/captcha";
        private const string RouteCheckCaptcha = "register/captcha/actions/check";

        private readonly ISettingsManager _settingsManager;
        private readonly IConfigRepository _configRepository;
        private readonly ITableStyleRepository _tableStyleRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly IStatRepository _statRepository;

        public RegisterController(ISettingsManager settingsManager, IConfigRepository configRepository, ITableStyleRepository tableStyleRepository, IUserRepository userRepository, IUserGroupRepository userGroupRepository, IStatRepository statRepository)
        {
            _settingsManager = settingsManager;
            _configRepository = configRepository;
            _tableStyleRepository = tableStyleRepository;
            _userRepository = userRepository;
            _userGroupRepository = userGroupRepository;
            _statRepository = statRepository;
        }

        public class GetResult
        {
            public bool IsUserRegistrationGroup { get; set; }
            public bool IsHomeAgreement { get; set; }
            public string HomeAgreementHtml { get; set; }
            public IEnumerable<InputStyle> Styles { get; set; }
            public IEnumerable<UserGroup> Groups { get; set; }
        }

        public class CheckRequest
        {
            public string Token { get; set; }
            public string Value { get; set; }
        }
    }
}
