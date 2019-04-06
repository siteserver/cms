using System;

namespace SiteServer.Plugin
{
    /// <summary>
    /// Access Token接口。
    /// </summary>
    public interface IAccessToken
    {
        /// <summary>
        /// 用户Id。
        /// </summary>
        int UserId { get; set; }

        /// <summary>
        /// 用户名。
        /// </summary>
        string UserName { get; set; }

        /// <summary>
        /// 到期时间。
        /// </summary>
        DateTime ExpiresAt { get; set; }
    }
}
