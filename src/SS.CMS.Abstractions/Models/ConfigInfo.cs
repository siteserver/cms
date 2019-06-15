using System;
using SS.CMS.Data;

namespace SS.CMS.Abstractions.Models
{
    [Table("siteserver_Config")]
    public partial class ConfigInfo : Entity
    {
        [TableColumn]
        private string IsInitialized { get; set; }

        public bool Initialized
        {
            get => IsInitialized == "True";
            set => IsInitialized = value.ToString();
        }

        [TableColumn]
        public string DatabaseVersion { get; set; }

        [TableColumn]
        public DateTime? UpdateDate { get; set; }

        [TableColumn(Text = true, Extend = true)]
        private string SystemConfig { get; set; }

        public bool IsSeparatedApi { get; set; }

        public string SeparatedApiUrl { get; set; }

        public bool IsLogSite { get; set; } = true;

        public bool IsLogAdmin { get; set; } = true;

        public bool IsLogUser { get; set; } = true;

        public bool IsLogError { get; set; } = true;

        /// <summary>
        /// �Ƿ�ֻ�鿴�Լ����ӵ�����
        /// ����ǣ���ô����Աֻ�ܲ鿴�Լ����ӵ�����
        /// ������ǣ���ô����Ա���Բ鿴��������Ա�����ݣ�Ĭ��false
        /// ע�⣺���������룬վ�����Ա����˹���Ա����������Ч
        /// </summary>
        public bool IsViewContentOnlySelf { get; set; }

        // �Ƿ���ʱ����ֵ
        public bool IsTimeThreshold { get; set; }

        public int TimeThreshold { get; set; } = 60;

        /****************����Ա����********************/

        public int AdminUserNameMinLength { get; set; }

        public int AdminPasswordMinLength { get; set; } = 6;

        public string AdminPasswordRestriction { get; set; } = "LetterAndDigit";

        public bool IsAdminLockLogin { get; set; }

        public int AdminLockLoginCount { get; set; } = 3;

        public string AdminLockLoginType { get; set; } = "Hours";

        public int AdminLockLoginHours { get; set; } = 3;

        /****************�û�����********************/

        public bool IsUserRegistrationAllowed { get; set; } = true;

        public string UserRegistrationAttributes { get; set; }

        public bool IsUserRegistrationGroup { get; set; }

        public bool IsUserRegistrationChecked { get; set; } = true;

        public bool IsUserUnRegistrationAllowed { get; set; } = true;

        public int UserPasswordMinLength { get; set; } = 6;

        public string UserPasswordRestriction { get; set; } = "LetterAndDigit";

        public int UserRegistrationMinMinutes { get; set; }

        public bool IsUserLockLogin { get; set; }

        public int UserLockLoginCount { get; set; } = 3;

        public string UserLockLoginType { get; set; } = "Hours";

        public int UserLockLoginHours { get; set; } = 3;

        public string UserDefaultGroupAdminName { get; set; }

        /****************�û���������********************/

        public bool IsHomeClosed { get; set; }

        public string HomeTitle { get; set; } = "�û�����";

        public bool IsHomeLogo { get; set; }

        public string HomeLogoUrl { get; set; }

        public bool IsHomeBackground { get; set; }

        public string HomeBackgroundUrl { get; set; }

        public string HomeDefaultAvatarUrl { get; set; }

        public bool IsHomeAgreement { get; set; }

        public string HomeAgreementHtml { get; set; } =
            @"�Ķ�������<a href=""/agreement.html"" target=""_blank"">���û�Э�顷</a>";

        /****************�Ʒ�������********************/

        public string RepositoryOwner { get; set; }

        public string RepositoryName { get; set; }

        public string RepositoryToken { get; set; }
    }
}
