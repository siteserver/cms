using System;
using Newtonsoft.Json;
using SS.CMS.Data;

namespace SS.CMS.Models
{
    [Serializable]
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

        [JsonIgnore]
        [DataColumn]
        public string RoleName { get; set; }

        /// <summary>
        /// 最后一次重设密码时间。
        /// </summary>
        [DataColumn]
        public DateTime? LastResetPasswordDate { get; set; }

        [DataColumn]
        public DateTime? LastActivityDate { get; set; }

        /// <summary>
        /// 用户组Id。
        /// </summary>
        [DataColumn]
        public int GroupId { get; set; }

        [DataColumn]
        public int CountOfLogin { get; set; }

        [DataColumn]
        public int CountOfFailedLogin { get; set; }

        [DataColumn]
        public int CreatorUserId { get; set; }

        [DataColumn]
        public bool IsLockedOut { get; set; }

        /// <summary>
        /// 是否已审核用户。
        /// </summary>
        [DataColumn]
        public bool IsChecked { get; set; }

        [DataColumn(Text = true)]
        public string SiteIdCollection { get; set; }

        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public int DepartmentId { get; set; }

        [DataColumn]
        public int AreaId { get; set; }

        [DataColumn]
        public string DisplayName { get; set; }

        [DataColumn]
        public string Mobile { get; set; }

        [DataColumn]
        public string Email { get; set; }

        [DataColumn(Length = 1000)]
        public string AvatarUrl { get; set; }

        [DataColumn(Text = true)]
        public string Bio { get; set; }

        [DataColumn(Text = true, Extend = true)]
        public string ExtendValues { get; set; }
    }
}
