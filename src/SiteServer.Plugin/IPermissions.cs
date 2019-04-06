using System.Collections.Generic;

namespace SiteServer.Plugin
{
    /// <summary>
    /// 登录账号的权限。
    /// </summary>
    public interface IPermissions
    {
        /// <summary>
        /// 判断管理员是否是超级管理员。
        /// </summary>
        /// <returns>
        /// 如果管理员是超级管理员，则返回 true，否则返回 false。
        /// </returns>
        bool IsSuperAdmin();

        /// <summary>
        /// 判断管理员是否是站点管理员。
        /// </summary>
        /// <param name="siteId">站点Id。</param>
        /// <returns>
        /// 如果管理员是对应的站点管理员，则返回 true，否则返回 false。
        /// </returns>
        bool IsSiteAdmin(int siteId);

        /// <summary>
        /// 获取管理员/用户拥有权限的站点Id列表。
        /// </summary>
        /// <returns>
        /// 管理员/用户拥有权限的站点Id列表。
        /// </returns>
        List<int> GetSiteIdList();

        /// <summary>
        /// 获取管理员/用户拥有权限的栏目Id列表。
        /// </summary>
        /// <param name="siteId">站点Id。</param>
        /// <param name="permissions">栏目权限，可以输入多个。</param>
        /// <returns>
        /// 管理员/用户拥有权限的栏目Id列表。
        /// </returns>
        List<int> GetChannelIdList(int siteId, params string[] permissions);

        /// <summary>
        /// 判断管理员/用户是否拥有指定的系统权限（非站点权限）。
        /// </summary>
        /// <param name="permissions">系统权限，可以输入多个。</param>
        /// <returns>
        /// 如果管理员/用户拥有指定的系统权限，则返回 true，否则返回 false。
        /// </returns>
        bool HasSystemPermissions(params string[] permissions);

        /// <summary>
        /// 判断管理员/用户是否拥有指定的站点权限。
        /// </summary>
        /// <param name="siteId">站点Id。</param>
        /// <param name="permissions">站点权限，可以输入多个。</param>
        /// <returns>
        /// 如果管理员/用户拥有指定的站点权限，则返回 true，否则返回 false。
        /// </returns>
        bool HasSitePermissions(int siteId, params string[] permissions);

        /// <summary>
        /// 判断管理员/用户是否拥有指定的栏目权限。
        /// </summary>
        /// <param name="siteId">站点Id。</param>
        /// <param name="channelId">栏目Id。</param>
        /// <param name="permissions">栏目权限，可以输入多个。</param>
        /// <returns>
        /// 如果管理员/用户拥有指定的栏目权限，则返回 true，否则返回 false。
        /// </returns>
        bool HasChannelPermissions(int siteId, int channelId, params string[] permissions);
    }
}