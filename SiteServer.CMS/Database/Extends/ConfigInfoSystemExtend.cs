using SiteServer.CMS.Database.Models;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Database.Extends
{
    public class ConfigInfoSystemExtend : AttributesImpl
    {
        private readonly ConfigInfo _configInfo;

        public ConfigInfoSystemExtend(ConfigInfo configInfo) : base(configInfo.GetSystemConfig())
        {
            _configInfo = configInfo;
        }

        public override void Set(string name, object value)
        {
            base.Set(name, value);
            _configInfo?.SetSystemConfig(ToString());
        }

        public bool IsSeparatedApi
        {
            get => GetBool(nameof(IsSeparatedApi));
            set => Set(nameof(IsSeparatedApi), value);
        }

        public string SeparatedApiUrl
        {
            get => GetString(nameof(SeparatedApiUrl));
            set => Set(nameof(SeparatedApiUrl), value);
        }

        public string ApiUrl => IsSeparatedApi ? SeparatedApiUrl : PageUtils.ParseNavigationUrl($"~/{WebConfigUtils.ApiPrefix}");

        public bool IsLogSite
        {
            get => GetBool(nameof(IsLogSite), true);
            set => Set(nameof(IsLogSite), value);
        }

        public bool IsLogAdmin
        {
            get => GetBool(nameof(IsLogAdmin), true);
            set => Set(nameof(IsLogAdmin), value);
        }

        public bool IsLogUser
        {
            get => GetBool(nameof(IsLogUser), true);
            set => Set(nameof(IsLogUser), value);
        }

        public bool IsLogError
        {
            get => GetBool(nameof(IsLogError), true);
            set => Set(nameof(IsLogError), value);
        }

        /// <summary>
        /// 是否只查看自己添加的内容
        /// 如果是，那么管理员只能查看自己添加的内容
        /// 如果不是，那么管理员可以查看其他管理员的内容，默认false
        /// 注意：超级管理与，站点管理员，审核管理员，此设置无效
        /// </summary>
        public bool IsViewContentOnlySelf
        {
            get => GetBool(nameof(IsViewContentOnlySelf));
            set => Set(nameof(IsViewContentOnlySelf), value);
        }

        // 是否开启时间阈值
        public bool IsTimeThreshold
        {
            get => GetBool(nameof(IsTimeThreshold));
            set => Set(nameof(IsTimeThreshold), value);
        }

        public int TimeThreshold
        {
            get => GetInt(nameof(TimeThreshold), 60);
            set => Set(nameof(TimeThreshold), value);
        }

        /****************管理员设置********************/

        public int AdminUserNameMinLength
        {
            get => GetInt(nameof(AdminUserNameMinLength));
            set => Set(nameof(AdminUserNameMinLength), value);
        }

        public int AdminPasswordMinLength
        {
            get => GetInt(nameof(AdminPasswordMinLength), 6);
            set => Set(nameof(AdminPasswordMinLength), value);
        }

        public string AdminPasswordRestriction
        {
            get => GetString(nameof(AdminPasswordRestriction), EUserPasswordRestrictionUtils.GetValue(EUserPasswordRestriction.LetterAndDigit));
            set => Set(nameof(AdminPasswordRestriction), value);
        }

        public bool IsAdminLockLogin
        {
            get => GetBool(nameof(IsAdminLockLogin));
            set => Set(nameof(IsAdminLockLogin), value);
        }

        public int AdminLockLoginCount
        {
            get => GetInt(nameof(AdminLockLoginCount), 3);
            set => Set(nameof(AdminLockLoginCount), value);
        }

        public string AdminLockLoginType
        {
            get => GetString(nameof(AdminLockLoginType), EUserLockTypeUtils.GetValue(EUserLockType.Hours));
            set => Set(nameof(AdminLockLoginType), value);
        }

        public int AdminLockLoginHours
        {
            get => GetInt(nameof(AdminLockLoginHours), 3);
            set => Set(nameof(AdminLockLoginHours), value);
        }

        /****************用户设置********************/

        public bool IsUserRegistrationAllowed
        {
            get => GetBool(nameof(IsUserRegistrationAllowed), true);
            set => Set(nameof(IsUserRegistrationAllowed), value);
        }

        public string UserRegistrationAttributes
        {
            get => GetString(nameof(UserRegistrationAttributes));
            set => Set(nameof(UserRegistrationAttributes), value);
        }

        public bool IsUserRegistrationGroup
        {
            get => GetBool(nameof(IsUserRegistrationGroup));
            set => Set(nameof(IsUserRegistrationGroup), value);
        }

        public bool IsUserRegistrationChecked
        {
            get => GetBool(nameof(IsUserRegistrationChecked), true);
            set => Set(nameof(IsUserRegistrationChecked), value);
        }

        public bool IsUserUnRegistrationAllowed
        {
            get => GetBool(nameof(IsUserUnRegistrationAllowed), true);
            set => Set(nameof(IsUserUnRegistrationAllowed), value);
        }

        public int UserPasswordMinLength
        {
            get => GetInt(nameof(UserPasswordMinLength), 6);
            set => Set(nameof(UserPasswordMinLength), value);
        }

        public string UserPasswordRestriction
        {
            get => GetString(nameof(UserPasswordRestriction), EUserPasswordRestrictionUtils.GetValue(EUserPasswordRestriction.LetterAndDigit));
            set => Set(nameof(UserPasswordRestriction), value);
        }

        public int UserRegistrationMinMinutes
        {
            get => GetInt(nameof(UserRegistrationMinMinutes));
            set => Set(nameof(UserRegistrationMinMinutes), value);
        }

        public bool IsUserLockLogin
        {
            get => GetBool(nameof(IsUserLockLogin));
            set => Set(nameof(IsUserLockLogin), value);
        }

        public int UserLockLoginCount
        {
            get => GetInt(nameof(UserLockLoginCount), 3);
            set => Set(nameof(UserLockLoginCount), value);
        }

        public string UserLockLoginType
        {
            get => GetString(nameof(UserLockLoginType), "Hours");
            set => Set(nameof(UserLockLoginType), value);
        }

        public int UserLockLoginHours
        {
            get => GetInt(nameof(UserLockLoginHours), 3);
            set => Set(nameof(UserLockLoginHours), value);
        }

        public string UserDefaultGroupAdminName
        {
            get => GetString(nameof(UserDefaultGroupAdminName));
            set => Set(nameof(UserDefaultGroupAdminName), value);
        }

        /****************用户中心设置********************/

        public bool IsHomeClosed
        {
            get => GetBool(nameof(IsHomeClosed));
            set => Set(nameof(IsHomeClosed), value);
        }

        public string HomeTitle
        {
            get => GetString(nameof(HomeTitle), "用户中心");
            set => Set(nameof(HomeTitle), value);
        }

        public bool IsHomeLogo
        {
            get => GetBool(nameof(IsHomeLogo));
            set => Set(nameof(IsHomeLogo), value);
        }

        public string HomeLogoUrl
        {
            get => GetString(nameof(HomeLogoUrl));
            set => Set(nameof(HomeLogoUrl), value);
        }

        public bool IsHomeBackground
        {
            get => GetBool(nameof(IsHomeBackground));
            set => Set(nameof(IsHomeBackground), value);
        }

        public string HomeBackgroundUrl
        {
            get => GetString(nameof(HomeBackgroundUrl));
            set => Set(nameof(HomeBackgroundUrl), value);
        }

        public string HomeDefaultAvatarUrl
        {
            get => GetString(nameof(HomeDefaultAvatarUrl));
            set => Set(nameof(HomeDefaultAvatarUrl), value);
        }

        public bool IsHomeAgreement
        {
            get => GetBool(nameof(IsHomeAgreement));
            set => Set(nameof(IsHomeAgreement), value);
        }

        public string HomeAgreementHtml
        {
            get => GetString(nameof(HomeAgreementHtml), @"阅读并接受<a href=""/agreement.html"" target=""_blank"">《用户协议》</a>");
            set => Set(nameof(HomeAgreementHtml), value);
        }

        /****************云服务设置********************/

        public string RepositoryOwner
        {
            get => GetString(nameof(RepositoryOwner));
            set => Set(nameof(RepositoryOwner), value);
        }

        public string RepositoryName
        {
            get => GetString(nameof(RepositoryName));
            set => Set(nameof(RepositoryName), value);
        }

        public string RepositoryToken
        {
            get => GetString(nameof(RepositoryToken));
            set => Set(nameof(RepositoryToken), value);
        }

        //public bool IsCloudActivated
        //{
        //    get => GetBool(nameof(IsCloudActivated));
        //    set => Set(nameof(IsCloudActivated), value);
        //}

        //public string CloudName
        //{
        //    get => GetString(nameof(CloudName));
        //    set => Set(nameof(CloudName), value);
        //}

        //public string CloudUserName
        //{
        //    get => GetString(nameof(CloudUserName));
        //    set => Set(nameof(CloudUserName), value);
        //}

        //public string CloudAccessToken
        //{
        //    get => GetString(nameof(CloudAccessToken));
        //    set => Set(nameof(CloudAccessToken), value);
        //}

        //public DateTime CloudExpiresAt
        //{
        //    get => GetDateTime(nameof(CloudExpiresAt), DateTime.Now);
        //    set => Set(nameof(CloudExpiresAt), value);
        //}
    }
}
