using System;
using Datory;
using Newtonsoft.Json;
using SiteServer.Plugin;

namespace SiteServer.CMS.Model
{
    [Table("siteserver_Administrator")]
    public class AdministratorInfo : Entity, IAdministratorInfo
    {
        [TableColumn]
        public string UserName { get; set; }

        [JsonIgnore]
        [TableColumn]
        public string Password { get; set; }

        [JsonIgnore]
        [TableColumn]
        public string PasswordFormat { get; set; }

        [JsonIgnore]
        [TableColumn]
        public string PasswordSalt { get; set; }

        [TableColumn]
        public DateTime? CreationDate { get; set; }

        [TableColumn]
        public DateTime? LastActivityDate { get; set; }

        [TableColumn]
        public int CountOfLogin { get; set; }

        [TableColumn]
        public int CountOfFailedLogin { get; set; }

        [TableColumn]
        public string CreatorUserName { get; set; }

        [TableColumn]
        private string IsLockedOut { get; set; }

        public bool Locked
        {
            get => IsLockedOut == "True";
            set => IsLockedOut = value.ToString();
        }

        [TableColumn(Text = true)]
        public string SiteIdCollection { get; set; }

        [TableColumn]
        public int SiteId { get; set; }

        [TableColumn]
        public int DepartmentId { get; set; }

        [TableColumn]
        public int AreaId { get; set; }

        [TableColumn]
        public string DisplayName { get; set; }

        [TableColumn]
        public string Mobile { get; set; }

        [TableColumn]
        public string Email { get; set; }

        [TableColumn(Length = 1000)]
        public string AvatarUrl { get; set; }
    }
}
