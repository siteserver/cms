using System;

namespace SiteServer.Plugin.Models
{
    public interface IUserInfo
    {
        int UserId { get; set; }

        string UserName { get; set; }

        DateTime CreateDate { get; set; }

        DateTime LastResetPasswordDate { get; set; }

        DateTime LastActivityDate { get; set; }

        int CountOfLogin { get; set; }

        int CountOfFailedLogin { get; set; }

        int CountOfWriting { get; set; }

        bool IsChecked { get; set; }

        bool IsLockedOut { get; set; }

        string DisplayName { get; set; }

        string Email { get; set; }

        string Mobile { get; set; }

        string AvatarUrl { get; set; }

        string Organization { get; set; }

        string Department { get; set; }

        string Position { get; set; }

        string Gender { get; set; }

        string Birthday { get; set; }

        string Education { get; set; }

        string Graduation { get; set; }

        string Address { get; set; }

        string WeiXin { get; set; }

        string Qq { get; set; }

        string WeiBo { get; set; }

        string Interests { get; set; }

        string Signature { get; set; }
    }
}
