using System;
using Datory;
using Datory.Annotations;
using Newtonsoft.Json;

namespace SiteServer.Abstractions
{
    [DataTable("siteserver_User")]
    public class User : Entity
    {
        [DataColumn]
        public string UserName { get; set; }

        [JsonIgnore]
        [DataColumn]
        public string Password { get; set; }

        [JsonIgnore]
        [DataColumn]
        public string PasswordFormat { get; set; }

        [JsonIgnore]
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

        public bool Checked
        {
            get => TranslateUtils.ToBool(IsChecked);
            set => IsChecked = value.ToString();
        }

        [DataColumn]
        public string IsLockedOut { get; set; }

        public bool Locked
        {
            get => TranslateUtils.ToBool(IsLockedOut);
            set => IsLockedOut = value.ToString();
        }

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