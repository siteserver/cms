using System;
using Dapper.Contrib.Extensions;
using SiteServer.CMS.Provider;
using SiteServer.Plugin;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Model
{
    public class AdministratorInfo : IAdministratorInfo
    {
        private string _displayName;

        public AdministratorInfo()
        {
            Id = 0;
            UserName = string.Empty;
            Password = string.Empty;
            PasswordFormat = EPasswordFormatUtils.GetValue(EPasswordFormat.Encrypted);
            PasswordSalt = string.Empty;
            CreationDate = DateUtils.SqlMinValue;
            LastActivityDate = DateUtils.SqlMinValue;
            CountOfLogin = 0;
            CountOfFailedLogin = 0;
            CreatorUserName = string.Empty;
            IsLockedOut = false;
            SiteIdCollection = string.Empty;
            SiteId = 0;
            DepartmentId = 0;
            AreaId = 0;
            _displayName = string.Empty;
            Mobile = string.Empty;
            Email = string.Empty;
            AvatarUrl = string.Empty;
        }

        public AdministratorInfo(int id, string userName, string password, string passwordFormat,
            string passwordSalt, DateTime creationDate, DateTime lastActivityDate, int countOfLogin,
            int countOfFailedLogin, string creatorUserName, bool isLockedOut, string siteIdCollection, int siteId,
            int departmentId, int areaId, string displayName, string mobile, string email, string avatarUrl)
        {
            Id = id;
            UserName = userName;
            Password = password;
            PasswordFormat = passwordFormat;
            PasswordSalt = passwordSalt;
            CreationDate = creationDate;
            LastActivityDate = lastActivityDate;
            CountOfLogin = countOfLogin;
            CountOfFailedLogin = countOfFailedLogin;
            CreatorUserName = creatorUserName;
            IsLockedOut = isLockedOut;
            SiteIdCollection = siteIdCollection;
            SiteId = siteId;
            DepartmentId = departmentId;
            AreaId = areaId;
            _displayName = displayName;
            Mobile = mobile;
            Email = email;
            AvatarUrl = avatarUrl;
        }

        public int Id { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string PasswordFormat { get; set; }

        public string PasswordSalt { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime LastActivityDate { get; set; }

        public int CountOfLogin { get; set; }

        public int CountOfFailedLogin { get; set; }

        public string CreatorUserName { get; set; }

        public bool IsLockedOut { get; set; }

        public string SiteIdCollection { get; set; }

        public int SiteId { get; set; }

        public int DepartmentId { get; set; }

        public int AreaId { get; set; }

        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(_displayName))
                {
                    _displayName = UserName;
                }

                return _displayName;
            }
            set { _displayName = value; }
        }

        public string Mobile { get; set; }

        public string Email { get; set; }

        public string AvatarUrl { get; set; }
    }

    [Table(AdministratorDao.DatabaseTableName)]
    public class AdministratorInfoDatabase
    {
        public AdministratorInfoDatabase()
        {
            Id = 0;
            UserName = string.Empty;
            Password = string.Empty;
            PasswordFormat = EPasswordFormatUtils.GetValue(EPasswordFormat.Encrypted);
            PasswordSalt = string.Empty;
            CreationDate = DateUtils.SqlMinValue;
            LastActivityDate = DateUtils.SqlMinValue;
            CountOfLogin = 0;
            CountOfFailedLogin = 0;
            CreatorUserName = string.Empty;
            IsLockedOut = false.ToString();
            SiteIdCollection = string.Empty;
            SiteId = 0;
            DepartmentId = 0;
            AreaId = 0;
            DisplayName = string.Empty;
            Mobile = string.Empty;
            Email = string.Empty;
            AvatarUrl = string.Empty;
        }

        public AdministratorInfoDatabase(AdministratorInfo adminInfo)
        {
            Id = adminInfo.Id;
            UserName = adminInfo.UserName;
            Password = adminInfo.Password;
            PasswordFormat = adminInfo.PasswordFormat;
            PasswordSalt = adminInfo.PasswordSalt;
            CreationDate = adminInfo.CreationDate;
            LastActivityDate = adminInfo.LastActivityDate;
            CountOfLogin = adminInfo.CountOfLogin;
            CountOfFailedLogin = adminInfo.CountOfFailedLogin;
            CreatorUserName = adminInfo.CreatorUserName;
            IsLockedOut = adminInfo.IsLockedOut.ToString();
            SiteIdCollection = adminInfo.SiteIdCollection;
            SiteId = adminInfo.SiteId;
            DepartmentId = adminInfo.DepartmentId;
            AreaId = adminInfo.AreaId;
            DisplayName = adminInfo.DisplayName;
            Mobile = adminInfo.Mobile;
            Email = adminInfo.Email;
            AvatarUrl = adminInfo.AvatarUrl;
        }

        public AdministratorInfo ToAdministratorInfo()
        {
            var adminInfo = new AdministratorInfo
            {
                Id = Id,
                UserName = UserName,
                Password = Password,
                PasswordFormat = PasswordFormat,
                PasswordSalt = PasswordSalt,
                CreationDate = CreationDate,
                LastActivityDate = LastActivityDate,
                CountOfLogin = CountOfLogin,
                CountOfFailedLogin = CountOfFailedLogin,
                CreatorUserName = CreatorUserName,
                IsLockedOut = TranslateUtils.ToBool(IsLockedOut),
                SiteIdCollection = SiteIdCollection,
                SiteId = SiteId,
                DepartmentId = DepartmentId,
                AreaId = AreaId,
                DisplayName = DisplayName,
                Mobile = Mobile,
                Email = Email,
                AvatarUrl = AvatarUrl
            };

            return adminInfo;
        }

        public int Id { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string PasswordFormat { get; set; }

        public string PasswordSalt { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime LastActivityDate { get; set; }

        public int CountOfLogin { get; set; }

        public int CountOfFailedLogin { get; set; }

        public string CreatorUserName { get; set; }

        public string IsLockedOut { get; set; }

        public string SiteIdCollection { get; set; }

        public int SiteId { get; set; }

        public int DepartmentId { get; set; }

        public int AreaId { get; set; }

        public string DisplayName { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }

        public string AvatarUrl { get; set; }
    }

    public class AdministratorInfoCreateUpdate
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string PasswordFormat { get; set; }

        public DateTime? CreationDate { get; set; }

        public DateTime? LastActivityDate { get; set; }

        public int? CountOfLogin { get; set; }

        public int? CountOfFailedLogin { get; set; }

        public string CreatorUserName { get; set; }

        public bool? IsLockedOut { get; set; }

        public string SiteIdCollection { get; set; }

        public int? SiteId { get; set; }

        public int? DepartmentId { get; set; }

        public int? AreaId { get; set; }

        public string DisplayName { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }

        public string AvatarUrl { get; set; }

        public void Load(AdministratorInfoDatabase dbInfo)
        {
            if (UserName != null)
            {
                dbInfo.UserName = UserName;
            }

            if (Password != null)
            {
                dbInfo.Password = Password;
            }

            if (PasswordFormat != null)
            {
                dbInfo.PasswordFormat = PasswordFormat;
            }

            if (CreationDate != null)
            {
                dbInfo.CreationDate = (DateTime) CreationDate;
            }

            if (LastActivityDate != null)
            {
                dbInfo.LastActivityDate = (DateTime) LastActivityDate;
            }

            if (CountOfLogin != null)
            {
                dbInfo.CountOfLogin = (int) CountOfLogin;
            }

            if (CountOfFailedLogin != null)
            {
                dbInfo.CountOfFailedLogin = (int) CountOfFailedLogin;
            }

            if (CreatorUserName != null)
            {
                dbInfo.CreatorUserName = CreatorUserName;
            }

            if (IsLockedOut != null)
            {
                dbInfo.IsLockedOut = IsLockedOut.ToString();
            }

            if (SiteIdCollection != null)
            {
                dbInfo.SiteIdCollection = SiteIdCollection;
            }

            if (SiteId != null)
            {
                dbInfo.SiteId = (int) SiteId;
            }

            if (DepartmentId != null)
            {
                dbInfo.DepartmentId = (int) DepartmentId;
            }

            if (AreaId != null)
            {
                dbInfo.AreaId = (int) AreaId;
            }

            if (DisplayName != null)
            {
                dbInfo.DisplayName = DisplayName;
            }


            if (Mobile != null)
            {
                dbInfo.Mobile = Mobile;
            }

            if (Email != null)
            {
                dbInfo.Email = Email;
            }

            if (AvatarUrl != null)
            {
                dbInfo.AvatarUrl = AvatarUrl;
            }
        }
    }
}
