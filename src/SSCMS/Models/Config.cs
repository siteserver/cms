using System;
using System.Collections.Generic;
using Datory;
using Datory.Annotations;
using SSCMS.Configuration;
using SSCMS.Enums;

namespace SSCMS.Models
{
    [DataTable("siteserver_Config")]
    public class Config : Entity
    {
        [DataColumn]
        public string DatabaseVersion { get; set; }

        [DataColumn]
        public DateTime UpdateDate { get; set; }
        public bool Initialized => Id > 0;
        public bool IsLogSite { get; set; } = true;
        public bool IsLogSiteCreate { get; set; } = false;
        public bool IsLogAdmin { get; set; } = true;
        public bool IsLogUser { get; set; } = true;
        public bool IsLogError { get; set; } = true;
        public bool IsTimeThreshold { get; set; }
        public int TimeThreshold { get; set; } = 60;
        public int AdminUserNameMinLength { get; set; }
        public int AdminPasswordMinLength { get; set; } = 6;
        public PasswordRestriction AdminPasswordRestriction { get; set; } = PasswordRestriction.LetterAndDigit;
        public bool IsAdminLockLogin { get; set; }
        public int AdminLockLoginCount { get; set; } = 3;
        public LockType AdminLockLoginType { get; set; } = LockType.Hours;
        public int AdminLockLoginHours { get; set; } = 3;
        public bool IsAdminEnforcePasswordChange { get; set; }
        public int AdminEnforcePasswordChangeDays { get; set; } = 90;
        public bool IsAdminEnforceLogout { get; set; }
        public int AdminEnforceLogoutMinutes { get; set; } = 960;
        public bool IsAdminCaptchaDisabled { get; set; }
        public bool IsUserRegistrationAllowed { get; set; } = true;
        public List<string> UserRegistrationAttributes { get; set; }
        public bool IsUserRegistrationMobile { get; set; }
        public bool IsUserRegistrationEmail { get; set; }
        public bool IsUserRegistrationGroup { get; set; }
        public bool IsUserRegistrationChecked { get; set; } = true;
        public bool IsUserUnRegistrationAllowed { get; set; } = true;
        public bool IsUserForceVerifyMobile { get; set; }
        public int UserPasswordMinLength { get; set; } = 6;
        public PasswordRestriction UserPasswordRestriction { get; set; } = PasswordRestriction.LetterAndDigit;
        public int UserRegistrationMinMinutes { get; set; } = 3;
        public bool IsUserLockLogin { get; set; }
        public int UserLockLoginCount { get; set; } = 3;
        public string UserLockLoginType { get; set; } = "Hours";
        public int UserLockLoginHours { get; set; } = 3;
        public string UserDefaultGroupAdminName { get; set; }
        public bool IsCloudAdmin { get; set; }
        public string AdminTitle { get; set; } = Constants.AdminTitle;
        public string AdminFaviconUrl { get; set; }
        public string AdminLogoUrl { get; set; }
        public string AdminLogoLinkUrl { get; set; } = "https://sscms.com";
        public string AdminWelcomeHtml { get; set; } = @"欢迎使用 SSCMS 管理后台";
        public bool IsHomeClosed { get; set; }
        public string HomeTitle { get; set; } = "用户中心";
        public bool IsHomeLogo { get; set; }
        public string HomeLogoUrl { get; set; }
        public string HomeDefaultAvatarUrl { get; set; }
        public bool IsHomeAgreement { get; set; }
        public string HomeAgreementHtml { get; set; } = @"阅读并接受<a href=""#"">《用户协议》</a>";
        public string HomeWelcomeHtml { get; set; } = @"欢迎使用用户中心";
        public int CloudUserId { get; set; }
        public string CloudUserName { get; set; }
        public string CloudMobile { get; set; }
        public string CloudToken { get; set; }
        public CloudType CloudType { get; set; } = CloudType.Free;
        public DateTime CloudExpirationDate { get; set; }
        public bool IsCloudCensorText { get; set; }
        public bool IsCloudCensorTextAuto { get; set; } = true;
        public bool IsCloudCensorTextIgnore { get; set; } = true;
        public bool IsCloudCensorTextWhiteList { get; set; } = true;
        public bool IsCloudSpellingCheck { get; set; }
        public bool IsCloudSpellingCheckAuto { get; set; } = true;
        public bool IsCloudSpellingCheckIgnore { get; set; } = true;
        public bool IsCloudSpellingCheckWhiteList { get; set; } = true;
        public bool IsCloudBackup { get; set; }
        public bool IsCloudCdn { get; set; }
        public bool IsCloudCdnImages { get; set; }
        public bool IsCloudCdnFiles { get; set; }
        public bool IsCloudVod { get; set; }
        public bool IsCloudImages { get; set; }
        public bool IsCloudSms { get; set; }
        public bool IsCloudSmsAdmin { get; set; } = true;
        public bool IsCloudSmsAdminAndDisableAccount { get; set; }
        public bool IsCloudSmsUser { get; set; }
        public bool IsCloudMail { get; set; }
        public string CloudMailFromAlias { get; set; }
        public bool IsCloudMailContentAdd { get; set; }
        public bool IsCloudMailContentEdit { get; set; }
        public string CloudMailAddress { get; set; }
    }
}
