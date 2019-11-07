using System;
using Datory;

namespace SiteServer.CMS.Model.Db
{
    [DataTable("siteserver_Administrator")]
    public class AdministratorInfo : Entity
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

        [DataColumn]
        public string IsLockedOut { get; set; }

        [DataColumn]
        public string SiteIdCollection { get; set; }

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
