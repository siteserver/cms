using System;
using Newtonsoft.Json;
using SiteServer.CMS.Database.Wrapper;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_User")]
    public class UserInfo : DynamicEntity, IUserInfo
    {
        /// <summary>
        /// 用户名。
        /// </summary>
        [TableColumn]
        public string UserName { get; set; }

        /// <summary>
        /// 密码。
        /// </summary>
        [TableColumn]
        [JsonIgnore]
        public string Password { get; set; }

        /// <summary>
        /// 加密格式。
        /// </summary>
        [TableColumn]
        [JsonIgnore]
        public string PasswordFormat { get; set; }

        /// <summary>
        /// 秘钥。
        /// </summary>
        [TableColumn]
        [JsonIgnore]
        public string PasswordSalt { get; set; }

        /// <summary>
        /// 创建时间。
        /// </summary>
        [TableColumn]
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 最后一次重设密码时间。
        /// </summary>
        [TableColumn]
        public DateTime? LastResetPasswordDate { get; set; }

        /// <summary>
        /// 最后活动时间。
        /// </summary>
        [TableColumn]
        public DateTime? LastActivityDate { get; set; }

        /// <summary>
        /// 用户组Id。
        /// </summary>
        [TableColumn]
        public int GroupId { get; set; }

        /// <summary>
        /// 登录次数。
        /// </summary>
        [TableColumn]
        public int CountOfLogin { get; set; }

        /// <summary>
        /// 连续登录失败次数。
        /// </summary>
        [TableColumn]
        public int CountOfFailedLogin { get; set; }

        /// <summary>
        /// 是否已审核用户。
        /// </summary>
        [TableColumn]
        private string IsChecked { get; set; }

        public bool Checked
        {
            get => IsChecked == "True";
            set => IsChecked = value.ToString();
        }

        /// <summary>
        /// 是否被锁定。
        /// </summary>
        [TableColumn]
        private string IsLockedOut { get; set; }

        public bool Locked
        {
            get => IsLockedOut == "True";
            set => IsLockedOut = value.ToString();
        }

        /// <summary>
        /// 姓名。
        /// </summary>
        [TableColumn]
        public string DisplayName { get; set; }

        /// <summary>
        /// 手机号。
        /// </summary>
        [TableColumn]
        public string Mobile { get; set; }

        /// <summary>
        /// 邮箱。
        /// </summary>
        [TableColumn]
        public string Email { get; set; }

        /// <summary>
        /// 头像图片路径。
        /// </summary>
        [TableColumn]
        public string AvatarUrl { get; set; }

        /// <summary>
        /// 性别。
        /// </summary>
        [TableColumn]
        public string Gender { get; set; }

        /// <summary>
        /// 出生日期。
        /// </summary>
        [TableColumn]
        public string Birthday { get; set; }

        /// <summary>
        /// 微信。
        /// </summary>
        [TableColumn]
        public string WeiXin { get; set; }

        /// <summary>
        /// QQ。
        /// </summary>
        [TableColumn]
        public string Qq { get; set; }

        /// <summary>
        /// 微博。
        /// </summary>
        [TableColumn]
        public string WeiBo { get; set; }

        /// <summary>
        /// 简介。
        /// </summary>
        [TableColumn(Text = true)]
        public string Bio { get; set; }

        /// <summary>
        /// 附加字段。
        /// </summary>
        [TableColumn(Text = true, Extend = true)]
        public string SettingsXml { get; set; }

        //public Dictionary<string, object> ToDictionary()
        //{
        //    var dict = base.ToDictionary();

        //    var styleInfoList = TableStyleManager.GetUserStyleInfoList();

        //    foreach (var styleInfo in styleInfoList)
        //    {
        //        dict.Remove(styleInfo.AttributeName);
        //        dict[styleInfo.AttributeName] = Get(styleInfo.AttributeName);
        //    }

        //    foreach (var attributeName in UserAttribute.AllAttributes.Value)
        //    {
        //        if (StringUtils.StartsWith(attributeName, "Is"))
        //        {
        //            dict.Remove(attributeName);
        //            dict[attributeName] = GetBool(attributeName);
        //        }
        //        else
        //        {
        //            dict.Remove(attributeName);
        //            dict[attributeName] = Get(attributeName);
        //        }
        //    }

        //    foreach (var attributeName in UserAttribute.ExcludedAttributes.Value)
        //    {
        //        dict.Remove(attributeName);
        //    }

        //    return dict;
        //}
    }
}
