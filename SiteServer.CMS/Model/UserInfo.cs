using System;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Model
{
    public class UserInfo: IUserInfo
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
}
