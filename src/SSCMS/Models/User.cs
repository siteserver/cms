using System;
using Datory;
using Datory.Annotations;
using Newtonsoft.Json;
using SSCMS.Enums;

namespace SSCMS.Models
{
    [DataTable("siteserver_User")]
    public class User : Entity
    {
        [DataColumn]
        public string UserName { get; set; }

        [DataColumn]
        [JsonIgnore]
        public string Password { get; set; }

        [DataColumn]
        [JsonIgnore]
        public PasswordFormat PasswordFormat { get; set; }

        [DataColumn]
        [JsonIgnore]
        public string PasswordSalt { get; set; }

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
        public bool Checked { get; set; }

        [DataColumn]
        public bool Locked { get; set; }

        [DataColumn]
        public string DisplayName { get; set; }

        [DataColumn]
        public string Mobile { get; set; }

        [DataColumn]
        public bool MobileVerified { get; set; }

        [DataColumn]
        public string Email { get; set; }

        [DataColumn]
        public string AvatarUrl { get; set; }
    }
}