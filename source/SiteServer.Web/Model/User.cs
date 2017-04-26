using System;
using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.CMS.Core;

namespace SiteServer.API.Model
{
    public class User
    {
        public User(UserInfo userInfo)
        {
            AvatarUrl = PageUtility.GetUserAvatarUrl(PageUtils.GetApiUrl(), userInfo);

            if (userInfo == null) return;

            Id = userInfo.UserId;
            UserName = userInfo.UserName;
            GroupId = userInfo.GroupId;
            CreateDate = userInfo.CreateDate;
            LastResetPasswordDate = userInfo.LastResetPasswordDate;
            LastActivityDate = userInfo.LastActivityDate;
            CountOfLogin = userInfo.CountOfLogin;
            CountOfFailedLogin = userInfo.CountOfFailedLogin;
            CountOfWriting = userInfo.CountOfWriting;
            IsChecked = userInfo.IsChecked;
            IsLockedOut = userInfo.IsLockedOut;
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
            IsAnonymous = string.IsNullOrEmpty(userInfo.UserName);
            Additional = userInfo.Additional;
        }

        public int Id { get; set; }

        public string UserName { get; set; }

        public int GroupId { get; set; }

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

        public bool IsAnonymous { get; set; }

        public UserInfoExtend Additional { get; set; }
    }
}