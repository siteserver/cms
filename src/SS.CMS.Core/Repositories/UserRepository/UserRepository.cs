using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using SqlKata;
using SS.CMS.Core.Security;
using SS.CMS.Data;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;
using SS.CMS.Utils;
using SS.CMS.Utils.Enumerations;

namespace SS.CMS.Core.Repositories
{
    public partial class UserRepository : IUserRepository
    {
        private readonly IDistributedCache _cache;
        private readonly ISettingsManager _settingsManager;
        private readonly IConfigRepository _configRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly Repository<UserInfo> _repository;

        public UserRepository(IDistributedCache cache, ISettingsManager settingsManager, IConfigRepository configRepository, IUserRoleRepository userRoleRepository)
        {
            _cache = cache;
            _settingsManager = settingsManager;
            _configRepository = configRepository;
            _userRoleRepository = userRoleRepository;
            _repository = new Repository<UserInfo>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string Id = nameof(UserInfo.Id);
            public const string UserName = nameof(UserInfo.UserName);
            public const string DepartmentId = nameof(UserInfo.DepartmentId);
            public const string AreaId = nameof(UserInfo.AreaId);
            public const string Mobile = nameof(UserInfo.Mobile);
            public const string Email = nameof(UserInfo.Email);
            public const string Password = nameof(UserInfo.Password);
            public const string PasswordFormat = nameof(UserInfo.PasswordFormat);
            public const string PasswordSalt = nameof(UserInfo.PasswordSalt);
            public const string SiteId = nameof(UserInfo.SiteId);
            public const string SiteIdCollection = nameof(UserInfo.SiteIdCollection);
            public const string IsLockedOut = "IsLockedOut";
        }

        public IEnumerable<UserInfo> GetAll(Query query)
        {
            return _repository.GetAll(query);
        }

        public int GetCount(Query query)
        {
            return _repository.Count(query);
        }

        public int GetCount()
        {
            return _repository.Count();
        }

