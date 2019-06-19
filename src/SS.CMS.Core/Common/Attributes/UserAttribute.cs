using System;
using System.Collections.Generic;
using SS.CMS.Models;

namespace SS.CMS.Core.Models.Attributes
{
    public static class UserAttribute
    {
        public const string Id = nameof(UserInfo.Id);
        public const string Guid = nameof(UserInfo.Guid);
        public const string CreationDate = nameof(UserInfo.CreationDate);
        public const string LastModifiedDate = nameof(UserInfo.LastModifiedDate);
        public const string UserName = nameof(UserInfo.UserName);
        public const string Password = nameof(UserInfo.Password);
        public const string PasswordFormat = nameof(UserInfo.PasswordFormat);
        public const string PasswordSalt = nameof(UserInfo.PasswordSalt);
        public const string LastResetPasswordDate = nameof(UserInfo.LastResetPasswordDate);
        public const string LastActivityDate = nameof(UserInfo.LastActivityDate);
        public const string CountOfLogin = nameof(UserInfo.CountOfLogin);
        public const string CountOfFailedLogin = nameof(UserInfo.CountOfFailedLogin);
        public const string GroupId = nameof(UserInfo.GroupId);
        public const string IsChecked = nameof(IsChecked);
        public const string IsLockedOut = nameof(IsLockedOut);
        public const string AvatarUrl = nameof(UserInfo.AvatarUrl);
        public const string DisplayName = nameof(UserInfo.DisplayName);
        public const string Mobile = nameof(UserInfo.Mobile);
        public const string Email = nameof(UserInfo.Email);
        public const string Bio = nameof(UserInfo.Bio);
        public const string ExtendValues = nameof(UserInfo.ExtendValues);

        public static readonly Lazy<List<string>> AllAttributes = new Lazy<List<string>>(() => new List<string>
        {
            Id,
            Guid,
            CreationDate,
            LastModifiedDate,
            UserName,
            Password,
            PasswordFormat,
            PasswordSalt,
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
            Bio,
            ExtendValues
        });

        public static readonly Lazy<List<string>> TableStyleAttributes = new Lazy<List<string>>(() => new List<string>
        {
            DisplayName,
            Mobile,
            Email,
            Bio
        });

        public static readonly Lazy<List<string>> ExcludedAttributes = new Lazy<List<string>>(() => new List<string>
        {
            Password,
            PasswordFormat,
            PasswordSalt,
            ExtendValues
        });
    }
}
