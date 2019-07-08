using System;
using Newtonsoft.Json;
using SS.CMS.Data;

namespace SS.CMS.Models
{
    [Serializable]
    [Table("siteserver_User")]
    public class UserInfo : Entity
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

        [JsonIgnore]
        [TableColumn]
        public string RoleName { get; set; }

        /// <summary>
        /// 最后一次重设密码时间。
        /// </summary>
        [TableColumn]
        public DateTime? LastResetPasswordDate { get; set; }

        [TableColumn]
        public DateTime? LastActivityDate { get; set; }

        /// <summary>
        /// 用户组Id。
        /// </summary>
        [TableColumn]
        public int GroupId { get; set; }

        [TableColumn]
        public int CountOfLogin { get; set; }

        [TableColumn]
        public int CountOfFailedLogin { get; set; }

        [TableColumn]
        public int CreatorUserId { get; set; }

        [TableColumn]
        public bool IsLockedOut { get; set; }

        /// <summary>
        /// 是否已审核用户。
        /// </summary>
        [TableColumn]
        public bool IsChecked { get; set; }

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

        [TableColumn(Text = true)]
        public string Bio { get; set; }

        [TableColumn(Text = true, Extend = true)]
        public string ExtendValues { get; set; }
    }
}
