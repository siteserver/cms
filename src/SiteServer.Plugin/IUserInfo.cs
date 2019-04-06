using System;

namespace SiteServer.Plugin
{
    /// <inheritdoc />
    /// <summary>
    /// 用户实体接口。
    /// </summary>
    public interface IUserInfo
    {
        /// <summary>
        /// 用户Id。
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// 用户名。
        /// </summary>
        string UserName { get; set; }

        /// <summary>
        /// 创建时间。
        /// </summary>
        string Password { get; set; }

        /// <summary>
        /// 创建时间。
        /// </summary>
        string PasswordFormat { get; set; }

        /// <summary>
        /// 创建时间。
        /// </summary>
        string PasswordSalt { get; set; }

        /// <summary>
        /// 创建时间。
        /// </summary>
        DateTime? CreateDate { get; set; }

        /// <summary>
        /// 最后一次重设密码时间。
        /// </summary>
        DateTime? LastResetPasswordDate { get; set; }

        /// <summary>
        /// 最后活动时间。
        /// </summary>
        DateTime? LastActivityDate { get; set; }

        /// <summary>
        /// 登录次数。
        /// </summary>
        int CountOfLogin { get; set; }

        /// <summary>
        /// 连续登录失败次数。
        /// </summary>
        int CountOfFailedLogin { get; set; }

        /// <summary>
        /// 用户组Id。
        /// </summary>
        int GroupId { get; set; }

        /// <summary>
        /// 是否已审核用户。
        /// </summary>
        bool Checked { get; set; }

        /// <summary>
        /// 是否被锁定。
        /// </summary>
        bool Locked { get; set; }

        /// <summary>
        /// 姓名。
        /// </summary>
        string DisplayName { get; set; }

        /// <summary>
        /// 邮箱。
        /// </summary>
        string Email { get; set; }

        /// <summary>
        /// 手机号。
        /// </summary>
        string Mobile { get; set; }

        /// <summary>
        /// 头像图片路径。
        /// </summary>
        string AvatarUrl { get; set; }

        /// <summary>
        /// 性别。
        /// </summary>
        string Gender { get; set; }

        /// <summary>
        /// 出生日期。
        /// </summary>
        string Birthday { get; set; }

        /// <summary>
        /// 微信。
        /// </summary>
        string WeiXin { get; set; }

        /// <summary>
        /// QQ。
        /// </summary>
        string Qq { get; set; }

        /// <summary>
        /// 微博。
        /// </summary>
        string WeiBo { get; set; }

        /// <summary>
        /// 简介。
        /// </summary>
        string Bio { get; set; }
    }
}
