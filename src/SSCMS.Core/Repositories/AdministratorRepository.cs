using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Datory;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Repositories
{
    public partial class AdministratorRepository : IAdministratorRepository
    {
        private readonly Repository<Administrator> _repository;
        private readonly IConfigRepository _configRepository;
        private readonly IAdministratorsInRolesRepository _administratorsInRolesRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IDbCacheRepository _dbCacheRepository;

        public AdministratorRepository(ISettingsManager settingsManager, IConfigRepository configRepository,
            IAdministratorsInRolesRepository administratorsInRolesRepository, IRoleRepository roleRepository,
            IDbCacheRepository dbCacheRepository)
        {
            _dbCacheRepository = dbCacheRepository;
            _repository = new Repository<Administrator>(settingsManager.Database, settingsManager.Redis);
            _configRepository = configRepository;
            _administratorsInRolesRepository = administratorsInRolesRepository;
            _roleRepository = roleRepository;
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task UpdateLastActivityDateAndCountOfFailedLoginAsync(Administrator administrator)
        {
            if (administrator == null) return;

            administrator.LastActivityDate = DateTime.Now;
            administrator.CountOfFailedLogin += 1;

            var cacheKeys = GetCacheKeys(administrator);

            await _repository.UpdateAsync(Q
                .Set(nameof(Administrator.LastActivityDate), administrator.LastActivityDate)
                .Set(nameof(Administrator.CountOfFailedLogin), administrator.CountOfFailedLogin)
                .Where(nameof(Administrator.Id), administrator.Id)
                .CachingRemove(cacheKeys.ToArray())
            );
        }

        public async Task UpdateLastActivityDateAsync(Administrator administrator)
        {
            if (administrator == null) return;

            administrator.LastActivityDate = DateTime.Now;

            var cacheKeys = GetCacheKeys(administrator);

            await _repository.UpdateAsync(Q
                .Set(nameof(Administrator.LastActivityDate), administrator.LastActivityDate)
                .Where(nameof(Administrator.Id), administrator.Id)
                .CachingRemove(cacheKeys.ToArray())
            );
        }

        public async Task UpdateLastActivityDateAndCountOfLoginAsync(Administrator administrator)
        {
            if (administrator == null) return;

            administrator.LastActivityDate = DateTime.Now;
            administrator.CountOfLogin += 1;
            administrator.CountOfFailedLogin = 0;

            var cacheKeys = GetCacheKeys(administrator);

            await _repository.UpdateAsync(Q
                .Set(nameof(Administrator.LastActivityDate), administrator.LastActivityDate)
                .Set(nameof(Administrator.CountOfLogin), administrator.CountOfLogin)
                .Set(nameof(Administrator.CountOfFailedLogin), administrator.CountOfFailedLogin)
                .Where(nameof(Administrator.Id), administrator.Id)
                .CachingRemove(cacheKeys.ToArray())
            );
        }

        public async Task UpdateSiteIdsAsync(Administrator administrator, List<int> siteIds)
        {
            if (administrator == null) return;

            administrator.SiteIds = siteIds;

            var cacheKeys = GetCacheKeys(administrator);

            await _repository.UpdateAsync(Q
                .Set(nameof(Administrator.SiteIds), ListUtils.ToString(administrator.SiteIds))
                .Where(nameof(Administrator.Id), administrator.Id)
                .CachingRemove(cacheKeys.ToArray())
            );
        }

        public async Task<List<int>> UpdateSiteIdAsync(Administrator administrator, int siteId)
        {
            if (administrator == null || siteId <= 0) return null;

            var siteIdsLatestAccessed = ListUtils.GetIntList(administrator.SiteIds);
            if (administrator.SiteId != siteId || siteIdsLatestAccessed.FirstOrDefault() != siteId)
            {
                siteIdsLatestAccessed.Remove(siteId);
                siteIdsLatestAccessed.Insert(0, siteId);

                administrator.SiteIds = siteIdsLatestAccessed.Distinct().ToList();
                administrator.SiteId = siteId;

                var cacheKeys = GetCacheKeys(administrator);

                await _repository.UpdateAsync(Q
                    .Set(nameof(Administrator.SiteIds), ListUtils.ToString(administrator.SiteIds))
                    .Set(nameof(Administrator.SiteId), administrator.SiteId)
                    .Where(nameof(Administrator.Id), administrator.Id)
                    .CachingRemove(cacheKeys.ToArray())
                );
            }

            return siteIdsLatestAccessed;
        }

        private async Task ChangePasswordAsync(Administrator administrator, PasswordFormat passwordFormat, string passwordSalt, string password)
        {
            administrator.LastChangePasswordDate = DateTime.Now;

            var cacheKeys = GetCacheKeys(administrator);

            await _repository.UpdateAsync(Q
                .Set(nameof(Administrator.Password), password)
                .Set(nameof(Administrator.PasswordFormat), passwordFormat.GetValue())
                .Set(nameof(Administrator.PasswordSalt), passwordSalt)
                .Set(nameof(Administrator.LastChangePasswordDate), administrator.LastChangePasswordDate)
                .Where(nameof(Administrator.Id), administrator.Id)
                .CachingRemove(cacheKeys.ToArray())
            );

            // 修改密码后吊销该管理员此前签发的所有 JWT（#3894）
            await TokenRevocationUtils.RevokeAsync(_dbCacheRepository, true, administrator.Id);
        }

        public async Task LockAsync(IList<string> userNames)
        {
            var cacheKeys = new List<string>();
            foreach (var userName in userNames)
            {
                var administrator = await GetByUserNameAsync(userName);
                if (administrator == null) continue;
                cacheKeys.AddRange(GetCacheKeys(administrator));
                // 锁定即吊销已签发 JWT（#3894）
                await TokenRevocationUtils.RevokeAsync(_dbCacheRepository, true, administrator.Id);
            }

            await _repository.UpdateAsync(Q
                .Set(nameof(Administrator.Locked), true)
                .WhereIn(nameof(Administrator.UserName), userNames)
                .CachingRemove(cacheKeys.ToArray())
            );
        }

        public async Task UnLockAsync(IList<string> userNames)
        {
            var cacheKeys = new List<string>();
            foreach (var userName in userNames)
            {
                var administrator = await GetByUserNameAsync(userName);
                cacheKeys.AddRange(GetCacheKeys(administrator));
            }

            await _repository.UpdateAsync(Q
                .Set(nameof(Administrator.Locked), false)
                .Set(nameof(Administrator.CountOfFailedLogin), 0)
                .WhereIn(nameof(Administrator.UserName), userNames)
                .CachingRemove(cacheKeys.ToArray())
            );
        }

        public async Task<int> GetCountAsync(string creatorUserName, string role, int lastActivityDate, string keyword)
        {
            var query = Q.NewQuery();
            if (!string.IsNullOrEmpty(creatorUserName))
            {
                query.Where(nameof(Administrator.CreatorUserName), creatorUserName);
            }
            if (lastActivityDate > 0)
            {
                var dateTime = DateTime.Now.AddDays(-lastActivityDate);
                query.Where(nameof(Administrator.LastActivityDate), ">=", DateUtils.ToString(dateTime));
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                var like = $"%{keyword}%";
                query.Where(q => q
                    .WhereLike(nameof(Administrator.UserName), like)
                    .OrWhereLike(nameof(Administrator.Mobile), like)
                    .OrWhereLike(nameof(Administrator.Email), like)
                    .OrWhereLike(nameof(Administrator.DisplayName), like)
                );
            }
            if (!string.IsNullOrEmpty(role))
            {
                var userNames = await _administratorsInRolesRepository.GetUsersInRoleAsync(role);
                if (userNames != null && userNames.Any())
                {
                    query.WhereIn(nameof(Administrator.UserName), userNames);
                }
            }

            return await _repository.CountAsync(query);
        }

        public async Task<List<Administrator>> GetAdministratorsAsync(string creatorUserName, string role, string order, int lastActivityDate, string keyword, int offset, int limit)
        {
            var query = Q.NewQuery();
            if (!string.IsNullOrEmpty(creatorUserName))
            {
                query.Where(nameof(Administrator.CreatorUserName), creatorUserName);
            }
            if (lastActivityDate > 0)
            {
                var dateTime = DateTime.Now.AddDays(-lastActivityDate);
                query.Where(nameof(Administrator.LastActivityDate), ">=", DateUtils.ToString(dateTime));
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                var like = $"%{keyword}%";
                query.Where(q => q
                    .WhereLike(nameof(Administrator.UserName), like)
                    .OrWhereLike(nameof(Administrator.Mobile), like)
                    .OrWhereLike(nameof(Administrator.Email), like)
                    .OrWhereLike(nameof(Administrator.DisplayName), like)
                );
            }
            if (!string.IsNullOrEmpty(role))
            {
                var userNames = await _administratorsInRolesRepository.GetUsersInRoleAsync(role);
                if (userNames == null || userNames.Count == 0)
                {
                    return new List<Administrator>();
                }

                query.WhereIn(nameof(Administrator.UserName), userNames);
            }

            if (!string.IsNullOrEmpty(order))
            {
                if (StringUtils.EqualsIgnoreCase(order, nameof(Administrator.UserName)))
                {
                    query.OrderBy(nameof(Administrator.UserName));
                }
                else
                {
                    query.OrderByDesc(order);
                }
            }
            else
            {
                query.OrderByDesc(nameof(Administrator.Id));
            }

            query.Offset(offset).Limit(limit);

            var dbs = await _repository.GetAllAsync(query);
            var list = new List<Administrator>();

            if (dbs != null)
            {
                foreach (var admin in dbs)
                {
                    if (admin != null)
                    {
                        list.Add(admin);
                    }
                }
            }

            return list;
        }

        public async Task<List<int>> GetAdministratorIdsAsync(string keyword)
        {
            var query = Q.Select(nameof(Administrator.Id));
            if (!string.IsNullOrEmpty(keyword))
            {
                var like = $"%{keyword}%";
                query.Where(q => q
                    .WhereLike(nameof(Administrator.UserName), like)
                    .OrWhereLike(nameof(Administrator.Mobile), like)
                    .OrWhereLike(nameof(Administrator.Email), like)
                    .OrWhereLike(nameof(Administrator.DisplayName), like)
                );
            }
            
            return await _repository.GetAllAsync<int>(query);
        }

        public async Task<bool> IsUserNameExistsAsync(string adminName)
        {
            return await _repository.ExistsAsync(Q.Where(nameof(Administrator.UserName), adminName));
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _repository.ExistsAsync(Q.Where(nameof(Administrator.Email), email));
        }

        public async Task<bool> IsMobileExistsAsync(string mobile)
        {
            return await _repository.ExistsAsync(Q.Where(nameof(Administrator.Mobile), mobile));
        }

        public async Task<List<string>> GetUserNamesAsync()
        {
            return await _repository.GetAllAsync<string>(Q
                .Select(nameof(Administrator.UserName))
            );
        }

        public async Task<List<int>> GetUserIdsAsync()
        {
            return await _repository.GetAllAsync<int>(Q
                .Select(nameof(Administrator.Id))
                .OrderByDesc(nameof(Administrator.Id))
            );
        }

        /// <summary>
        /// 新建/修改口令一律使用不可逆 PBKDF2（GJ20260924），不再生成可逆的 DES/SM4 口令。
        /// </summary>
        private static string EncodePassword(string plainPassword, out string passwordSalt)
        {
            return PasswordHashUtils.Hash(PasswordHashUtils.ToMd5(plainPassword), out passwordSalt);
        }

        private async Task<(bool IsValid, string ErrorMessage)> UpdateValidateAsync(Administrator adminEntityToUpdate, string userName, string email, string mobile)
        {
            if (adminEntityToUpdate.UserName != null && adminEntityToUpdate.UserName != userName)
            {
                if (string.IsNullOrEmpty(adminEntityToUpdate.UserName))
                {
                    return (false, "用户名不能为空");
                }
                var config = await _configRepository.GetAsync();
                if (adminEntityToUpdate.UserName.Length < config.AdminUserNameMinLength)
                {
                    return (false, $"用户名长度必须大于等于{config.AdminUserNameMinLength}");
                }
                if (await IsUserNameExistsAsync(adminEntityToUpdate.UserName))
                {
                    return (false, "用户名已存在，请更换用户名");
                }
            }

            if (adminEntityToUpdate.Mobile != null && adminEntityToUpdate.Mobile != mobile)
            {
                if (!string.IsNullOrEmpty(adminEntityToUpdate.Mobile) && await IsMobileExistsAsync(mobile))
                {
                    return (false, "手机号码已被注册，请更换手机号码");
                }
            }

            if (adminEntityToUpdate.Email != null && adminEntityToUpdate.Email != email)
            {
                if (!string.IsNullOrEmpty(adminEntityToUpdate.Email) && await IsEmailExistsAsync(email))
                {
                    return (false, "电子邮件地址已被注册，请更换邮箱");
                }
            }

            return (true, string.Empty);
        }

        public async Task<(bool IsValid, string ErrorMessage)> InsertValidateAsync(string userName, string password, string email, string mobile)
        {
            var config = await _configRepository.GetAsync();

            if (string.IsNullOrEmpty(userName))
            {
                return (false, "用户名不能为空");
            }
            if (userName.Length < config.AdminUserNameMinLength)
            {
                return (false, $"用户名长度必须大于等于{config.AdminUserNameMinLength}");
            }
            if (string.IsNullOrEmpty(password))
            {
                return (false, "密码不能为空");
            }
            if (password.Length < config.AdminPasswordMinLength)
            {
                return (false, $"密码长度必须大于等于{config.AdminPasswordMinLength}");
            }
            if (
                !PasswordRestrictionUtils.IsValid(password,
                    config.AdminPasswordRestriction))
            {
                return (false, $"密码不符合规则，请包含{config.AdminPasswordRestriction.GetDisplayName()}");
            }

            return (true, string.Empty);
        }

        public async Task<(bool IsValid, string ErrorMessage)> InsertAsync(Administrator administrator, string password)
        {
            var fieldValid = AccountValidateUtils.Validate(administrator.UserName, administrator.DisplayName, administrator.Email, administrator.Mobile, administrator.AvatarUrl);
            if (!fieldValid.IsValid) return fieldValid;

            var valid = await InsertValidateAsync(administrator.UserName, password, administrator.Email, administrator.Mobile);
            if (!valid.IsValid) return valid;

            if (await IsUserNameExistsAsync(administrator.UserName))
            {
                return (false, "用户名已存在，请更换用户名");
            }
            if (!string.IsNullOrEmpty(administrator.Email) && await IsEmailExistsAsync(administrator.Email))
            {
                return (false, "电子邮件地址已被注册，请更换邮箱");
            }
            if (!string.IsNullOrEmpty(administrator.Mobile) && await IsMobileExistsAsync(administrator.Mobile))
            {
                return (false, "手机号码已被注册，请更换手机号码");
            }

            try
            {
                administrator.LastActivityDate = DateTime.Now;
                administrator.LastChangePasswordDate = DateTime.Now;
                administrator.PasswordFormat = PasswordFormat.Pbkdf2;
                administrator.Password = EncodePassword(password, out var passwordSalt);
                administrator.PasswordSalt = passwordSalt;
                administrator.Set("ConfirmPassword", string.Empty);

                await _repository.InsertAsync(administrator);

                var roles = new[] { PredefinedRole.Administrator.GetValue() };
                await AddUserToRolesAsync(administrator.UserName, roles);

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                // 不向调用方暴露数据库异常详情（库名/表名/字段名），仅输出到服务端日志
                Console.Error.WriteLine($"Administrator insert failed: {ex}");
                return (false, "数据保存失败，请检查输入内容");
            }
        }

        public async Task<(bool IsValid, string ErrorMessage)> UpdateAsync(Administrator administrator)
        {
            var admin = await GetByUserIdAsync(administrator.Id);
            if (admin == null) return (false, "管理员不存在");
            var cacheKeys = GetCacheKeys(admin);

            // 仅对发生变化的字段做校验，避免历史数据格式不规范导致无法更新
            var fieldValid = AccountValidateUtils.Validate(
                administrator.UserName != admin.UserName ? administrator.UserName : null,
                administrator.DisplayName != admin.DisplayName ? administrator.DisplayName : null,
                administrator.Email != admin.Email ? administrator.Email : null,
                administrator.Mobile != admin.Mobile ? administrator.Mobile : null,
                administrator.AvatarUrl != admin.AvatarUrl ? administrator.AvatarUrl : null);
            if (!fieldValid.IsValid) return fieldValid;

            var valid = await UpdateValidateAsync(admin, administrator.UserName, administrator.Email, administrator.Mobile);
            if (!valid.IsValid) return valid;

            await _repository.UpdateAsync(Q
                .Set(nameof(Administrator.LastActivityDate), administrator.LastActivityDate)
                .Set(nameof(Administrator.LastChangePasswordDate), administrator.LastChangePasswordDate)
                .Set(nameof(Administrator.CountOfLogin), administrator.CountOfLogin)
                .Set(nameof(Administrator.CountOfFailedLogin), administrator.CountOfFailedLogin)
                .Set(nameof(Administrator.Locked), administrator.Locked)
                .Set(nameof(Administrator.SiteIds), ListUtils.ToString(administrator.SiteIds))
                .Set(nameof(Administrator.SiteId), administrator.SiteId)
                .Set(nameof(Administrator.DisplayName), administrator.DisplayName)
                .Set(nameof(Administrator.Mobile), administrator.Mobile)
                .Set(nameof(Administrator.MobileVerified), administrator.MobileVerified)
                .Set(nameof(Administrator.Email), administrator.Email)
                .Set(nameof(Administrator.AvatarUrl), administrator.AvatarUrl)
                .Set(nameof(Administrator.UserName), administrator.UserName)
                .Where(nameof(Administrator.Id), administrator.Id)
                .CachingRemove(cacheKeys.ToArray())
            );

            return (true, string.Empty);
        }

        public async Task<(bool IsValid, string ErrorMessage)> ChangePasswordAsync(Administrator adminEntity, string password)
        {
            var config = await _configRepository.GetAsync();

            if (string.IsNullOrEmpty(password))
            {
                return (false, "密码不能为空");
            }
            if (password.Length < config.AdminPasswordMinLength)
            {
                return (false, $"密码长度必须大于等于{config.AdminPasswordMinLength}");
            }
            if (
                !PasswordRestrictionUtils.IsValid(password, config.AdminPasswordRestriction))
            {
                return (false, $"密码不符合规则，请包含{config.AdminPasswordRestriction.GetDisplayName()}");
            }

            password = EncodePassword(password, out var passwordSalt);
            await ChangePasswordAsync(adminEntity, PasswordFormat.Pbkdf2, passwordSalt, password);
            return (true, string.Empty);
        }

        public async Task<(Administrator administrator, string userName, string errorMessage)> ValidateAsync(string account, string password, bool isPasswordMd5)
        {
            var userName = string.Empty;

            if (string.IsNullOrEmpty(account))
            {
                return (null, userName, "账号不能为空");
            }
            if (string.IsNullOrEmpty(password))
            {
                return (null, userName, "密码不能为空");
            }

            var administrator = await GetByAccountAsync(account);
            if (string.IsNullOrEmpty(administrator?.UserName))
            {
                return (null, userName, "帐号或密码错误");
            }

            userName = administrator.UserName;

            var (success, errorMessage) = await ValidateLockAsync(administrator);
            if (!success)
            {
                return (null, userName, errorMessage);
            }

            return await CheckPasswordByAdminIdAsync(administrator.Id, password, isPasswordMd5)
                ? (administrator, userName, string.Empty)
                : (null, userName, "账号或密码错误");
        }

        public async Task<(bool Success, string ErrorMessage)> ValidateLockAsync(Administrator administrator)
        {
            if (administrator.Locked)
            {
                return (false, "此账号被锁定，无法登录");
            }

            var config = await _configRepository.GetAsync();

            if (config.IsAdminLockLogin)
            {
                if (administrator.CountOfFailedLogin > 0 &&
                    administrator.CountOfFailedLogin >= config.AdminLockLoginCount)
                {
                    var lockType = config.AdminLockLoginType;
                    if (lockType == LockType.Forever)
                    {
                        return (false, "此账号错误登录次数过多，已被永久锁定");
                    }
                    if (lockType == LockType.Hours && administrator.LastActivityDate.HasValue)
                    {
                        var ts = new TimeSpan(DateTime.Now.Ticks - administrator.LastActivityDate.Value.Ticks);
                        var hours = Convert.ToInt32(config.AdminLockLoginHours - ts.TotalHours);
                        if (hours > 0)
                        {
                            return (false, $"此账号错误登录次数过多，已被锁定，请等待{hours}小时后重试");
                        }
                    }
                }
            }

            return (true, null);
        }

        /// <summary>
        /// 仅用于旧口令迁移：还原 Clear/Encrypted/SM4 明文，无法还原时返回 null。
        /// </summary>
        private static string TryDecodeLegacyPassword(string password, PasswordFormat passwordFormat, string passwordSalt)
        {
            try
            {
                string plain = null;
                if (passwordFormat == PasswordFormat.Clear)
                {
                    plain = password;
                }
                else if (passwordFormat == PasswordFormat.Encrypted && !string.IsNullOrEmpty(passwordSalt))
                {
                    plain = DesEncryptor.DecryptStringBySecretKey(password, passwordSalt);
                }
                else if (passwordFormat == PasswordFormat.SM4 && !string.IsNullOrEmpty(passwordSalt))
                {
                    plain = EncryptUtils.Decrypt(password, passwordSalt);
                }
                return string.IsNullOrEmpty(plain) ? null : plain;
            }
            catch
            {
                return null;
            }
        }

        private static string LegacySha1(string password, string passwordSalt)
        {
            try
            {
                var src = Encoding.Unicode.GetBytes(password);
                var salt = Convert.FromBase64String(passwordSalt);
                var dst = new byte[salt.Length + src.Length];
                Buffer.BlockCopy(salt, 0, dst, 0, salt.Length);
                Buffer.BlockCopy(src, 0, dst, salt.Length, src.Length);
                using var sha1 = SHA1.Create();
                return Convert.ToBase64String(sha1.ComputeHash(dst));
            }
            catch
            {
                return null;
            }
        }

        private async Task<bool> CheckPasswordByAdminIdAsync(int adminId, string password, bool isPasswordMd5)
        {
            var dbAdmin = await _repository.GetAsync<Administrator>(Q
                .Select(nameof(Administrator.Id))
                .Select(nameof(Administrator.Guid))
                .Select(nameof(Administrator.UserName))
                .Select(nameof(Administrator.Mobile))
                .Select(nameof(Administrator.Email))
                .Select(nameof(Administrator.Password))
                .Select(nameof(Administrator.PasswordFormat))
                .Select(nameof(Administrator.PasswordSalt))
                .Where(nameof(Administrator.Id), adminId)
            );

            if (dbAdmin == null || string.IsNullOrEmpty(dbAdmin.Password) || string.IsNullOrEmpty(password))
            {
                return false;
            }

            var inputMd5 = isPasswordMd5 ? password : PasswordHashUtils.ToMd5(password);

            if (dbAdmin.PasswordFormat == PasswordFormat.Pbkdf2)
            {
                return PasswordHashUtils.Verify(inputMd5, dbAdmin.Password, dbAdmin.PasswordSalt);
            }

            // 旧格式（Clear/Encrypted/SM4/Hashed）：校验通过后立即升级为 PBKDF2
            var plain = TryDecodeLegacyPassword(dbAdmin.Password, dbAdmin.PasswordFormat, dbAdmin.PasswordSalt);
            bool isValid;
            if (plain != null)
            {
                isValid = SecureCompareUtils.FixedTimeEquals(inputMd5, PasswordHashUtils.ToMd5(plain));
            }
            else if (dbAdmin.PasswordFormat == PasswordFormat.Hashed && !isPasswordMd5)
            {
                isValid = SecureCompareUtils.FixedTimeEquals(dbAdmin.Password, LegacySha1(password, dbAdmin.PasswordSalt));
            }
            else
            {
                isValid = false;
            }

            if (isValid)
            {
                await UpgradePasswordAsync(dbAdmin, inputMd5);
            }
            return isValid;
        }

        /// <summary>
        /// 只替换口令存储格式，不更新改密时间、不吊销令牌。
        /// </summary>
        private async Task UpgradePasswordAsync(Administrator administrator, string passwordMd5)
        {
            var hash = PasswordHashUtils.Hash(passwordMd5, out var salt);
            await _repository.UpdateAsync(Q
                .Set(nameof(Administrator.Password), hash)
                .Set(nameof(Administrator.PasswordFormat), PasswordFormat.Pbkdf2.GetValue())
                .Set(nameof(Administrator.PasswordSalt), salt)
                .Where(nameof(Administrator.Id), administrator.Id)
                .CachingRemove(GetCacheKeys(administrator).ToArray())
            );
        }

        /// <summary>
        /// 启动时把所有可还原的旧口令（Clear/Encrypted/SM4）批量转换为 PBKDF2，
        /// 转换后数据库中不再保留可逆口令。返回转换数量。
        /// </summary>
        public async Task<int> MigrateLegacyPasswordsAsync()
        {
            var admins = await _repository.GetAllAsync<Administrator>(Q
                .Select(nameof(Administrator.Id))
                .Select(nameof(Administrator.Guid))
                .Select(nameof(Administrator.UserName))
                .Select(nameof(Administrator.Mobile))
                .Select(nameof(Administrator.Email))
                .Select(nameof(Administrator.Password))
                .Select(nameof(Administrator.PasswordFormat))
                .Select(nameof(Administrator.PasswordSalt))
            );

            var count = 0;
            foreach (var admin in admins)
            {
                if (admin.PasswordFormat == PasswordFormat.Pbkdf2 || admin.PasswordFormat == PasswordFormat.Hashed) continue;
                if (string.IsNullOrEmpty(admin.Password)) continue;

                var plain = TryDecodeLegacyPassword(admin.Password, admin.PasswordFormat, admin.PasswordSalt);
                if (string.IsNullOrEmpty(plain)) continue;

                await UpgradePasswordAsync(admin, PasswordHashUtils.ToMd5(plain));
                count++;
            }
            return count;
        }

        public async Task<int> GetCountAsync()
        {
            return await _repository.CountAsync();
        }

        public async Task<List<Administrator>> GetAdministratorsAsync(int offset, int limit)
        {
            var list = new List<Administrator>();

            var dbList = await _repository.GetAllAsync(Q.Offset(offset).Limit(limit).OrderBy(nameof(Administrator.Id)));

            if (dbList != null)
            {
                foreach (var dbEntity in dbList)
                {
                    if (dbEntity != null)
                    {
                        list.Add(dbEntity);
                    }
                }
            }

            return list;
        }

        public async Task<bool> IsExistsAsync(int id)
        {
            return await _repository.ExistsAsync(id);
        }

        public async Task<Administrator> DeleteAsync(int id)
        {
            var admin = await GetByUserIdAsync(id);
            var cacheKeys = GetCacheKeys(admin);

            await _repository.DeleteAsync(id, Q.CachingRemove(cacheKeys.ToArray()));

            return admin;
        }
    }
}