        public async Task<(bool IsSuccess, int UserId, string ErrorMessage)> InsertAsync(UserInfo userInfo)
        {
            var (isSuccess, errorMessage) = await InsertValidateAsync(userInfo.UserName, userInfo.Password, userInfo.Email, userInfo.Mobile);

            if (!isSuccess) return (false, 0, errorMessage);

            try
            {
                userInfo.CreatedDate = DateTime.Now;
                userInfo.PasswordFormat = PasswordFormat.Encrypted.Value;
                userInfo.Password = EncodePassword(userInfo.Password, PasswordFormat.Parse(userInfo.PasswordFormat), out var passwordSalt);
                userInfo.PasswordSalt = passwordSalt;

                var identity = await _repository.InsertAsync(userInfo);

                return (true, identity, null);
            }
            catch (Exception ex)
            {
                return (false, 0, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> UpdateAsync(UserInfo userInfo)
        {
            var oldUserInfo = await GetByUserIdAsync(userInfo.Id);
            await RemoveCacheAsync(oldUserInfo);

            userInfo.Password = oldUserInfo.Password;
            userInfo.PasswordFormat = oldUserInfo.PasswordFormat;
            userInfo.PasswordSalt = oldUserInfo.PasswordSalt;

            var (isSuccess, errorMessage) = await UpdateValidateAsync(userInfo, oldUserInfo.UserName, oldUserInfo.Email, oldUserInfo.Mobile);

            if (!isSuccess) return (false, errorMessage);

            var updated = await _repository.UpdateAsync(userInfo);

            return (updated, null);
        }

        public async Task<bool> UpdateLastActivityDateAndCountOfFailedLoginAsync(UserInfo userInfo)
        {
            if (userInfo == null) return false;

            userInfo.LastActivityDate = DateTime.Now;
            userInfo.CountOfFailedLogin += 1;

            var (updated, _) = await UpdateAsync(userInfo);
            if (updated)
            {
                await RemoveCacheAsync(userInfo);
            }
            return updated;
        }

        public async Task<bool> UpdateLastActivityDateAndCountOfLoginAsync(UserInfo userInfo)
        {
            if (userInfo == null) return false;

            userInfo.LastActivityDate = DateTime.Now;
            userInfo.CountOfLogin += 1;
            userInfo.CountOfFailedLogin = 0;

            var (updated, _) = await UpdateAsync(userInfo);
            if (updated)
            {
                await RemoveCacheAsync(userInfo);
            }
            return updated;
        }

        public async Task UpdateSiteIdCollectionAsync(UserInfo userInfo, string siteIdCollection)
        {
            if (userInfo == null) return;

            userInfo.SiteIdCollection = siteIdCollection;

            await _repository.UpdateAsync(Q
                    .Set(Attr.SiteIdCollection, siteIdCollection)
                    .Where(Attr.Id, userInfo.Id)
                );

            await RemoveCacheAsync(userInfo);
        }

        public async Task<List<int>> UpdateSiteIdAsync(UserInfo userInfo, int siteId)
        {
            if (userInfo == null) return null;

            var siteIdListLatestAccessed = TranslateUtils.StringCollectionToIntList(userInfo.SiteIdCollection);
            if (userInfo.SiteId != siteId || siteIdListLatestAccessed.FirstOrDefault() != siteId)
            {
                siteIdListLatestAccessed.Remove(siteId);
                siteIdListLatestAccessed.Insert(0, siteId);

                userInfo.SiteIdCollection = TranslateUtils.ObjectCollectionToString(siteIdListLatestAccessed);
                userInfo.SiteId = siteId;

                await _repository.UpdateAsync(Q
                    .Set(Attr.SiteId, siteId)
                    .Set(Attr.SiteIdCollection, TranslateUtils.ObjectCollectionToString(siteIdListLatestAccessed))
                    .Where(Attr.Id, userInfo.Id)
                );

                await RemoveCacheAsync(userInfo);
            }

            return siteIdListLatestAccessed;
        }

        private async Task ChangePasswordAsync(UserInfo userInfo, PasswordFormat passwordFormat, string passwordSalt, string password)
        {
            userInfo.Password = password;
            userInfo.PasswordFormat = passwordFormat.Value;
            userInfo.PasswordSalt = passwordSalt;

            await _repository.UpdateAsync(userInfo, Attr.Password, Attr.PasswordFormat, Attr.PasswordSalt);

            await RemoveCacheAsync(userInfo);
        }

        public async Task<bool> DeleteAsync(UserInfo userInfo)
        {
            if (userInfo == null) return false;

            var deleted = await _repository.DeleteAsync(userInfo.Id);

            if (deleted)
            {
                await RemoveCacheAsync(userInfo);

                await _userRoleRepository.RemoveUserAsync(userInfo.UserName);
            }

            return deleted;
        }

        public async Task LockAsync(List<int> userIdList)
        {
            foreach (var userId in userIdList)
            {
                var userInfo = await GetByUserIdAsync(userId);
                await RemoveCacheAsync(userInfo);
            }

            await _repository.UpdateAsync(Q
                .Set(Attr.IsLockedOut, true.ToString())
                .WhereIn(Attr.Id, userIdList)
            );
        }

        public async Task UnLockAsync(List<int> userIdList)
        {
            foreach (var userId in userIdList)
            {
                var userInfo = await GetByUserIdAsync(userId);
                await RemoveCacheAsync(userInfo);
            }

            await _repository.UpdateAsync(Q
                .Set(Attr.IsLockedOut, false.ToString())
                .WhereIn(Attr.Id, userIdList)
            );
        }

        public async Task<UserInfo> GetByAccountAsync(string account)
        {
            var UserInfo = await GetByUserNameAsync(account);
            if (UserInfo != null) return UserInfo;
            if (StringUtils.IsMobile(account)) return await GetByMobileAsync(account);
            if (StringUtils.IsEmail(account)) return await GetByEmailAsync(account);

            return null;
        }

        public async Task<UserInfo> GetByUserIdAsync(int userId)
        {
            if (userId <= 0) return null;

            var cacheKey = _cache.GetKey(nameof(UserRepository), nameof(GetByUserIdAsync), userId.ToString());
            return await _cache.GetOrCreateAsync(cacheKey, async options =>
            {
                return await _repository.GetAsync(userId);
            });
        }

        public async Task<UserInfo> GetByUserNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) return null;

            var cacheKey = _cache.GetKey(nameof(UserRepository), nameof(GetByUserNameAsync), userName);
            return await _cache.GetOrCreateAsync(cacheKey, async options =>
            {
                return await _repository.GetAsync(Q.Where(Attr.UserName, userName));
            });
        }

        public async Task<UserInfo> GetByMobileAsync(string mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile)) return null;

