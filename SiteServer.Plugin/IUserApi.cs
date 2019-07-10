using System;
using System.Collections.Generic;

namespace SiteServer.Plugin
{
    /// <summary>
    /// 用户Api接口。
    /// </summary>
    public interface IUserApi
    {
        /// <summary>
        /// 初始化 <see cref="T:SiteServer.Plugin.IUserInfo" /> 类的新实例。
        /// </summary>
        /// <returns>
        /// 返回用户对象实例。
        /// </returns>
        IUserInfo NewInstance();

        /// <summary>
        /// 通过用户Id获取用户对象实例。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <returns>
        /// 如果用户Id不存在，则返回 null，否则返回指定的用户对象实例。
        /// </returns>
        IUserInfo GetUserInfoByUserId(int userId);

        /// <summary>
        /// 通过用户名获取用户对象实例。
        /// </summary>
        /// <param name="userName">用户名。</param>
        /// <returns>
        /// 如果用户名不存在，则返回 null，否则返回指定的用户对象实例。
        /// </returns>
        IUserInfo GetUserInfoByUserName(string userName);

        /// <summary>
        /// 通过用户邮箱获取用户对象实例。
        /// </summary>
        /// <param name="email">用户邮箱。</param>
        /// <returns>
        /// 如果用户邮箱不存在，则返回 null，否则返回指定的用户对象实例。
        /// </returns>
        IUserInfo GetUserInfoByEmail(string email);

        /// <summary>
        /// 通过用户手机获取用户对象实例。
        /// </summary>
        /// <param name="mobile">用户手机。</param>
        /// <returns>
        /// 如果用户手机不存在，则返回 null，否则返回指定的用户对象实例。
        /// </returns>
        IUserInfo GetUserInfoByMobile(string mobile);

        /// <summary>
        /// 通过用户账号获取用户对象实例。
        /// </summary>
        /// <param name="account">用户名、用户邮箱或者用户手机均可作为用户账号。</param>
        /// <returns>
        /// 如果用户名、用户邮箱或者用户手机均不存在，则返回 null，否则返回指定的用户对象实例。
        /// </returns>
        IUserInfo GetUserInfoByAccount(string account);

        /// <summary>
        /// 判断用户名是否存在。
        /// </summary>
        /// <param name="userName">用户名。</param>
        /// <returns>如果用户名存在，则返回 true，否则返回 false。</returns>
        bool IsUserNameExists(string userName);

        /// <summary>
        /// 判断用户邮箱是否存在。
        /// </summary>
        /// <param name="email">用户邮箱。</param>
        /// <returns>如果用户邮箱存在，则返回 true，否则返回 false。</returns>
        bool IsEmailExists(string email);

        /// <summary>
        /// 判断用户手机是否存在。
        /// </summary>
        /// <param name="mobile">用户手机。</param>
        /// <returns>如果用户手机存在，则返回 true，否则返回 false。</returns>
        bool IsMobileExists(string mobile);

        /// <summary>
        /// 新增用户。
        /// </summary>
        /// <param name="userInfo">用户对象实例。</param>
        /// <param name="password">用户登录密码。</param>
        /// <param name="errorMessage">如果新增失败用户，返回失败原因。</param>
        /// <returns>如果新增用户成功，则返回 true，否则返回 false。</returns>
        bool Insert(IUserInfo userInfo, string password, out string errorMessage);

        /// <summary>
        /// 验证用户登录。
        /// </summary>
        /// <param name="account">用户账号（用户名、用户邮箱或者用户手机均可）。</param>
        /// <param name="password">用户密码。</param>
        /// <param name="userName">如果验证成功，返回用户的用户名。</param>
        /// <param name="errorMessage">如果验证失败，返回失败原因。</param>
        /// <returns>如果用户验证成功，则返回 true，否则返回 false。</returns>
        bool Validate(string account, string password, out string userName, out string errorMessage);

        /// <summary>
        /// 修改用户登录密码。
        /// </summary>
        /// <param name="userName">用户名。</param>
        /// <param name="password">新密码。</param>
        /// <param name="errorMessage">如果修改用户密码失败，返回失败原因。</param>
        /// <returns>如果密码修改成功，则返回 true，否则返回 false。</returns>
        bool ChangePassword(string userName, string password, out string errorMessage);

        /// <summary>
        /// 修改用户属性。
        /// </summary>
        /// <param name="userInfo">用户对象实例。</param>
        void Update(IUserInfo userInfo);

        /// <summary>
        /// 判断用户密码是否符合规则。
        /// </summary>
        /// <param name="password">用户密码。</param>
        /// <param name="errorMessage">如果用户密码不符合规则，返回原因。</param>
        /// <returns>如果密码符合规则，则返回 true，否则返回 false。</returns>
        bool IsPasswordCorrect(string password, out string errorMessage);

        /// <summary>
        /// 添加用户日志。
        /// </summary>
        /// <param name="userName">用户名。</param>
        /// <param name="action">动作。</param>
        /// <param name="summary">描述。</param>
        void AddLog(string userName, string action, string summary);

        /// <summary>
        /// 获取用户日志列表。
        /// </summary>
        /// <param name="userName">用户名。</param>
        /// <param name="totalNum">需要获取的日志总数。</param>
        /// <param name="action">动作，可以为空。</param>
        /// <returns>返回用户日志列表。</returns>
        List<ILogInfo> GetLogs(string userName, int totalNum, string action = "");

        /// <summary>
        /// 获取Access Token字符串。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="userName">用户名。</param>
        /// <param name="expiresAt">Access Token 到期时间。</param>
        /// <returns>返回此用户的Access Token。</returns>
        string GetAccessToken(int userId, string userName, TimeSpan expiresAt);

        /// <summary>
        /// 解析Access Token字符串。
        /// </summary>
        /// <param name="accessToken">用户Access Token。</param>
        /// <returns>存储于用户Token中的用户名。</returns>
        IAccessToken ParseAccessToken(string accessToken);
    }
}
