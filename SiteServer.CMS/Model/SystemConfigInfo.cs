using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Model
{
    public class SystemConfigInfo : ExtendedAttributes
    {
        public SystemConfigInfo(string settings) : base(settings)
        {
            
        }

        public bool IsSeparatedApi
        {
            get { return GetBool("IsSeparatedApi"); }
            set { Set("IsSeparatedApi", value.ToString()); }
        }

        public string SeparatedApiUrl
        {
            get { return GetString("SeparatedApiUrl"); }
            set { Set("SeparatedApiUrl", value); }
        }

        public string ApiUrl => IsSeparatedApi ? SeparatedApiUrl : PageUtils.ParseNavigationUrl("~/api");

        public bool IsLogAdmin
        {
            get { return GetBool("IsLogAdmin", true); }
            set { Set("IsLogAdmin", value.ToString()); }
        }

        public bool IsLogUser
        {
            get { return GetBool("IsLogUser", true); }
            set { Set("IsLogUser", value.ToString()); }
        }

        public bool IsLogTask
        {
            get { return GetBool("IsLogTask", true); }
            set { Set("IsLogTask", value.ToString()); }
        }

        /// <summary>
        /// 是否只查看自己添加的内容
        /// 如果是，那么管理员只能查看自己添加的内容
        /// 如果不是，那么管理员可以查看其他管理员天机的内容，默认false
        /// 注意：超级管理与，站点管理员，审核管理员，此设置无效
        /// add by sessionliang at 20151217
        /// </summary>
        public bool IsViewContentOnlySelf
        {
            get { return GetBool("IsViewContentOnlySelf"); }
            set { Set("IsViewContentOnlySelf", value.ToString()); }
        }

        // 是否开启时间阈值
        public bool IsTimeThreshold
        {
            get { return GetBool("IsTimeThreshold"); }
            set { Set("IsTimeThreshold", value.ToString()); }
        }

        public int TimeThreshold
        {
            get { return GetInt("TimeThreshold", 60); }
            set { Set("TimeThreshold", value.ToString()); }
        }

        public ESmsProviderType SmsProviderType
        {
            get { return ESmsProviderTypeUtils.GetEnumType(GetString("SmsProviderType", ESmsProviderTypeUtils.GetValue(ESmsProviderType.None))); }
            set { Set("SmsProviderType", ESmsProviderTypeUtils.GetValue(value)); }
        }

        public string SmsAppKey
        {
            get { return GetString("SmsAppKey", string.Empty); }
            set { Set("SmsAppKey", value); }
        }

        public string IntegrationPayConfigJson
        {
            get { return GetString("IntegrationPayConfigJson", string.Empty); }
            set { Set("IntegrationPayConfigJson", value); }
        }

        /****************管理员设置********************/

        public int AdminUserNameMinLength
        {
            get { return GetInt("AdminUserNameMinLength"); }
            set { Set("AdminUserNameMinLength", value.ToString()); }
        }

        public int AdminPasswordMinLength
        {
            get { return GetInt("AdminPasswordMinLength", 6); }
            set { Set("AdminPasswordMinLength", value.ToString()); }
        }

        public string AdminPasswordRestriction
        {
            get { return GetString("AdminPasswordRestriction", EUserPasswordRestrictionUtils.GetValue(EUserPasswordRestriction.LetterAndDigit)); }
            set { Set("AdminPasswordRestriction", value); }
        }

        public bool IsAdminLockLogin
        {
            get { return GetBool("IsAdminLockLogin"); }
            set { Set("IsAdminLockLogin", value.ToString()); }
        }

        public int AdminLockLoginCount
        {
            get { return GetInt("AdminLockLoginCount", 3); }
            set { Set("AdminLockLoginCount", value.ToString()); }
        }

        public string AdminLockLoginType
        {
            get { return GetString("AdminLockLoginType", EUserLockTypeUtils.GetValue(EUserLockType.Hours)); }
            set { Set("AdminLockLoginType", value); }
        }

        public int AdminLockLoginHours
        {
            get { return GetInt("AdminLockLoginHours", 3); }
            set { Set("AdminLockLoginHours", value.ToString()); }
        }

        public bool IsAdminFindPassword
        {
            get { return GetBool("IsAdminFindPassword"); }
            set { Set("IsAdminFindPassword", value.ToString()); }
        }

        public string AdminFindPasswordSmsTplId
        {
            get { return GetString("AdminFindPasswordSmsTplId", string.Empty); }
            set { Set("AdminFindPasswordSmsTplId", value); }
        }

        /****************用户设置********************/

        public bool IsUserRegistrationAllowed
        {
            get { return GetBool("IsUserRegistrationAllowed", true); }
            set { Set("IsUserRegistrationAllowed", value.ToString()); }
        }

        public int UserPasswordMinLength
        {
            get { return GetInt("UserPasswordMinLength", 6); }
            set { Set("UserPasswordMinLength", value.ToString()); }
        }

        public string UserPasswordRestriction
        {
            get { return GetString("UserPasswordRestriction", EUserPasswordRestrictionUtils.GetValue(EUserPasswordRestriction.LetterAndDigit)); }
            set { Set("UserPasswordRestriction", value); }
        }

        public int UserRegistrationMinMinutes
        {
            get { return GetInt("UserRegistrationMinMinutes"); }
            set { Set("UserRegistrationMinMinutes", value.ToString()); }
        }

        public bool IsUserFindPassword
        {
            get { return GetBool("IsUserFindPassword"); }
            set { Set("IsUserFindPassword", value.ToString()); }
        }

        public string UserFindPasswordSmsTplId
        {
            get { return GetString("UserFindPasswordSmsTplId", string.Empty); }
            set { Set("UserFindPasswordSmsTplId", value); }
        }

        public bool IsUserLockLogin
        {
            get { return GetBool("IsUserLockLogin"); }
            set { Set("IsUserLockLogin", value.ToString()); }
        }

        public int UserLockLoginCount
        {
            get { return GetInt("UserLockLoginCount", 3); }
            set { Set("UserLockLoginCount", value.ToString()); }
        }

        public string UserLockLoginType
        {
            get { return GetString("UserLockLoginType", "Hours"); }
            set { Set("UserLockLoginType", value); }
        }

        public int UserLockLoginHours
        {
            get { return GetInt("UserLockLoginHours", 3); }
            set { Set("UserLockLoginHours", value.ToString()); }
        }
    }
}
