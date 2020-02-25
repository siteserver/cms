using SS.CMS.Abstractions;

namespace SS.CMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersConfigController
    {
        public class GetResult
        {
            public Config Config { get; set; }
        }

        public class SubmitRequest
        {
            public bool IsUserRegistrationAllowed { get; set; }
            public bool IsUserRegistrationChecked { get; set; }
            public bool IsUserUnRegistrationAllowed { get; set; }
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
