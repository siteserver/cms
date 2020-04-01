using System;
using System.Collections.Generic;

namespace SiteServer.CMS.Model.Attributes
{
    public static class UserAttribute
    {
        public const string Id = nameof(UserInfo.Id);
        public const string UserName = nameof(UserInfo.UserName);
        public const string Password = nameof(UserInfo.Password);
        public const string PasswordFormat = nameof(UserInfo.PasswordFormat);
        public const string PasswordSalt = nameof(UserInfo.PasswordSalt);
        public const string CreateDate = nameof(UserInfo.CreateDate);
        public const string LastResetPasswordDate = nameof(UserInfo.LastResetPasswordDate);
        public const string LastActivityDate = nameof(UserInfo.LastActivityDate);
        public const string CountOfLogin = nameof(UserInfo.CountOfLogin);
        public const string CountOfFailedLogin = nameof(UserInfo.CountOfFailedLogin);
        public const string GroupId = nameof(UserInfo.GroupId);
        public const string IsChecked = nameof(UserInfo.IsChecked);
        public const string IsLockedOut = nameof(UserInfo.IsLockedOut);
        public const string AvatarUrl = nameof(UserInfo.AvatarUrl);
        public const string DisplayName = nameof(UserInfo.DisplayName);
        public const string Mobile = nameof(UserInfo.Mobile);
        public const string Email = nameof(UserInfo.Email);
        public const string Gender = nameof(UserInfo.Gender);
        public const string Birthday = nameof(UserInfo.Birthday);
        public const string WeiXin = nameof(UserInfo.WeiXin);
        public const string Qq = nameof(UserInfo.Qq);
        public const string WeiBo = nameof(UserInfo.WeiBo);
        public const string Bio = nameof(UserInfo.Bio);
        public const string SettingsXml = nameof(UserInfo.SettingsXml);

        public static readonly Lazy<List<string>> AllAttributes = new Lazy<List<string>>(() => new List<string>
        {
            Id,
            UserName,
            Password,
            PasswordFormat,
            PasswordSalt,
            CreateDate,
            LastResetPasswordDate,
            LastActivityDate,
            CountOfLogin,
            CountOfFailedLogin,
            GroupId,
            IsChecked,
            IsLockedOut,
            AvatarUrl,
            DisplayName,
            Mobile,
            Email,
            Gender,
            Birthday,
            WeiXin,
            Qq,
            WeiBo,
            Bio,
            SettingsXml
        });

        public static readonly Lazy<List<string>> TableStyleAttributes = new Lazy<List<string>>(() => new List<string>
        {
            DisplayName,
            Mobile,
            Email,
            Gender,
            Birthday,
            WeiXin,
            Qq,
            WeiBo,
            Bio
        });

        public static readonly Lazy<List<string>> ExcludedAttributes = new Lazy<List<string>>(() => new List<string>
        {
            Password,
            PasswordFormat,
            PasswordSalt,
            SettingsXml
        });
    }
}
