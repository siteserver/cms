using System;

namespace SiteServer.Plugin
{
    /// <summary>
    /// 访问插件时的认证请求。
    /// </summary>
    public interface IRequest
    {
        /// <summary>
        /// 访问地址是否包含指定的查询字符串。
        /// </summary>
        /// <param name="name">查询字符串名称。</param>
        /// <returns>如果包含指定的查询字符串，则返回 true；否则返回 false。</returns>
        bool IsQueryExists(string name);

        /// <summary>
        /// 获取访问地址中的指定查询字符串。
        /// </summary>
        /// <param name="name">查询字符串名称。</param>
        /// <returns>如果包含指定的查询字符串，则返回对应的值；否则返回 null。</returns>
        string GetQueryString(string name);

        /// <summary>
        /// 获取访问地址中的指定查询字符串并转换为整数。
        /// </summary>
        /// <param name="name">查询字符串名称。</param>
        /// <param name="defaultValue">访问地址中不包含查询字符串或者无法转换为整数时的返回值。</param>
        /// <returns>如果包含指定的查询字符串，则返回转换为整数后的的值；否则返回 defaultValue。</returns>
        int GetQueryInt(string name, int defaultValue = 0);

        /// <summary>
        /// 获取访问地址中的指定查询字符串并转换为小数。
        /// </summary>
        /// <param name="name">查询字符串名称。</param>
        /// <param name="defaultValue">访问地址中不包含查询字符串或者无法转换为小数时的返回值。</param>
        /// <returns>如果包含指定的查询字符串，则返回转换为小数后的的值；否则返回 defaultValue。</returns>
        decimal GetQueryDecimal(string name, decimal defaultValue = 0);

        /// <summary>
        /// 获取访问地址中的指定查询字符串并转换为布尔值。
        /// </summary>
        /// <param name="name">查询字符串名称。</param>
        /// <param name="defaultValue">访问地址中不包含查询字符串或者无法转换为布尔值时的返回值。</param>
        /// <returns>如果包含指定的查询字符串，则返回转换为布尔值后的的值；否则返回 defaultValue。</returns>
        bool GetQueryBool(string name, bool defaultValue = false);

        /// <summary>
        /// JSON方式提交的Body中是否包含指定的键/值对。
        /// </summary>
        /// <param name="name">Body中包含的键。</param>
        /// <returns>如果Body中包含指定的键，则返回 true；否则返回 false。</returns>
        bool IsPostExists(string name);

        /// <summary>
        /// 获取JSON方式提交的Body中的指定键的值。
        /// </summary>
        /// <param name="name">Body中包含的键。</param>
        /// <returns>如果Body中包含指定的键，则返回对应的值；否则返回 null。</returns>
        string GetPostString(string name);

        /// <summary>
        /// 获取JSON方式提交的Body中的指定键的值并转换为整数。
        /// </summary>
        /// <param name="name">Body中包含的键。</param>
        /// <param name="defaultValue">Body中不包含键或者键值无法转换为整数时的返回值。</param>
        /// <returns>如果Body中包含指定的键，则返回转换为整数后的的值；否则返回 defaultValue。</returns>
        int GetPostInt(string name, int defaultValue = 0);

        /// <summary>
        /// 获取JSON方式提交的Body中的指定键的值并转换为小数。
        /// </summary>
        /// <param name="name">Body中包含的键。</param>
        /// <param name="defaultValue">Body中不包含键或者键值无法转换为小数时的返回值。</param>
        /// <returns>如果Body中包含指定的键，则返回转换为小数后的的值；否则返回 defaultValue。</returns>
        decimal GetPostDecimal(string name, decimal defaultValue = 0);

        /// <summary>
        /// 获取JSON方式提交的Body中的指定键的值并转换为布尔值。
        /// </summary>
        /// <param name="name">Body中包含的键。</param>
        /// <param name="defaultValue">Body中不包含键或者键值无法转换为布尔值时的返回值。</param>
        /// <returns>如果Body中包含指定的键，则返回转换为布尔值后的的值；否则返回 defaultValue。</returns>
        bool GetPostBool(string name, bool defaultValue = false);

