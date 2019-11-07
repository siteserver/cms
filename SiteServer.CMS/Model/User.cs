using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;

namespace SiteServer.CMS.Model
{
    public class User : AttributesImpl, IUserInfo
    {
        public User()
        {

        }

        public User(IDataReader rdr) : base(rdr)
        {

        }

        public User(IDataRecord record) : base(record)
        {

        }

        public User(DataRowView view) : base(view)
        {

        }

        public User(DataRow row) : base(row)
        {

        }

        public User(Dictionary<string, object> dict) : base(dict)
        {

        }

        public User(NameValueCollection nvc) : base(nvc)
        {

        }

        public User(object anonymous) : base(anonymous)
        {

        }

        /// <summary>
        /// 用户Id。
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 用户名。
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 创建时间。
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 创建时间。
        /// </summary>
        public string PasswordFormat { get; set; }

        /// <summary>
        /// 创建时间。
        /// </summary>
        public string PasswordSalt { get; set; }

        /// <summary>
        /// 创建时间。
        /// </summary>
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 最后一次重设密码时间。
        /// </summary>
        public DateTime? LastResetPasswordDate { get; set; }

        /// <summary>
        /// 最后活动时间。
        /// </summary>
        public DateTime? LastActivityDate { get; set; }

        /// <summary>
        /// 用户组Id。
        /// </summary>
        public int GroupId { get; set; }

        public string GroupName { get; set; }

        /// <summary>
        /// 登录次数。
        /// </summary>
        public int CountOfLogin { get; set; }

        /// <summary>
        /// 连续登录失败次数。
        /// </summary>
        public int CountOfFailedLogin { get; set; }

        public bool Checked { get; set; }

        public bool Locked { get; set; }

        /// <summary>
        /// 姓名。
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 手机号。
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 邮箱。
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 头像图片路径。
        /// </summary>
        public string AvatarUrl { get; set; }

        /// <summary>
        /// 性别。
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// 出生日期。
        /// </summary>
        public string Birthday { get; set; }

        /// <summary>
        /// 微信。
        /// </summary>
        public string WeiXin { get; set; }

        /// <summary>
        /// QQ。
        /// </summary>
        public string Qq { get; set; }

        /// <summary>
        /// 微博。
        /// </summary>
        public string WeiBo { get; set; }

        /// <summary>
        /// 简介。
        /// </summary>
        public string Bio { get; set; }
    }
}
