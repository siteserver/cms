using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Abstractions.Models;
using SS.CMS.Data;

namespace SS.CMS.Abstractions.Repositories
{
    public interface IAccessTokenRepository : IRepository
    {
        Task<int> InsertAsync(AccessTokenInfo accessTokenInfo);

        Task<bool> UpdateAsync(AccessTokenInfo accessTokenInfo);

        Task<bool> DeleteAsync(int id);

        Task<string> RegenerateAsync(AccessTokenInfo accessTokenInfo);

        Task<bool> IsScopeAsync(string token, string scope);

        Task<AccessTokenInfo> GetAsync(string token);

        Task<AccessTokenInfo> GetAsync(int id);

        Task<IEnumerable<AccessTokenInfo>> GetAllAsync();

        Task<bool> IsTitleExistsAsync(string title);

        string ScopeContents { get; }
        string ScopeAdministrators { get; }
        string ScopeUsers { get; }
        string ScopeStl { get; }

        List<string> ScopeList { get; }

        /// <summary>
        /// 获取Access Token字符串。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="userName">用户名。</param>
        /// <param name="expiresAt">Access Token 到期时间。</param>
        /// <returns>返回此用户的Access Token。</returns>
        string GetAccessToken(int userId, string userName, TimeSpan expiresAt);

        /// <summary>
        /// 获取Access Token字符串。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="userName">用户名。</param>
        /// <param name="expiresAt">Access Token 到期时间。</param>
        /// <returns>返回此用户的Access Token。</returns>
        string GetAccessToken(int userId, string userName, DateTime expiresAt);

        /// <summary>
        /// 解析Access Token字符串。
        /// </summary>
        /// <param name="accessToken">用户Access Token。</param>
        /// <returns>存储于用户Token中的用户名。</returns>
        IAccessToken ParseAccessToken(string accessToken);
    }
}