        /// <summary>
        /// 获取JSON方式提交的Body中的指定键的值并转换为对应的类型。
        /// </summary>
        /// <param name="name">Body中包含的键，如果name为空则将这个Body作为对象进行转换。</param>
        /// <typeparam name="T">需要转换的类名称。</typeparam>
        /// <returns>如果Body中包含指定的键，则返回转换为指定对象后的的值；否则返回 null。</returns>
        T GetPostObject<T>(string name = "");

        /// <summary>
        /// 判断用户是否登录。
        /// </summary>
        bool IsUserLoggin { get; }

        /// <summary>
        /// 如果用户已登录，则返回登录用户的Id；否则返回 0。
        /// </summary>
        int UserId { get; }

        /// <summary>
        /// 如果用户已登录，则返回登录用户的用户名；否则返回空。
        /// </summary>
        string UserName { get; }

#pragma warning disable CS1591 // 缺少对公共可见类型或成员“IRequest.UserPermissions”的 XML 注释
        IPermissions UserPermissions { get; }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员“IRequest.UserPermissions”的 XML 注释

        /// <summary>
        /// 用户登录，调用此方法后系统将计算此用户的Token并存储在cookie中，同时此方法将返回用户Token，用于REST Api以及其他场景中。
        /// </summary>
        /// <param name="userName">登录用户的用户名。</param>
        /// <param name="isAutoLogin">
        /// 是否下次自动登录。
        /// 如果设置为 true，则登录cookie将保留7天；否则当浏览器关闭时系统将清除登录cookie。
        /// </param>
        /// <returns>返回此用户的Token。</returns>
        string UserLogin(string userName, bool isAutoLogin);

        /// <summary>
        /// 用户退出登录，调用此方法后系统将清除登录cookie。
        /// </summary>
        void UserLogout();

        /// <summary>
        /// 判断管理员是否登录。
        /// </summary>
        bool IsAdminLoggin { get; }

        /// <summary>
        /// 如果管理员已登录，则返回登录管理员的Id；否则返回 0。
        /// </summary>
        int AdminId { get; }

        /// <summary>
        /// 如果管理员已登录，则返回登录管理员的用户名；否则返回空。
        /// </summary>
        string AdminName { get; }

#pragma warning disable CS1591 // 缺少对公共可见类型或成员“IRequest.AdminPermissions”的 XML 注释
        IPermissions AdminPermissions { get; }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员“IRequest.AdminPermissions”的 XML 注释

        /// <summary>
        /// 管理员登录，调用此方法后系统将计算此管理员的Token并存储在cookie中，同时此方法将返回管理员Token，用于REST Api以及其他场景中。
        /// </summary>
        /// <param name="userName">登录管理员的用户名。</param>
        /// <param name="isAutoLogin">
        /// 是否下次自动登录。
        /// 如果设置为 true，则登录cookie将保留7天；否则当浏览器关闭时系统将清除登录cookie。
        /// </param>
        /// <returns>返回此管理员的Token。</returns>
        string AdminLogin(string userName, bool isAutoLogin);

        /// <summary>
        /// 管理员退出登录，调用此方法后系统将清除登录cookie。
        /// </summary>
        void AdminLogout();

        /// <summary>
        /// 是否针对此插件的REST Api访问包含Api认证Token。
        /// </summary>
        bool IsApiAuthenticated { get; }

        /// <summary>
        /// 设置cookie。
        /// 此cookie将随着浏览器的关闭而删除。
        /// </summary>
        /// <param name="name">cookie名称。</param>
        /// <param name="value">cookie值。</param>
        void SetCookie(string name, string value);

        /// <summary>
        /// 设置cookie。
        /// 此cookie在到期时间后将自动删除。
        /// </summary>
        /// <param name="name">cookie名称。</param>
        /// <param name="value">cookie值。</param>
        /// <param name="expiresAt">到期时间。</param>
        void SetCookie(string name, string value, TimeSpan expiresAt);

        /// <summary>
        /// 获取cookie。
        /// </summary>
        /// <param name="name">cookie名称。</param>
        /// <returns>如果cookie存在，则返回cookie的值；否则返回 null。</returns>
        string GetCookie(string name);

        /// <summary>
        /// 判断指定的cookie是否存在。
        /// </summary>
        /// <param name="name">cookie名称。</param>
        /// <returns>如果cookie存在，则返回 true；否则返回 false。</returns>
        bool IsCookieExists(string name);
    }
}