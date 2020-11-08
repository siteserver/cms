using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class UsersConfigController : ControllerBase
    {
        private const string Route = "settings/usersConfig";

        private readonly IAuthManager _authManager;
        private readonly ISmsManager _smsManager;
        private readonly IConfigRepository _configRepository;

        public UsersConfigController(IAuthManager authManager, ISmsManager smsManager, IConfigRepository configRepository)
        {
            _authManager = authManager;
            _smsManager = smsManager;
            _configRepository = configRepository;
        }

        public class GetResult
        {
            public bool IsSmsEnabled { get; set; }
            public Config Config { get; set; }
        }

        public class SubmitRequest
        {
            public bool IsUserRegistrationAllowed { get; set; }
            public bool IsUserRegistrationChecked { get; set; }
            public bool IsUserUnRegistrationAllowed { get; set; }
            public bool IsUserForceVerifyMobile { get; set; }
            public int UserPasswordMinLength { get; set; }
            public PasswordRestriction UserPasswordRestriction { get; set; }
            public int UserRegistrationMinMinutes { get; set; }
            public bool IsUserLockLogin { get; set; }
            public int UserLockLoginCount { get; set; }
            public string UserLockLoginType { get; set; }
            public int UserLockLoginHours { get; set; }
        }
    }
}
