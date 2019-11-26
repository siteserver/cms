using System;
using System.Collections.Generic;
using Datory;
using Newtonsoft.Json;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Model
{
    [DataTable("siteserver_Administrator")]
    public class Administrator : Entity, IAdministrator
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
        public DateTime? CreationDate { get; set; }

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

        [JsonIgnore]
        [DataColumn]
        public string IsLockedOut { get; set; }

        public bool Locked
        {
            get => TranslateUtils.ToBool(IsLockedOut);
            set => IsLockedOut = value.ToString();
        }

        [JsonIgnore]
        [DataColumn]
        private string SiteIdCollection { get; set; }

        public List<int> SiteIds
        {
            get => TranslateUtils.StringCollectionToIntList(SiteIdCollection);
            set => SiteIdCollection = string.Join(",", SiteIdCollection);
        }

        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public string DisplayName { get; set; }

        [DataColumn]
        public string Mobile { get; set; }

        [DataColumn]
        public string Email { get; set; }

        [DataColumn]
        public string AvatarUrl { get; set; }
    }
}
