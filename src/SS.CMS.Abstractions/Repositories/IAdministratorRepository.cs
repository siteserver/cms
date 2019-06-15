using System.Collections.Generic;
using System.Threading.Tasks;
using SqlKata;
using SS.CMS.Abstractions.Enums;
using SS.CMS.Abstractions.Models;
using SS.CMS.Data;

namespace SS.CMS.Abstractions.Repositories
{
    public interface IAdministratorRepository : IRepository
    {
        IEnumerable<AdministratorInfo> GetAll(Query query);

        int GetCount(Query query);

        int GetCount();

        int Insert(AdministratorInfo adminInfo, out string errorMessage);

        bool Update(AdministratorInfo administratorInfo, out string errorMessage);

        bool UpdateLastActivityDateAndCountOfFailedLogin(AdministratorInfo adminInfo);

        void UpdateLastActivityDateAndCountOfLogin(AdministratorInfo adminInfo);

        void UpdateSiteIdCollection(AdministratorInfo adminInfo, string siteIdCollection);

        List<int> UpdateSiteId(AdministratorInfo adminInfo, int siteId);

        bool Delete(AdministratorInfo adminInfo);

        void Lock(List<int> userIdList);

        void UnLock(List<int> userIdList);

        AdministratorInfo GetByUserId(int userId);

        AdministratorInfo GetByUserName(string userName);

        AdministratorInfo GetByMobile(string mobile);

        AdministratorInfo GetByEmail(string email);

        /// <summary>
        /// 判断用户名是否存在。
        /// </summary>
        /// <param name="userName">用户名。</param>
        /// <returns>如果用户名存在，则返回 true，否则返回 false。</returns>
        bool IsUserNameExists(string adminName);

        /// <summary>
        /// 判断管理员邮箱是否存在。
        /// </summary>
        /// <param name="email">管理员邮箱。</param>
        /// <returns>如果管理员邮箱存在，则返回 true，否则返回 false。</returns>
        bool IsEmailExists(string email);

        /// <summary>
        /// 判断管理员手机是否存在。
        /// </summary>
        /// <param name="mobile">管理员手机。</param>
        /// <returns>如果管理员手机存在，则返回 true，否则返回 false。</returns>
        bool IsMobileExists(string mobile);

        int GetCountByAreaId(int areaId);

        int GetCountByDepartmentId(int departmentId);

        /// <summary>
        /// 获取管理员用户名列表。
        /// </summary>
        /// <returns>
        /// 管理员用户名列表。
        /// </returns>
        Task<IEnumerable<string>> GetUserNameListAsync();

        IEnumerable<string> GetUserNameList(int departmentId);

        bool ChangePassword(AdministratorInfo adminInfo, string password, out string errorMessage);

        bool Validate(string account, string password, bool isPasswordMd5, out string userName, out string errorMessage);

        bool CheckPassword(string password, bool isPasswordMd5, string dbPassword, PasswordFormat passwordFormat, string passwordSalt);

        // cache

        void ClearCache();

        void UpdateCache(AdministratorInfo adminInfo);

        void RemoveCache(AdministratorInfo adminInfo);

        /// <summary>
        /// 通过管理员Id获取管理员对象实例。
        /// </summary>
        /// <param name="userId">管理员Id。</param>
        /// <returns>
        /// 如果管理员Id不存在，则返回 null，否则返回指定的管理员对象实例。
        /// </returns>
        AdministratorInfo GetAdminInfoByUserId(int userId);

        /// <summary>
        /// 通过用户名获取管理员对象实例。
        /// </summary>
        /// <param name="userName">用户名。</param>
        /// <returns>
        /// 如果用户名不存在，则返回 null，否则返回指定的管理员对象实例。
        /// </returns>
        AdministratorInfo GetAdminInfoByUserName(string userName);

        /// <summary>
        /// 通过管理员手机获取管理员对象实例。
        /// </summary>
        /// <param name="mobile">用户手机。</param>
        /// <returns>
        /// 如果管理员手机不存在，则返回 null，否则返回指定的管理员对象实例。
        /// </returns>
        AdministratorInfo GetAdminInfoByMobile(string mobile);

        /// <summary>
        /// 通过管理员邮箱获取管理员对象实例。
        /// </summary>
        /// <param name="email">用户邮箱。</param>
        /// <returns>
        /// 如果管理员邮箱不存在，则返回 null，否则返回指定的管理员对象实例。
        /// </returns>
        AdministratorInfo GetAdminInfoByEmail(string email);

        /// <summary>
        /// 通过管理员账号获取用户对象实例。
        /// </summary>
        /// <param name="account">用户名、管理员邮箱或者管理员手机均可作为管理员账号。</param>
        /// <returns>
        /// 如果用户名、管理员邮箱或者管理员手机均不存在，则返回 null，否则返回指定的管理员对象实例。
        /// </returns>
        AdministratorInfo GetAdminInfoByAccount(string account);

        List<int> GetLatestTop10SiteIdList(List<int> siteIdListLatestAccessed, List<int> siteIdListOrderByLevel, List<int> siteIdListWithPermissions);

        string GetDisplayName(string userName);

        string GetRoleNames(string userName);

        bool IsSuperAdmin(string userName);
    }
}
