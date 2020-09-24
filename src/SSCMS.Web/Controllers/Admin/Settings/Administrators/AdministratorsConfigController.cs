using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class AdministratorsConfigController : ControllerBase
    {
        private const string Route = "settings/administratorsConfig";

        private readonly IAuthManager _authManager;
        private readonly IConfigRepository _configRepository;

        public AdministratorsConfigController(IAuthManager authManager, IConfigRepository configRepository)
        {
            _authManager = authManager;
            _configRepository = configRepository;
        }

        public class GetResult
        {
            public Config Config { get; set; }
        }

        public class SubmitRequest
        {
            public int AdminUserNameMinLength { get; set; }
            public int AdminPasswordMinLength { get; set; }
            public PasswordRestriction AdminPasswordRestriction { get; set; }
            public bool IsAdminLockLogin { get; set; }
            public int AdminLockLoginCount { get; set; }
            public LockType AdminLockLoginType { get; set; }
            public int AdminLockLoginHours { get; set; }
            public bool IsAdminEnforcePasswordChange { get; set; }
            public int AdminEnforcePasswordChangeDays { get; set; }
            public bool IsAdminEnforceLogout { get; set; }
            public int AdminEnforceLogoutMinutes { get; set; }
        }
    }
}
