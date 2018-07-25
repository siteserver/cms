using System;
using Dapper.Contrib.Extensions;
using SiteServer.CMS.Provider;
using SiteServer.Plugin;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Model
{
    public class UserInfo : IUserInfo
    {
        public UserInfo()
        {
            Id = 0;
            UserName = string.Empty;
            Password = string.Empty;
            PasswordFormat = string.Empty;
            PasswordSalt = string.Empty;
            CreateDate = DateUtils.SqlMinValue;
            LastResetPasswordDate = DateUtils.SqlMinValue;
            LastActivityDate = DateUtils.SqlMinValue;
            CountOfLogin = 0;
            CountOfFailedLogin = 0;
            CountOfWriting = 0;
            IsChecked = true;
            IsLockedOut = false;
            DisplayName = string.Empty;
            Email = string.Empty;
            Mobile = string.Empty;
            AvatarUrl = string.Empty;
            Organization = string.Empty;
            Department = string.Empty;
            Position = string.Empty;
            Gender = string.Empty;
            Birthday = string.Empty;
            Education = string.Empty;
            Graduation = string.Empty;
            Address = string.Empty;
            WeiXin = string.Empty;
            Qq = string.Empty;
            WeiBo = string.Empty;
            Interests = string.Empty;
            Signature = string.Empty;
        }

        public UserInfo(object dataItem)
        {
            if (dataItem == null) return;
            Id = SqlUtils.EvalInt(dataItem, nameof(Id));
            UserName = SqlUtils.EvalString(dataItem, nameof(UserName));
            Password = SqlUtils.EvalString(dataItem, nameof(Password));
            PasswordFormat = SqlUtils.EvalString(dataItem, nameof(PasswordFormat));
            PasswordSalt = SqlUtils.EvalString(dataItem, nameof(PasswordSalt));
            CreateDate = SqlUtils.EvalDateTime(dataItem, nameof(CreateDate));
            LastResetPasswordDate = SqlUtils.EvalDateTime(dataItem, nameof(LastResetPasswordDate));
            LastActivityDate = SqlUtils.EvalDateTime(dataItem, nameof(LastActivityDate));
            CountOfLogin = SqlUtils.EvalInt(dataItem, nameof(CountOfLogin));
            CountOfFailedLogin = SqlUtils.EvalInt(dataItem, nameof(CountOfFailedLogin));
            CountOfWriting = SqlUtils.EvalInt(dataItem, nameof(CountOfWriting));
            IsChecked = SqlUtils.EvalBool(dataItem, nameof(IsChecked));
            IsLockedOut = SqlUtils.EvalBool(dataItem, nameof(IsLockedOut));
            DisplayName = SqlUtils.EvalString(dataItem, nameof(DisplayName));
            Email = SqlUtils.EvalString(dataItem, nameof(Email));
            Mobile = SqlUtils.EvalString(dataItem, nameof(Mobile));
            AvatarUrl = SqlUtils.EvalString(dataItem, nameof(AvatarUrl));
            Organization = SqlUtils.EvalString(dataItem, nameof(Organization));
            Department = SqlUtils.EvalString(dataItem, nameof(Department));
            Position = SqlUtils.EvalString(dataItem, nameof(Position));
            Gender = SqlUtils.EvalString(dataItem, nameof(Gender));
            Birthday = SqlUtils.EvalString(dataItem, nameof(Birthday));
            Education = SqlUtils.EvalString(dataItem, nameof(Education));
            Graduation = SqlUtils.EvalString(dataItem, nameof(Graduation));
            Address = SqlUtils.EvalString(dataItem, nameof(Address));
            WeiXin = SqlUtils.EvalString(dataItem, nameof(WeiXin));
            Qq = SqlUtils.EvalString(dataItem, nameof(Qq));
            WeiBo = SqlUtils.EvalString(dataItem, nameof(WeiBo));
            Interests = SqlUtils.EvalString(dataItem, nameof(Interests));
            Signature = SqlUtils.EvalString(dataItem, nameof(Signature));
        }

        public int Id { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string PasswordFormat { get; set; }

        public string PasswordSalt { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime LastResetPasswordDate { get; set; }

        public DateTime LastActivityDate { get; set; }

        public int CountOfLogin { get; set; }

        public int CountOfFailedLogin { get; set; }

        public int CountOfWriting { get; set; }

        public bool IsChecked { get; set; }

        public bool IsLockedOut { get; set; }

        public string DisplayName { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public string AvatarUrl { get; set; }

        public string Organization { get; set; }

        public string Department { get; set; }

        public string Position { get; set; }

        public string Gender { get; set; }

        public string Birthday { get; set; }

        public string Education { get; set; }

        public string Graduation { get; set; }

        public string Address { get; set; }

        public string WeiXin { get; set; }

        public string Qq { get; set; }

        public string WeiBo { get; set; }

        public string Interests { get; set; }

        public string Signature { get; set; }
    }

    [Table(UserDao.DatabaseTableName)]
    public class UserInfoDatabase
    {
        public UserInfoDatabase()
        {
            Id = 0;
            UserName = string.Empty;
            Password = string.Empty;
            PasswordFormat = EPasswordFormatUtils.GetValue(EPasswordFormat.Encrypted);
            PasswordSalt = string.Empty;
            CreateDate = DateTime.Now;
            LastResetPasswordDate = DateTime.Now;
            LastActivityDate = DateTime.Now;
            CountOfLogin = 0;
            CountOfFailedLogin = 0;
            CountOfWriting = 0;
            IsChecked = true.ToString();
            IsLockedOut = false.ToString();
            DisplayName = string.Empty;
            Email = string.Empty;
            Mobile = string.Empty;
            AvatarUrl = string.Empty;
            Organization = string.Empty;
            Department = string.Empty;
            Position = string.Empty;
            Gender = string.Empty;
            Birthday = string.Empty;
            Education = string.Empty;
            Graduation = string.Empty;
            Address = string.Empty;
            WeiXin = string.Empty;
            Qq = string.Empty;
            WeiBo = string.Empty;
            Interests = string.Empty;
            Signature = string.Empty;
        }

        public UserInfoDatabase(UserInfo userInfo)
        {
            Id = userInfo.Id;
            UserName = userInfo.UserName;
            Password = userInfo.Password;
            PasswordFormat = userInfo.PasswordFormat;
            PasswordSalt = userInfo.PasswordSalt;
            CreateDate = userInfo.CreateDate;
            LastResetPasswordDate = userInfo.LastResetPasswordDate;
            LastActivityDate = userInfo.LastActivityDate;
            CountOfLogin = userInfo.CountOfLogin;
            CountOfFailedLogin = userInfo.CountOfFailedLogin;
            CountOfWriting = userInfo.CountOfWriting;
            IsChecked = userInfo.IsChecked.ToString();
            IsLockedOut = userInfo.IsLockedOut.ToString();
            DisplayName = userInfo.DisplayName;
            Email = userInfo.Email;
            Mobile = userInfo.Mobile;
            AvatarUrl = userInfo.AvatarUrl;
            Organization = userInfo.Organization;
            Department = userInfo.Department;
            Position = userInfo.Position;
            Gender = userInfo.Gender;
            Birthday = userInfo.Birthday;
            Education = userInfo.Education;
            Graduation = userInfo.Graduation;
            Address = userInfo.Address;
            WeiXin = userInfo.WeiXin;
            Qq = userInfo.Qq;
            WeiBo = userInfo.WeiBo;
            Interests = userInfo.Interests;
            Signature = userInfo.Signature;
        }

        public UserInfo ToUserInfo()
        {
            var userInfo = new UserInfo
            {
                Id = Id,
                UserName = UserName,
                Password = Password,
                PasswordFormat = PasswordFormat,
                PasswordSalt = PasswordSalt,
                CreateDate = CreateDate,
                LastResetPasswordDate = LastResetPasswordDate,
                LastActivityDate = LastActivityDate,
                CountOfLogin = CountOfLogin,
                CountOfFailedLogin = CountOfFailedLogin,
                CountOfWriting = CountOfWriting,
                IsChecked = TranslateUtils.ToBool(IsChecked),
                IsLockedOut = TranslateUtils.ToBool(IsLockedOut),
                DisplayName = DisplayName,
                Email = Email,
                Mobile = Mobile,
                AvatarUrl = AvatarUrl,
                Organization = Organization,
                Department = Department,
                Position = Position,
                Gender = Gender,
                Birthday = Birthday,
                Education = Education,
                Graduation = Graduation,
                Address = Address,
                WeiXin = WeiXin,
                Qq = Qq,
                WeiBo = WeiBo,
                Interests = Interests,
                Signature = Signature
            };

            return userInfo;
        }

        public int Id { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string PasswordFormat { get; set; }

        public string PasswordSalt { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime LastResetPasswordDate { get; set; }

        public DateTime LastActivityDate { get; set; }

        public int CountOfLogin { get; set; }

        public int CountOfFailedLogin { get; set; }

        public int CountOfWriting { get; set; }

        public string IsChecked { get; set; }

        public string IsLockedOut { get; set; }

        public string DisplayName { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public string AvatarUrl { get; set; }

        public string Organization { get; set; }

        public string Department { get; set; }

        public string Position { get; set; }

        public string Gender { get; set; }

        public string Birthday { get; set; }

        public string Education { get; set; }

        public string Graduation { get; set; }

        public string Address { get; set; }

        public string WeiXin { get; set; }

        public string Qq { get; set; }

        public string WeiBo { get; set; }

        public string Interests { get; set; }

        public string Signature { get; set; }
    }

    public class UserInfoCreateUpdate
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string PasswordFormat { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? LastResetPasswordDate { get; set; }

        public DateTime? LastActivityDate { get; set; }

        public int? CountOfLogin { get; set; }

        public int? CountOfFailedLogin { get; set; }

        public int? CountOfWriting { get; set; }

        public bool? IsChecked { get; set; }

        public bool? IsLockedOut { get; set; }

        public string DisplayName { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public string AvatarUrl { get; set; }

        public string Organization { get; set; }

        public string Department { get; set; }

        public string Position { get; set; }

        public string Gender { get; set; }

        public string Birthday { get; set; }

        public string Education { get; set; }

        public string Graduation { get; set; }

        public string Address { get; set; }

        public string WeiXin { get; set; }

        public string Qq { get; set; }

        public string WeiBo { get; set; }

        public string Interests { get; set; }

        public string Signature { get; set; }

        public void Load(UserInfoDatabase dbUserInfo)
        {
            if (UserName != null)
            {
                dbUserInfo.UserName = UserName;
            }

            if (Password != null)
            {
                dbUserInfo.Password = Password;
            }

            if (PasswordFormat != null)
            {
                dbUserInfo.PasswordFormat = PasswordFormat;
            }

            if (CreateDate != null)
            {
                dbUserInfo.CreateDate = (DateTime)CreateDate;
            }

            if (LastResetPasswordDate != null)
            {
                dbUserInfo.LastResetPasswordDate = (DateTime)LastResetPasswordDate;
            }

            if (LastActivityDate != null)
            {
                dbUserInfo.LastActivityDate = (DateTime)LastActivityDate;
            }

            if (CountOfLogin != null)
            {
                dbUserInfo.CountOfLogin = (int)CountOfLogin;
            }

            if (CountOfFailedLogin != null)
            {
                dbUserInfo.CountOfFailedLogin = (int)CountOfFailedLogin;
            }

            if (CountOfWriting != null)
            {
                dbUserInfo.CountOfWriting = (int)CountOfWriting;
            }

            if (IsChecked != null)
            {
                dbUserInfo.IsChecked = IsChecked.ToString();
            }

            if (IsLockedOut != null)
            {
                dbUserInfo.IsLockedOut = IsLockedOut.ToString();
            }

            if (DisplayName != null)
            {
                dbUserInfo.DisplayName = DisplayName;
            }

            if (Email != null)
            {
                dbUserInfo.Email = Email;
            }

            if (Mobile != null)
            {
                dbUserInfo.Mobile = Mobile;
            }

            if (AvatarUrl != null)
            {
                dbUserInfo.AvatarUrl = AvatarUrl;
            }

            if (Organization != null)
            {
                dbUserInfo.Organization = Organization;
            }

            if (Department != null)
            {
                dbUserInfo.Department = Department;
            }

            if (Position != null)
            {
                dbUserInfo.Position = Position;
            }

            if (Gender != null)
            {
                dbUserInfo.Gender = Gender;
            }

            if (Birthday != null)
            {
                dbUserInfo.Birthday = Birthday;
            }

            if (Education != null)
            {
                dbUserInfo.Education = Education;
            }

            if (Graduation != null)
            {
                dbUserInfo.Graduation = Graduation;
            }

            if (Address != null)
            {
                dbUserInfo.Address = Address;
            }

            if (WeiXin != null)
            {
                dbUserInfo.WeiXin = WeiXin;
            }

            if (Qq != null)
            {
                dbUserInfo.Qq = Qq;
            }

            if (WeiBo != null)
            {
                dbUserInfo.WeiBo = WeiBo;
            }

            if (Interests != null)
            {
                dbUserInfo.Interests = Interests;
            }

            if (Signature != null)
            {
                dbUserInfo.Signature = Signature;
            }
        }
    }
}
