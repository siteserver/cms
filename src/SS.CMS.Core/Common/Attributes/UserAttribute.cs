using System;
using System.Collections.Generic;
using SS.CMS.Models;

namespace SS.CMS.Core.Models.Attributes
{
    public static class UserAttribute
    {
        public const string Id = nameof(User.Id);
        public const string Guid = nameof(User.Guid);
        public const string CreatedDate = nameof(User.CreatedDate);
        public const string LastModifiedDate = nameof(User.LastModifiedDate);
        public const string UserName = nameof(User.UserName);
        public const string Password = nameof(User.Password);
        public const string PasswordFormat = nameof(User.PasswordFormat);
        public const string PasswordSalt = nameof(User.PasswordSalt);
        public const string LastResetPasswordDate = nameof(User.LastResetPasswordDate);
        public const string LastActivityDate = nameof(User.LastActivityDate);
        public const string CountOfLogin = nameof(User.CountOfLogin);
        public const string CountOfFailedLogin = nameof(User.CountOfFailedLogin);
        public const string GroupId = nameof(User.GroupId);
        public const string IsChecked = nameof(IsChecked);
        public const string IsLockedOut = nameof(IsLockedOut);
        public const string AvatarUrl = nameof(User.AvatarUrl);
        public const string DisplayName = nameof(User.DisplayName);
        public const string Mobile = nameof(User.Mobile);
        public const string Email = nameof(User.Email);
        public const string Bio = nameof(User.Bio);
        public const string ExtendValues = nameof(User.ExtendValues);

        public static readonly Lazy<List<string>> AllAttributes = new Lazy<List<string>>(() => new List<string>
        {
            Id,
            Guid,
            CreatedDate,
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
