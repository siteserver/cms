using System;
using SS.CMS.Data;

namespace SS.CMS.Models
{
    [Serializable]
    [DataTable("siteserver_Config")]
    public partial class Config : Entity
    {
        [DataColumn]
        public string DatabaseVersion { get; set; }

        [DataColumn]
        public DateTime? UpdateDate { get; set; }

        [DataColumn(Text = true, Extend = true)]
        public string ExtendValues { get; set; }

        public bool IsLogSite { get; set; } = true;

        public bool IsLogAdmin { get; set; } = true;

        public bool IsLogUser { get; set; } = true;

        public bool IsLogError { get; set; } = true;

        public bool IsViewContentOnlySelf { get; set; }

        public bool IsTimeThreshold { get; set; }

        public int TimeThreshold { get; set; } = 60;

        public int AdminUserNameMinLength { get; set; }

        public int AdminPasswordMinLength { get; set; } = 6;

        public string AdminPasswordRestriction { get; set; } = "LetterAndDigit";

        public bool IsAdminLockLogin { get; set; }

        public int AdminLockLoginCount { get; set; } = 3;

        public string AdminLockLoginType { get; set; } = "Hours";

        public int AdminLockLoginHours { get; set; } = 3;

        public bool IsUserRegistrationAllowed { get; set; } = true;

        public string UserRegistrationAttributes { get; set; }

        public bool IsUserRegistrationGroup { get; set; }

        public bool IsUserRegistrationChecked { get; set; } = true;

        public bool IsUserUnRegistrationAllowed { get; set; } = true;

        public int UserPasswordMinLength { get; set; } = 6;

        public string UserPasswordRestriction { get; set; } = "LetterAndDigit";

        public int UserRegistrationMinMinutes { get; set; }

        public bool IsUserLockLogin { get; set; }

        public int UserLockLoginCount { get; set; } = 3;

        public string UserLockLoginType { get; set; } = "Hours";

        public int UserLockLoginHours { get; set; } = 3;

        public string UserDefaultGroupAdminName { get; set; }

        public string RepositoryOwner { get; set; }

        public string RepositoryName { get; set; }

        public string RepositoryToken { get; set; }
    }
}