            var cacheKey = _cache.GetKey(nameof(UserRepository), nameof(GetByMobileAsync), mobile);
            return await _cache.GetOrCreateAsync(cacheKey, async options =>
            {
                return await _repository.GetAsync(Q.Where(Attr.Mobile, mobile));
            });
        }

        public async Task<UserInfo> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;

            var cacheKey = _cache.GetKey(nameof(UserRepository), nameof(GetByEmailAsync), email);
            return await _cache.GetOrCreateAsync(cacheKey, async options =>
            {
                return await _repository.GetAsync(Q.Where(Attr.Email, email));
            });
        }

        public bool IsUserNameExists(string userName)
        {
            return _repository.Exists(Q.Where(Attr.UserName, userName));
        }

        public bool IsEmailExists(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;

            var exists = IsUserNameExists(email);
            if (exists) return true;

            return _repository.Exists(Q
                .Where(Attr.Email, email));
        }

        public bool IsMobileExists(string mobile)
        {
            if (string.IsNullOrEmpty(mobile)) return false;

            var exists = IsUserNameExists(mobile);
            if (exists) return true;

            return _repository.Exists(Q
                .Where(Attr.Mobile, mobile));
        }

        public int GetCountByAreaId(int areaId)
        {
            return _repository.Count(Q
                .Where(Attr.AreaId, areaId));
        }

        public int GetCountByDepartmentId(int departmentId)
        {
            return _repository.Count(Q
                .Where(Attr.DepartmentId, departmentId));
        }

        /// <summary>
        /// 获取管理员用户名列表。
        /// </summary>
        /// <returns>
        /// 管理员用户名列表。
        /// </returns>
        public async Task<IEnumerable<string>> GetUserNameListAsync()
        {
            return await _repository.GetAllAsync<string>(Q.Select(Attr.UserName));
        }

        public IEnumerable<string> GetUserNameList(int departmentId)
        {
            return _repository.GetAll<string>(Q
                .Select(Attr.UserName)
                .Where(Attr.DepartmentId, departmentId));
        }

        private async Task<(bool IsSuccess, string ErrorMessage)> UpdateValidateAsync(UserInfo adminInfoToUpdate, string userName, string email, string mobile)
        {
            var configInfo = await _configRepository.GetConfigInfoAsync();

            if (adminInfoToUpdate.UserName != null && adminInfoToUpdate.UserName != userName)
            {
                if (string.IsNullOrEmpty(adminInfoToUpdate.UserName))
                {
                    return (false, "用户名不能为空");
                }
                if (adminInfoToUpdate.UserName.Length < configInfo.AdminUserNameMinLength)
                {
                    return (false, $"用户名长度必须大于等于 {configInfo.AdminUserNameMinLength}");
                }
                if (IsUserNameExists(adminInfoToUpdate.UserName))
                {
                    return (false, "用户名已存在，请更换用户名");
                }
            }

            if (adminInfoToUpdate.Mobile != null && adminInfoToUpdate.Mobile != mobile)
            {
                if (!string.IsNullOrEmpty(adminInfoToUpdate.Mobile) && IsMobileExists(adminInfoToUpdate.Mobile))
                {
                    return (false, "手机号码已被注册，请更换手机号码");
                }
            }

            if (adminInfoToUpdate.Email != null && adminInfoToUpdate.Email != email)
            {
                if (!string.IsNullOrEmpty(adminInfoToUpdate.Email) && IsEmailExists(adminInfoToUpdate.Email))
                {
                    return (false, "电子邮件地址已被注册，请更换邮箱");
                }
            }

            return (true, null);
        }

        private async Task<(bool IsSuccess, string ErrorMessage)> InsertValidateAsync(string userName, string password, string email, string mobile)
        {
            var configInfo = await _configRepository.GetConfigInfoAsync();

            if (string.IsNullOrEmpty(userName))
            {
                return (false, "用户名不能为空");
            }
            if (userName.Length < configInfo.AdminUserNameMinLength)
            {
                return (false, $"用户名长度必须大于等于{configInfo.AdminUserNameMinLength}");
            }
            if (IsUserNameExists(userName))
            {
                return (false, "用户名已存在，请更换用户名");
            }

            if (string.IsNullOrEmpty(password))
            {
                return (false, "密码不能为空");
            }
            if (password.Length < configInfo.AdminPasswordMinLength)
            {
                return (false, $"密码长度必须大于等于{configInfo.AdminPasswordMinLength}");
            }
            if (
                !EUserPasswordRestrictionUtils.IsValid(password,
                    configInfo.AdminPasswordRestriction))
            {
                return (false, $"密码不符合规则，请包含{EUserPasswordRestrictionUtils.GetText(EUserPasswordRestrictionUtils.GetEnumType(configInfo.AdminPasswordRestriction))}");
            }

            if (!string.IsNullOrEmpty(mobile) && IsMobileExists(mobile))
            {
                return (false, "手机号码已被注册，请更换手机号码");
            }
            if (!string.IsNullOrEmpty(email) && IsEmailExists(email))
            {
                return (false, "电子邮件地址已被注册，请更换邮箱");
            }

            return (true, null);
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> ChangePasswordAsync(UserInfo userInfo, string password)
        {
            var configInfo = await _configRepository.GetConfigInfoAsync();

            if (string.IsNullOrEmpty(password))
            {
                return (false, "密码不能为空");
            }
            if (password.Length < configInfo.AdminPasswordMinLength)
            {
                return (false, $"密码长度必须大于等于{configInfo.AdminPasswordMinLength}");
            }
            if (
                !EUserPasswordRestrictionUtils.IsValid(password, configInfo.AdminPasswordRestriction))
            {
                return (false, $"密码不符合规则，请包含{EUserPasswordRestrictionUtils.GetText(EUserPasswordRestrictionUtils.GetEnumType(configInfo.AdminPasswordRestriction))}");
            }

            password = EncodePassword(password, PasswordFormat.Encrypted, out var passwordSalt);
            await ChangePasswordAsync(userInfo, PasswordFormat.Encrypted, passwordSalt, password);

            return (true, null);
        }

        public async Task<(bool IsSuccess, string UserName, string ErrorMessage)> ValidateAsync(string account, string password, bool isPasswordMd5)
        {
            var configInfo = await _configRepository.GetConfigInfoAsync();

            if (string.IsNullOrEmpty(account))
            {
                return (false, null, "账号不能为空");
            }
            if (string.IsNullOrEmpty(password))
            {
                return (false, null, "密码不能为空");
            }

            var userInfo = await GetByAccountAsync(account);
            if (string.IsNullOrEmpty(userInfo?.UserName))
            {
                return (false, null, "帐号或密码错误");
            }

            var userName = userInfo.UserName;

            if (userInfo.IsLockedOut)
            {
                return (false, userName, "此账号被锁定，无法登录");
            }

            if (configInfo.IsAdminLockLogin)
            {
                if (userInfo.CountOfFailedLogin > 0 &&
                    userInfo.CountOfFailedLogin >= configInfo.AdminLockLoginCount)
                {
                    var lockType = EUserLockTypeUtils.GetEnumType(configInfo.AdminLockLoginType);
                    if (lockType == EUserLockType.Forever)
                    {
                        return (false, userName, "此账号错误登录次数过多，已被永久锁定");
                    }
                    if (lockType == EUserLockType.Hours)
                    {
                        if (userInfo.LastActivityDate.HasValue)
                        {
                            var ts = new TimeSpan(DateTime.Now.Ticks - userInfo.LastActivityDate.Value.Ticks);
                            var hours = Convert.ToInt32(configInfo.AdminLockLoginHours - ts.TotalHours);
                            if (hours > 0)
                            {
                                return (false, userName, $"此账号错误登录次数过多，已被锁定，请等待 {hours} 小时后重试");
                            }
                        }
                    }
                }
            }

            if (CheckPassword(password, isPasswordMd5, userInfo.Password, PasswordFormat.Parse(userInfo.PasswordFormat), userInfo.PasswordSalt))
            {
                return (true, userName, null);
            }

            return (false, userName, "账号或密码错误");
        }

        public bool CheckPassword(string password, bool isPasswordMd5, string dbPassword, PasswordFormat passwordFormat, string passwordSalt)
        {
            var decodePassword = DecodePassword(dbPassword, passwordFormat, passwordSalt);
            if (isPasswordMd5)
            {
                return password == AuthUtils.Md5ByString(decodePassword);
            }
            return password == decodePassword;
        }

        private static string EncodePassword(string password, PasswordFormat passwordFormat, out string passwordSalt)
        {
            var retVal = string.Empty;
            passwordSalt = string.Empty;

            if (passwordFormat == PasswordFormat.Clear)
            {
                retVal = password;
            }
            else if (passwordFormat == PasswordFormat.Hashed)
            {
                passwordSalt = GenerateSalt();

                var src = Encoding.Unicode.GetBytes(password);
                var buffer2 = Convert.FromBase64String(passwordSalt);
                var dst = new byte[buffer2.Length + src.Length];
                Buffer.BlockCopy(buffer2, 0, dst, 0, buffer2.Length);
                Buffer.BlockCopy(src, 0, dst, buffer2.Length, src.Length);
                var algorithm = HashAlgorithm.Create("SHA1");
                if (algorithm == null) return retVal;
                var inArray = algorithm.ComputeHash(dst);

                retVal = Convert.ToBase64String(inArray);
            }
            else if (passwordFormat == PasswordFormat.Encrypted)
            {
                passwordSalt = GenerateSalt();

                var encrypt = new DesEncryptor
                {
                    InputString = password,
                    EncryptKey = passwordSalt
                };
                encrypt.DesEncrypt();

                retVal = encrypt.OutString;
            }
            return retVal;
        }

        private static string GenerateSalt()
        {
            var data = new byte[0x10];
            new RNGCryptoServiceProvider().GetBytes(data);
            return Convert.ToBase64String(data);
        }

        private static string DecodePassword(string password, PasswordFormat passwordFormat, string passwordSalt)
        {
            var retVal = string.Empty;
            if (passwordFormat == PasswordFormat.Clear)
            {
                retVal = password;
            }
            else if (passwordFormat == PasswordFormat.Hashed)
            {
                throw new Exception("can not decode hashed password");
            }
            else if (passwordFormat == PasswordFormat.Encrypted)
            {
                var encrypt = new DesEncryptor
                {
                    InputString = password,
                    DecryptKey = passwordSalt
                };
                encrypt.DesDecrypt();

                retVal = encrypt.OutString;
            }
            return retVal;
        }
    }
}
