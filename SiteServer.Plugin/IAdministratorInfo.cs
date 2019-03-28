using System;

namespace SiteServer.Plugin
{
    /// <summary>
    /// 管理员实体接口。
    /// 对应数据库中的siteserver_Administrator表。
    /// </summary>
    public interface IAdministratorInfo
    {
        /// <summary>
        /// 自增长主键。
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// 管理员账号，具有唯一性。
        /// </summary>
        string UserName { get; set; }

        /// <summary>
        /// 创建时间。
        /// </summary>
        DateTime? CreationDate { get; set; }

        /// <summary>
        /// 最后活动时间。
        /// </summary>
        DateTime? LastActivityDate { get; set; }

        /// <summary>
        /// 登录次数。
        /// </summary>
        int CountOfLogin { get; set; }

        /// <summary>
        /// 连续登录失败次数。
        /// </summary>
        int CountOfFailedLogin { get; set; }

        /// <summary>
        /// 管理员创建者。
        /// </summary>
        string CreatorUserName { get; set; }

        /// <summary>
        /// 是否被锁定。
        /// </summary>
        bool Locked { get; set; }

        /// <summary>
        /// 拥有管理权限的站点Id列表。
        /// </summary>
        string SiteIdCollection { get; set; }

        /// <summary>
        /// 最后一次管理的站点Id。
        /// </summary>
        int SiteId { get; set; }

        /// <summary>
        /// 所属部门Id，对应 siteserver_Department 表的 Id 字段。
        /// </summary>
        int DepartmentId { get; set; }

        /// <summary>
        /// 所在区域Id，对应 siteserver_Area 表的 Id 字段。
        /// </summary>
        int AreaId { get; set; }

        /// <summary>
        /// 管理员显示名称。
        /// </summary>
        string DisplayName { get; set; }

        /// <summary>
        /// 电子邮箱，具有唯一性，可作为登录账号使用。
        /// </summary>
        string Email { get; set; }

        /// <summary>
        /// 手机号码，具有唯一性，可作为登录账号使用。
        /// </summary>
        string Mobile { get; set; }

        /// <summary>
        /// 管理员头像图片地址。
        /// </summary>
        string AvatarUrl { get; set; }
    }
}
