using BaiRong.Core.Model.Enumerations;

namespace BaiRong.Core.Model
{
    public class SystemConfigInfo : ExtendedAttributes
    {
        public SystemConfigInfo(string systemConfig)
        {
            var nameValueCollection = TranslateUtils.ToNameValueCollection(systemConfig);
            SetExtendedAttribute(nameValueCollection);
        }

        public override string ToString()
        {
            return TranslateUtils.NameValueCollectionToString(Attributes);
        }

        public string Cipherkey
        {
            get { return GetString("Cipherkey", string.Empty); }
            set { SetExtendedAttribute("Cipherkey", value); }
        }

        public ERestrictionType RestrictionType
        {
            get { return ERestrictionTypeUtils.GetEnumType(GetString("RestrictionType", string.Empty)); }
            set { SetExtendedAttribute("RestrictionType", ERestrictionTypeUtils.GetValue(value)); }
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
            get { return GetBool("IsViewContentOnlySelf", false); }
            set { SetExtendedAttribute("IsViewContentOnlySelf", value.ToString()); }
        }

        // 是否开启时间阈值
        public bool IsTimeThreshold
        {
            get { return GetBool("IsTimeThreshold", false); }
            set { SetExtendedAttribute("IsTimeThreshold", value.ToString()); }
        }

        public int TimeThreshold
        {
            get { return GetInt("TimeThreshold", 60); }
            set { SetExtendedAttribute("TimeThreshold", value.ToString()); }
        }

        // 是否开启条数阈值
        public bool IsCounterThreshold
        {
            get { return GetBool("IsCounterThreshold", false); }
            set { SetExtendedAttribute("IsCounterThreshold", value.ToString()); }
        }

        public int CounterThreshold
        {
            get { return GetInt("CounterThreshold", 3); }
            set { SetExtendedAttribute("CounterThreshold", value.ToString()); }
        }

        public int LoginUserNameMinLength
        {
            get { return GetInt("LoginUserNameMinLength", 0); }
            set { SetExtendedAttribute("LoginUserNameMinLength", value.ToString()); }
        }

        public int LoginPasswordMinLength
        {
            get { return GetInt("LoginPasswordMinLength", 6); }
            set { SetExtendedAttribute("LoginPasswordMinLength", value.ToString()); }
        }

        public EUserPasswordRestriction LoginPasswordRestriction
        {
            get { return EUserPasswordRestrictionUtils.GetEnumType(GetString("LoginPasswordRestriction", EUserPasswordRestrictionUtils.GetValue(EUserPasswordRestriction.None))); }
            set { SetExtendedAttribute("LoginPasswordRestriction", EUserPasswordRestrictionUtils.GetValue(value)); }
        }

        public bool IsLoginFailToLock
        {
            get { return GetBool("IsLoginFailToLock", false); }
            set { SetExtendedAttribute("IsLoginFailToLock", value.ToString()); }
        }

        public int LoginFailToLockCount
        {
            get { return GetInt("LoginFailToLockCount", 3); }
            set { SetExtendedAttribute("LoginFailToLockCount", value.ToString()); }
        }

        public string LoginLockingType
        {
            get { return GetString("LoginLockingType", "Forever"); }
            set { SetExtendedAttribute("LoginLockingType", value); }
        }

        public int LoginLockingHours
        {
            get { return GetInt("LoginLockingHours", 3); }
            set { SetExtendedAttribute("LoginLockingHours", value.ToString()); }
        }

        public bool IsFindPassword
        {
            get { return GetBool("IsFindPassword", false); }
            set { SetExtendedAttribute("IsFindPassword", value.ToString()); }
        }

        public string FindPasswordSmsTplId
        {
            get { return GetString("FindPasswordSmsTplId", string.Empty); }
            set { SetExtendedAttribute("FindPasswordSmsTplId", value); }
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

        public string SmsAppSecret
        {
            get { return GetString("SmsAppSecret", string.Empty); }
            set { SetExtendedAttribute("SmsAppSecret", value); }
        }

        public string SmsSignName
        {
            get { return GetString("SmsSignName", string.Empty); }
            set { SetExtendedAttribute("SmsSignName", value); }
        }
    }
}
