using System;
using System.Collections.Generic;
using Datory;
using Datory.Annotations;
using Newtonsoft.Json;
using SSCMS.Enums;

namespace SSCMS.Models
{
    [DataTable("siteserver_Administrator")]
    public class Administrator : Entity
    {
        [DataColumn]
        public string UserName { get; set; }

        [JsonIgnore]
        [DataColumn]
        public string Password { get; set; }

        [JsonIgnore]
        [DataColumn]
        public PasswordFormat PasswordFormat { get; set; }

        [JsonIgnore]
        [DataColumn]
        public string PasswordSalt { get; set; }

        [DataColumn]
        public DateTime? LastActivityDate { get; set; }

        [DataColumn]
        public DateTime? LastChangePasswordDate { get; set; }

        [DataColumn]
        public int CountOfLogin { get; set; }

        [DataColumn]
        public int CountOfFailedLogin { get; set; }

        [DataColumn]
        public string CreatorUserName { get; set; }

        [DataColumn]
        public bool Locked { get; set; }

        [DataColumn]
        public List<int> SiteIds { get; set; }

        [DataColumn]
        public int SiteId { get; set; }

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
