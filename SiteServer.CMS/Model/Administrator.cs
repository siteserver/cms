using System;
using SiteServer.Plugin;

namespace SiteServer.CMS.Model
{
    public class Administrator : IAdministratorInfo
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public DateTime? CreationDate { get; set; }

        public DateTime? LastActivityDate { get; set; }

        public DateTime? LastChangePasswordDate { get; set; }

        public int CountOfLogin { get; set; }

        public int CountOfFailedLogin { get; set; }

        public string CreatorUserName { get; set; }

        public bool Locked { get; set; }

        public string SiteIdCollection { get; set; }

        public int SiteId { get; set; }

        public string DisplayName { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }

        public string AvatarUrl { get; set; }
    }
}
