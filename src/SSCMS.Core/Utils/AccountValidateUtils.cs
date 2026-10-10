using SSCMS.Utils;

namespace SSCMS.Core.Utils
{
    /// <summary>
    /// 管理员/用户账号字段的长度与格式校验，避免超长或非法输入进入数据库导致异常及信息泄露。
    /// 长度上限按数据库字段长度的保守值设置（兼容历史库中较短的字段定义）。
    /// </summary>
    public static class AccountValidateUtils
    {
        public const int UserNameMaxLength = 50;
        public const int DisplayNameMaxLength = 50;
        public const int EmailMaxLength = 100;
        public const int MobileMaxLength = 20;
        public const int AvatarUrlMaxLength = 400;

        public static (bool IsValid, string ErrorMessage) Validate(string userName, string displayName, string email, string mobile, string avatarUrl)
        {
            if (!string.IsNullOrEmpty(userName) && userName.Length > UserNameMaxLength)
            {
                return (false, $"用户名长度不能超过{UserNameMaxLength}个字符");
            }
            if (!string.IsNullOrEmpty(displayName) && displayName.Length > DisplayNameMaxLength)
            {
                return (false, $"姓名长度不能超过{DisplayNameMaxLength}个字符");
            }
            if (!string.IsNullOrEmpty(email))
            {
                if (email.Length > EmailMaxLength) return (false, $"电子邮箱长度不能超过{EmailMaxLength}个字符");
                if (!StringUtils.IsEmail(email)) return (false, "电子邮箱格式不正确");
            }
            if (!string.IsNullOrEmpty(mobile))
            {
                if (mobile.Length > MobileMaxLength) return (false, $"手机号码长度不能超过{MobileMaxLength}个字符");
                if (!StringUtils.IsMobile(mobile)) return (false, "手机号码格式不正确");
            }
            if (!string.IsNullOrEmpty(avatarUrl) && avatarUrl.Length > AvatarUrlMaxLength)
            {
                return (false, "头像地址过长");
            }

            return (true, string.Empty);
        }
    }
}
