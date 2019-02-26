using System;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using SiteServer.CMS.Database.Core;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_Administrator")]
    public class AdministratorInfo : IDataInfo, IAdministratorInfo
    {
        public int Id { get; set; }

        public string Guid { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string UserName { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        [JsonIgnore]
        public string PasswordFormat { get; set; }

        [JsonIgnore]
        public string PasswordSalt { get; set; }

        public DateTime? CreationDate { get; set; }

        public DateTime? LastActivityDate { get; set; }

        public int CountOfLogin { get; set; }

        public int CountOfFailedLogin { get; set; }

        public string CreatorUserName { get; set; }

        private string IsLockedOut { get; set; }

        [Computed]
        public bool Locked
        {
            get => TranslateUtils.ToBool(IsLockedOut);
            set => IsLockedOut = value.ToString();
        }

        public string SiteIdCollection { get; set; }

        public int SiteId { get; set; }

        public int DepartmentId { get; set; }

        public int AreaId { get; set; }

        public string DisplayName { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }

        public string AvatarUrl { get; set; }
    }
}
