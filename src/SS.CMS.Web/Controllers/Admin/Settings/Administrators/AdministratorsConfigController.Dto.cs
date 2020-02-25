using SS.CMS.Abstractions;

namespace SS.CMS.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsConfigController
    {
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
