using SiteServer.CMS.Plugin.Impl;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Model.Attributes
{
    public class SystemConfigInfo : AttributesImpl
    {
        public SystemConfigInfo(string settings) : base(settings)
        {
            
        }

        public bool IsSeparatedApi
        {
            get => GetBool("IsSeparatedApi");
            set => Set("IsSeparatedApi", value);
        }

        public string SeparatedApiUrl
        {
            get => GetString("SeparatedApiUrl");
            set => Set("SeparatedApiUrl", value);
        }

        public string ApiUrl => IsSeparatedApi ? SeparatedApiUrl : PageUtils.ParseNavigationUrl($"~/{WebConfigUtils.ApiPrefix}");

        public bool IsLogSite
        {
            get => GetBool("IsLogSite", true);
            set => Set("IsLogSite", value);
        }

        public bool IsLogAdmin
        {
            get => GetBool("IsLogAdmin", true);
            set => Set("IsLogAdmin", value);
        }

        public bool IsLogUser
        {
            get => GetBool("IsLogUser", true);
            set => Set("IsLogUser", value);
        }

        public bool IsLogError
        {
            get => GetBool("IsLogError", true);
            set => Set("IsLogError", value);
        }

        /// <summary>
        /// 是否只查看自己添加的内容
        /// 如果是，那么管理员只能查看自己添加的内容
        /// 如果不是，那么管理员可以查看其他管理员的内容，默认false
        /// 注意：超级管理与，站点管理员，审核管理员，此设置无效
        /// add by sessionliang at 20151217
        /// </summary>
        public bool IsViewContentOnlySelf
        {
            get => GetBool("IsViewContentOnlySelf");
            set => Set("IsViewContentOnlySelf", value);
        }

        // 是否开启时间阈值
        public bool IsTimeThreshold
        {
            get => GetBool("IsTimeThreshold");
            set => Set("IsTimeThreshold", value);
        }

        public int TimeThreshold
        {
            get => GetInt("TimeThreshold", 60);
            set => Set("TimeThreshold", value);
        }

        /****************管理员设置********************/

        public int AdminUserNameMinLength
        {
            get => GetInt("AdminUserNameMinLength");
            set => Set("AdminUserNameMinLength", value);
        }

        public int AdminPasswordMinLength
        {
            get => GetInt("AdminPasswordMinLength", 6);
            set => Set("AdminPasswordMinLength", value);
        }

        public string AdminPasswordRestriction
        {
            get => GetString("AdminPasswordRestriction", EUserPasswordRestrictionUtils.GetValue(EUserPasswordRestriction.LetterAndDigit));
            set => Set("AdminPasswordRestriction", value);
        }

        public bool IsAdminLockLogin
        {
            get => GetBool("IsAdminLockLogin");
            set => Set("IsAdminLockLogin", value);
        }

        public int AdminLockLoginCount
        {
            get => GetInt("AdminLockLoginCount", 3);
            set => Set("AdminLockLoginCount", value);
        }

        public string AdminLockLoginType
        {
            get => GetString("AdminLockLoginType", EUserLockTypeUtils.GetValue(EUserLockType.Hours));
            set => Set("AdminLockLoginType", value);
        }

        public int AdminLockLoginHours
        {
            get => GetInt("AdminLockLoginHours", 3);
            set => Set("AdminLockLoginHours", value);
        }

        /****************用户设置********************/

        public bool IsUserRegistrationAllowed
        {
            get => GetBool("IsUserRegistrationAllowed", true);
            set => Set("IsUserRegistrationAllowed", value);
        }

        public string UserRegistrationAttributes
        {
            get => GetString("UserRegistrationAttributes");
            set => Set("UserRegistrationAttributes", value);
        }

        public bool IsUserRegistrationGroup
        {
            get => GetBool("IsUserRegistrationGroup");
            set => Set("IsUserRegistrationGroup", value);
        }

        public bool IsUserRegistrationChecked
        {
            get => GetBool("IsUserRegistrationChecked", true);
            set => Set("IsUserRegistrationChecked", value);
        }

        public bool IsUserUnRegistrationAllowed
        {
            get => GetBool("IsUserUnRegistrationAllowed", true);
            set => Set("IsUserUnRegistrationAllowed", value);
        }

        public int UserPasswordMinLength
        {
            get => GetInt("UserPasswordMinLength", 6);
            set => Set("UserPasswordMinLength", value);
        }

        public string UserPasswordRestriction
        {
            get => GetString("UserPasswordRestriction", EUserPasswordRestrictionUtils.GetValue(EUserPasswordRestriction.LetterAndDigit));
            set => Set("UserPasswordRestriction", value);
        }

        public int UserRegistrationMinMinutes
        {
            get => GetInt("UserRegistrationMinMinutes");
            set => Set("UserRegistrationMinMinutes", value);
        }

        public bool IsUserLockLogin
        {
            get => GetBool("IsUserLockLogin");
            set => Set("IsUserLockLogin", value);
        }

        public int UserLockLoginCount
        {
            get => GetInt("UserLockLoginCount", 3);
            set => Set("UserLockLoginCount", value);
        }

        public string UserLockLoginType
        {
            get => GetString("UserLockLoginType", "Hours");
            set => Set("UserLockLoginType", value);
        }

        public int UserLockLoginHours
        {
            get => GetInt("UserLockLoginHours", 3);
            set => Set("UserLockLoginHours", value);
        }

        public string UserDefaultGroupAdminName
        {
            get => GetString("UserDefaultGroupAdminName");
            set => Set("UserDefaultGroupAdminName", value);
        }

        /****************用户中心设置********************/

        public bool IsHomeClosed
        {
            get => GetBool("IsHomeClosed");
            set => Set("IsHomeClosed", value);
        }

        public string HomeTitle
        {
            get => GetString("HomeTitle", "用户中心");
            set => Set("HomeTitle", value);
        }

        public bool IsHomeLogo
        {
            get => GetBool("IsHomeLogo");
            set => Set("IsHomeLogo", value);
        }

        public string HomeLogoUrl
        {
            get => GetString("HomeLogoUrl");
            set => Set("HomeLogoUrl", value);
        }

        public string HomeDefaultAvatarUrl
        {
            get => GetString("HomeDefaultAvatarUrl");
            set => Set("HomeDefaultAvatarUrl", value);
        }

        public bool IsHomeAgreement
        {
            get => GetBool("IsHomeAgreement");
            set => Set("IsHomeAgreement", value);
        }

        public string HomeAgreementHtml
        {
            get => GetString("HomeAgreementHtml", @"阅读并接受<a href=""/agreement.html"" target=""_blank"">《用户协议》</a>");
            set => Set("HomeAgreementHtml", value);
        }
    }
}
