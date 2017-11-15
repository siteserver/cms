using BaiRong.Core.Model.Enumerations;
using SiteServer.Plugin.Models;

namespace BaiRong.Core.Model
{
    public class SystemConfigInfo : ExtendedAttributes, ISystemConfigInfo
    {
        public SystemConfigInfo(string systemConfig)
        {
            var nameValueCollection = TranslateUtils.ToNameValueCollection(systemConfig);
            SetExtendedAttribute(nameValueCollection);
        }

        public override string ToString()
        {
            return TranslateUtils.NameValueCollectionToString(GetExtendedAttributes());
        }

        public bool IsUrlGlobalSetting
        {
            get { return GetBool("IsUrlGlobalSetting", true); }
            set { SetExtendedAttribute("IsUrlGlobalSetting", value.ToString()); }
        }

        public bool IsSeparatedWeb
        {
            get { return GetBool("IsSeparatedWeb"); }
            set { SetExtendedAttribute("IsSeparatedWeb", value.ToString()); }
        }

        public string WebUrl => IsSeparatedWeb ? SeparatedWebUrl : "/";

        public string SeparatedWebUrl
        {
            get { return GetString("SeparatedWebUrl"); }
            set { SetExtendedAttribute("SeparatedWebUrl", value); }
        }

        public bool IsSeparatedApi
        {
            get { return GetBool("IsSeparatedApi"); }
            set { SetExtendedAttribute("IsSeparatedApi", value.ToString()); }
        }

        public string ApiUrl => IsSeparatedApi ? SeparatedApiUrl : "/api";

        public string SeparatedApiUrl
        {
            get { return GetString("SeparatedApiUrl"); }
            set { SetExtendedAttribute("SeparatedApiUrl", value); }
        }

        public bool IsLogAdmin
        {
            get { return GetBool("IsLogAdmin", true); }
            set { SetExtendedAttribute("IsLogAdmin", value.ToString()); }
        }

        public bool IsLogUser
        {
            get { return GetBool("IsLogUser", true); }
            set { SetExtendedAttribute("IsLogUser", value.ToString()); }
        }

        public bool IsLogTask
        {
            get { return GetBool("IsLogTask", true); }
            set { SetExtendedAttribute("IsLogTask", value.ToString()); }
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
            set { SetExtendedAttribute("IsViewContentOnlySelf", value.ToString()); }
        }

        // 是否开启时间阈值
        public bool IsTimeThreshold
        {
            get { return GetBool("IsTimeThreshold"); }
            set { SetExtendedAttribute("IsTimeThreshold", value.ToString()); }
        }

        public int TimeThreshold
        {
            get { return GetInt("TimeThreshold", 60); }
            set { SetExtendedAttribute("TimeThreshold", value.ToString()); }
        }

        public ESmsProviderType SmsProviderType
        {
            get { return ESmsProviderTypeUtils.GetEnumType(GetString("SmsProviderType", ESmsProviderTypeUtils.GetValue(ESmsProviderType.None))); }
            set { SetExtendedAttribute("SmsProviderType", ESmsProviderTypeUtils.GetValue(value)); }
        }

        public string SmsAppKey
        {
            get { return GetString("SmsAppKey", string.Empty); }
            set { SetExtendedAttribute("SmsAppKey", value); }
        }

        public string IntegrationPayConfigJson
        {
            get { return GetString("IntegrationPayConfigJson", string.Empty); }
            set { SetExtendedAttribute("IntegrationPayConfigJson", value); }
        }

        /****************管理员设置********************/

        public int AdminUserNameMinLength
        {
            get { return GetInt("AdminUserNameMinLength"); }
            set { SetExtendedAttribute("AdminUserNameMinLength", value.ToString()); }
        }

        public int AdminPasswordMinLength
        {
            get { return GetInt("AdminPasswordMinLength", 6); }
            set { SetExtendedAttribute("AdminPasswordMinLength", value.ToString()); }
        }

