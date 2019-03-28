using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Database.Models
{
    public partial class ConfigInfo
    {
        public bool IsSeparatedApi { get; set; }

        public string SeparatedApiUrl { get; set; }

        public string ApiUrl =>
            IsSeparatedApi ? SeparatedApiUrl : PageUtils.ParseNavigationUrl($"~/{WebConfigUtils.ApiPrefix}");

        public bool IsLogSite { get; set; } = true;

        public bool IsLogAdmin { get; set; } = true;

        public bool IsLogUser { get; set; } = true;

        public bool IsLogError { get; set; } = true;

        /// <summary>
        /// 是否只查看自己添加的内容
        /// 如果是，那么管理员只能查看自己添加的内容
        /// 如果不是，那么管理员可以查看其他管理员的内容，默认false
        /// 注意：超级管理与，站点管理员，审核管理员，此设置无效
        /// </summary>
        public bool IsViewContentOnlySelf { get; set; }

        // 是否开启时间阈值
        public bool IsTimeThreshold { get; set; }

        public int TimeThreshold { get; set; } = 60;

        /****************管理员设置********************/

        public int AdminUserNameMinLength { get; set; }

        public int AdminPasswordMinLength { get; set; } = 6;

        public string AdminPasswordRestriction { get; set; } =
            EUserPasswordRestrictionUtils.GetValue(EUserPasswordRestriction.LetterAndDigit);

        public bool IsAdminLockLogin { get; set; }

        public int AdminLockLoginCount { get; set; } = 3;

        public string AdminLockLoginType { get; set; } = EUserLockTypeUtils.GetValue(EUserLockType.Hours);

        public int AdminLockLoginHours { get; set; } = 3;

        /****************用户设置********************/

        public bool IsUserRegistrationAllowed { get; set; } = true;

        public string UserRegistrationAttributes { get; set; }

        public bool IsUserRegistrationGroup { get; set; }

        public bool IsUserRegistrationChecked { get; set; } = true;

        public bool IsUserUnRegistrationAllowed { get; set; } = true;

        public int UserPasswordMinLength { get; set; } = 6;

        public string UserPasswordRestriction { get; set; } =
            EUserPasswordRestrictionUtils.GetValue(EUserPasswordRestriction.LetterAndDigit);

        public int UserRegistrationMinMinutes { get; set; }

        public bool IsUserLockLogin { get; set; }

        public int UserLockLoginCount { get; set; } = 3;

        public string UserLockLoginType { get; set; } = "Hours";

        public int UserLockLoginHours { get; set; } = 3;

        public string UserDefaultGroupAdminName { get; set; }

        /****************用户中心设置********************/

        public bool IsHomeClosed { get; set; }

        public string HomeTitle { get; set; } = "用户中心";

        public bool IsHomeLogo { get; set; }

        public string HomeLogoUrl { get; set; }

        public bool IsHomeBackground { get; set; }

        public string HomeBackgroundUrl { get; set; }

        public string HomeDefaultAvatarUrl { get; set; }

        public bool IsHomeAgreement { get; set; }

        public string HomeAgreementHtml { get; set; } =
            @"阅读并接受<a href=""/agreement.html"" target=""_blank"">《用户协议》</a>";

        /****************云服务设置********************/

        public string RepositoryOwner { get; set; }

        public string RepositoryName { get; set; }

        public string RepositoryToken { get; set; }
    }
}
