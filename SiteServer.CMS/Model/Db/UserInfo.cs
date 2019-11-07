using System;
using Datory;

namespace SiteServer.CMS.Model.Db
{
    [DataTable("siteserver_User")]
    public class UserInfo : Entity
    {
        [DataColumn]
        public string UserName { get; set; }

        [DataColumn]
        public string Password { get; set; }

        [DataColumn]
        public string PasswordFormat { get; set; }

        [DataColumn]
        public string PasswordSalt { get; set; }

        [DataColumn]
        public DateTime? CreateDate { get; set; }

        [DataColumn]
        public DateTime? LastResetPasswordDate { get; set; }

        [DataColumn]
        public DateTime? LastActivityDate { get; set; }

        [DataColumn]
        public int GroupId { get; set; }

        [DataColumn]
        public int CountOfLogin { get; set; }

        [DataColumn]
        public int CountOfFailedLogin { get; set; }

        [DataColumn]
        public string IsChecked { get; set; }

        [DataColumn]
        public string IsLockedOut { get; set; }

        [DataColumn]
        public string DisplayName { get; set; }

        [DataColumn]
        public string Mobile { get; set; }

        [DataColumn]
        public string Email { get; set; }

        [DataColumn]
        public string AvatarUrl { get; set; }

        [DataColumn]
        public string Gender { get; set; }

        [DataColumn]
        public string Birthday { get; set; }

        [DataColumn]
        public string WeiXin { get; set; }

        [DataColumn]
        public string Qq { get; set; }

        [DataColumn]
        public string WeiBo { get; set; }

        [DataColumn]
        public string Bio { get; set; }

        [DataColumn(Extend = true, Text = true)]
        public string SettingsXml { get; set; }
    }
}