        public string AdminPasswordRestriction
        {
            get { return GetString("AdminPasswordRestriction", EUserPasswordRestrictionUtils.GetValue(EUserPasswordRestriction.LetterAndDigit)); }
            set { SetExtendedAttribute("AdminPasswordRestriction", value); }
        }

        public bool IsAdminLockLogin
        {
            get { return GetBool("IsAdminLockLogin"); }
            set { SetExtendedAttribute("IsAdminLockLogin", value.ToString()); }
        }

        public int AdminLockLoginCount
        {
            get { return GetInt("AdminLockLoginCount", 3); }
            set { SetExtendedAttribute("AdminLockLoginCount", value.ToString()); }
        }

        public string AdminLockLoginType
        {
            get { return GetString("AdminLockLoginType", EUserLockTypeUtils.GetValue(EUserLockType.Hours)); }
            set { SetExtendedAttribute("AdminLockLoginType", value); }
        }

        public int AdminLockLoginHours
        {
            get { return GetInt("AdminLockLoginHours", 3); }
            set { SetExtendedAttribute("AdminLockLoginHours", value.ToString()); }
        }

        public bool IsAdminFindPassword
        {
            get { return GetBool("IsAdminFindPassword"); }
            set { SetExtendedAttribute("IsAdminFindPassword", value.ToString()); }
        }

        public string AdminFindPasswordSmsTplId
        {
            get { return GetString("AdminFindPasswordSmsTplId", string.Empty); }
            set { SetExtendedAttribute("AdminFindPasswordSmsTplId", value); }
        }

        /****************用户设置********************/

        public bool IsUserRegistrationAllowed
        {
            get { return GetBool("IsUserRegistrationAllowed", true); }
            set { SetExtendedAttribute("IsUserRegistrationAllowed", value.ToString()); }
        }

        public int UserPasswordMinLength
        {
            get { return GetInt("UserPasswordMinLength", 6); }
            set { SetExtendedAttribute("UserPasswordMinLength", value.ToString()); }
        }

        public string UserPasswordRestriction
        {
            get { return GetString("UserPasswordRestriction", EUserPasswordRestrictionUtils.GetValue(EUserPasswordRestriction.LetterAndDigit)); }
            set { SetExtendedAttribute("UserPasswordRestriction", value); }
        }

        public string UserRegistrationVerifyType
        {
            get { return GetString("UserRegistrationVerifyType", EUserVerifyTypeUtils.GetValue(EUserVerifyType.None)); }
            set { SetExtendedAttribute("UserRegistrationVerifyType", value); }
        }

        public string UserRegistrationSmsTplId
        {
            get { return GetString("UserRegistrationSmsTplId", string.Empty); }
            set { SetExtendedAttribute("UserRegistrationSmsTplId", value); }
        }

        public int UserRegistrationMinMinutes
        {
            get { return GetInt("UserRegistrationMinMinutes"); }
            set { SetExtendedAttribute("UserRegistrationMinMinutes", value.ToString()); }
        }

        public bool IsUserFindPassword
        {
            get { return GetBool("IsUserFindPassword"); }
            set { SetExtendedAttribute("IsUserFindPassword", value.ToString()); }
        }

        public string UserFindPasswordSmsTplId
        {
            get { return GetString("UserFindPasswordSmsTplId", string.Empty); }
            set { SetExtendedAttribute("UserFindPasswordSmsTplId", value); }
        }

        public bool IsUserLockLogin
        {
            get { return GetBool("IsUserLockLogin"); }
            set { SetExtendedAttribute("IsUserLockLogin", value.ToString()); }
        }

        public int UserLockLoginCount
        {
            get { return GetInt("UserLockLoginCount", 3); }
            set { SetExtendedAttribute("UserLockLoginCount", value.ToString()); }
        }

        public string UserLockLoginType
        {
            get { return GetString("UserLockLoginType", "Hours"); }
            set { SetExtendedAttribute("UserLockLoginType", value); }
        }

        public int UserLockLoginHours
        {
            get { return GetInt("UserLockLoginHours", 3); }
            set { SetExtendedAttribute("UserLockLoginHours", value.ToString()); }
        }
    }
}
