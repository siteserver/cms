using System;

namespace SiteServer.Plugin.Models
{
    public interface IAdministratorInfo
    {
        string UserName { get; set; }

        DateTime CreationDate { get; set; }

        DateTime LastActivityDate { get; set; }

        int CountOfLogin { get; set; }

        int CountOfFailedLogin { get; set; }

        string CreatorUserName { get; set; }

        bool IsLockedOut { get; set; }

        string PublishmentSystemIdCollection { get; set; }

        int PublishmentSystemId { get; set; }

        int DepartmentId { get; set; }

        int AreaId { get; set; }

        string DisplayName { get; set; }

        string Email { get; set; }

        string Mobile { get; set; }
    }
}
