using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Datory;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.CMS.Provider
{
    public class AdministratorDao : IRepository
    {
        private readonly Repository<Administrator> _repository;

        public AdministratorDao()
        {
            _repository = new Repository<Administrator>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString));
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        private class Attr
        {
            public static string SiteIdCollection = nameof(SiteIdCollection);
        }

        public async Task UpdateLastActivityDateAndCountOfFailedLoginAsync(Administrator administrator)
        {
            if (administrator == null) return;

            administrator.LastActivityDate = DateTime.Now;
            administrator.CountOfFailedLogin += 1;

            await _repository.UpdateAsync(Q
                .Set(nameof(Administrator.LastActivityDate), administrator.LastActivityDate)
                .Set(nameof(Administrator.CountOfFailedLogin), administrator.CountOfFailedLogin)
                .Where(nameof(Administrator.Id), administrator.Id)
            );

            AdminManager.UpdateCache(administrator);
        }

        public async Task UpdateLastActivityDateAsync(Administrator administrator)
        {
            if (administrator == null) return;

            administrator.LastActivityDate = DateTime.Now;

            await _repository.UpdateAsync(Q
                .Set(nameof(Administrator.LastActivityDate), administrator.LastActivityDate)
                .Where(nameof(Administrator.Id), administrator.Id)
            );

            AdminManager.UpdateCache(administrator);
        }

        public async Task UpdateLastActivityDateAndCountOfLoginAsync(Administrator administrator)
        {
            if (administrator == null) return;

            administrator.LastActivityDate = DateTime.Now;
            administrator.CountOfLogin += 1;
            administrator.CountOfFailedLogin = 0;

            await _repository.UpdateAsync(Q
                .Set(nameof(Administrator.LastActivityDate), administrator.LastActivityDate)
                .Set(nameof(Administrator.CountOfLogin), administrator.CountOfLogin)
                .Set(nameof(Administrator.CountOfFailedLogin), administrator.CountOfFailedLogin)
                .Where(nameof(Administrator.Id), administrator.Id)
            );

            AdminManager.UpdateCache(administrator);
        }

        public async Task UpdateSiteIdCollectionAsync(Administrator administrator, List<int> siteIds)
        {
            if (administrator == null) return;

            administrator.SiteIds = siteIds;

            await _repository.UpdateAsync(Q
                .Set(Attr.SiteIdCollection, string.Join(",", administrator.SiteIds))
                .Where(nameof(Administrator.Id), administrator.Id)
            );

            AdminManager.UpdateCache(administrator);
        }

        public async Task<List<int>> UpdateSiteIdAsync(Administrator administrator, int siteId)
        {
            if (administrator == null) return null;

            var siteIdListLatestAccessed = administrator.SiteIds;
            if (administrator.SiteId != siteId || siteIdListLatestAccessed.FirstOrDefault() != siteId)
            {
                siteIdListLatestAccessed.Remove(siteId);
                siteIdListLatestAccessed.Insert(0, siteId);

                administrator.SiteIds = siteIdListLatestAccessed;
                administrator.SiteId = siteId;

                await _repository.UpdateAsync(Q
                    .Set(Attr.SiteIdCollection, string.Join(",", administrator.SiteIds))
                    .Set(nameof(Administrator.SiteId), administrator.SiteId)
                    .Where(nameof(Administrator.Id), administrator.Id)
                );

                AdminManager.UpdateCache(administrator);
            }

            return siteIdListLatestAccessed;
        }

        private async Task ChangePasswordAsync(Administrator administrator, EPasswordFormat passwordFormat, string passwordSalt, string password)
        {
            administrator.LastChangePasswordDate = DateTime.Now;

            await _repository.UpdateAsync(Q
                .Set(nameof(Administrator.Password), password)
                .Set(nameof(Administrator.PasswordFormat), EPasswordFormatUtils.GetValue(passwordFormat))
                .Set(nameof(Administrator.PasswordSalt), passwordSalt)
                .Set(nameof(Administrator.LastChangePasswordDate), administrator.LastChangePasswordDate)
                .Where(nameof(Administrator.Id), administrator.Id)
            );

            AdminManager.RemoveCache(administrator);
        }

        public async Task DeleteAsync(Administrator administrator)
        {
            if (administrator == null) return;

            await _repository.DeleteAsync(administrator.Id);

            AdminManager.RemoveCache(administrator);
        }

        public async Task LockAsync(IEnumerable<string> userNameList)
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(Administrator.IsLockedOut), true.ToString())
                .WhereIn(nameof(Administrator.UserName), userNameList)
            );

            AdminManager.ClearCache();
        }

        public async Task UnLockAsync(IEnumerable<string> userNameList)
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(Administrator.IsLockedOut), false.ToString())
                .Set(nameof(Administrator.CountOfFailedLogin), 0)
                .WhereIn(nameof(Administrator.UserName), userNameList)
            );

            AdminManager.ClearCache();
        }

        public async Task<Administrator> GetByIdAsync(int id)
        {
            var admin = await _repository.GetAsync(id);
            return admin;
        }

        private async Task<Administrator> GetByAccountAsync(string account)
        {
            var administratorEntity = await GetByUserNameAsync(account);
            if (administratorEntity != null) return administratorEntity;
            if (StringUtils.IsMobile(account)) return await GetByMobileAsync(account);
            if (StringUtils.IsEmail(account)) return await GetByEmailAsync(account);

            return null;
        }

        public async Task<Administrator> GetByUserIdAsync(int userId)
        {
            var admin = await _repository.GetAsync(userId);
            return admin;
        }

        public async Task<Administrator> GetByUserNameAsync(string userName)
        {
            var admin = await _repository.GetAsync(Q.Where(nameof(Administrator.UserName), userName));
            return admin;
        }

        public async Task<Administrator> GetByMobileAsync(string mobile)
        {
            var admin = await _repository.GetAsync(Q.Where(nameof(Administrator.Mobile), mobile));
            return admin;
        }

        public async Task<Administrator> GetByEmailAsync(string email)
        {
            var admin = await _repository.GetAsync(Q.Where(nameof(Administrator.Email), email));
            return admin;
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
                query.WhereDate(nameof(Administrator.LastActivityDate), ">=", dateTime);
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
                var userNameList = await DataProvider.AdministratorsInRolesDao.GetUsersInRoleAsync(role);
                if (userNameList != null && userNameList.Any())
                {
                    query.WhereIn(nameof(Administrator.UserName), userNameList);
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
                query.WhereDate(nameof(Administrator.LastActivityDate), ">=", dateTime);
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
                var userNameList = await DataProvider.AdministratorsInRolesDao.GetUsersInRoleAsync(role);
                if (userNameList != null && userNameList.Any())
                {
                    query.WhereIn(nameof(Administrator.UserName), userNameList);
                }
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

            var dbList = await _repository.GetAllAsync(query);
            var list = new List<Administrator>();

            if (dbList != null)
            {
                foreach (var admin in dbList)
                {
                    if (admin != null)
                    {
                        list.Add(admin);
                    }
                }
            }

            return list;
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

        public async Task<IEnumerable<string>> GetUserNameListAsync()
        {
            return await _repository.GetAllAsync<string>(Q
                .Select(nameof(Administrator.UserName))
            );
        }

        public async Task<IEnumerable<int>> GetUserIdListAsync()
        {
            return await _repository.GetAllAsync<int>(Q
                .Select(nameof(Administrator.Id))
                .OrderByDesc(nameof(Administrator.Id))
            );
        }

        private string EncodePassword(string password, EPasswordFormat passwordFormat, out string passwordSalt)
        {
            var retVal = string.Empty;
            passwordSalt = string.Empty;

            if (passwordFormat == EPasswordFormat.Clear)
            {
                retVal = password;
            }
            else if (passwordFormat == EPasswordFormat.Hashed)
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
            else if (passwordFormat == EPasswordFormat.Encrypted)
            {
                passwordSalt = GenerateSalt();

                var des = new DesEncryptor
                {
                    InputString = password,
                    EncryptKey = passwordSalt
                };
                des.DesEncrypt();

                retVal = des.OutString;
            }
            return retVal;
        }

        private static string GenerateSalt()
        {
            var data = new byte[0x10];
            new RNGCryptoServiceProvider().GetBytes(data);
            return Convert.ToBase64String(data);
        }

        private async Task<(bool IsValid, string ErrorMessage)> UpdateValidateAsync(Administrator adminEntityToUpdate, string userName, string email, string mobile)
        {
            if (adminEntityToUpdate.UserName != null && adminEntityToUpdate.UserName != userName)
            {
                if (string.IsNullOrEmpty(adminEntityToUpdate.UserName))
                {
                    return (false, "用户名不能为空");
                }
                var config = await DataProvider.ConfigDao.GetAsync();
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
                if (!string.IsNullOrEmpty(adminEntityToUpdate.Mobile) && await IsMobileExistsAsync(adminEntityToUpdate.Mobile))
                {
                    return (false, "手机号码已被注册，请更换手机号码");
                }
            }

            if (adminEntityToUpdate.Email != null && adminEntityToUpdate.Email != email)
            {
                if (!string.IsNullOrEmpty(adminEntityToUpdate.Email) && await IsEmailExistsAsync(adminEntityToUpdate.Email))
                {
                    return (false, "电子邮件地址已被注册，请更换邮箱");
                }
            }

            return (true, string.Empty);
        }

        private async Task<(bool IsValid, string ErrorMessage)> InsertValidateAsync(string userName, string password, string email, string mobile)
        {
            var config = await DataProvider.ConfigDao.GetAsync();

            if (string.IsNullOrEmpty(userName))
            {
                return (false, "用户名不能为空");
            }
            if (userName.Length < config.AdminUserNameMinLength)
            {
                return (false, $"用户名长度必须大于等于{config.AdminUserNameMinLength}");
            }
            if (await IsUserNameExistsAsync(userName))
            {
                return (false, "用户名已存在，请更换用户名");
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
                !EUserPasswordRestrictionUtils.IsValid(password,
                    config.AdminPasswordRestriction))
            {
                return (false, $"密码不符合规则，请包含{EUserPasswordRestrictionUtils.GetText(EUserPasswordRestrictionUtils.GetEnumType(config.AdminPasswordRestriction))}");
            }

            if (!string.IsNullOrEmpty(mobile) && await IsMobileExistsAsync(mobile))
            {
                return (false, "手机号码已被注册，请更换手机号码");
            }
            if (!string.IsNullOrEmpty(email) && await IsEmailExistsAsync(email))
            {
                return (false, "电子邮件地址已被注册，请更换邮箱");
            }

            return (true, string.Empty);
        }

        public async Task<(bool IsValid, string ErrorMessage)> InsertAsync(Administrator administrator, string password)
        {
            var valid = await InsertValidateAsync(administrator.UserName, password, administrator.Email, administrator.Mobile);
            if (!valid.IsValid) return valid;

            try
            {
                administrator.CreationDate = DateTime.Now;
                administrator.LastActivityDate = DateTime.Now;
                administrator.LastChangePasswordDate = DateTime.Now;
                administrator.PasswordFormat = EPasswordFormatUtils.GetValue(EPasswordFormat.Encrypted);
                administrator.Password = EncodePassword(password, EPasswordFormatUtils.GetEnumType(administrator.PasswordFormat), out var passwordSalt);
                administrator.PasswordSalt = passwordSalt;

                await _repository.InsertAsync(administrator);

                var roles = new[] { EPredefinedRoleUtils.GetValue(EPredefinedRole.Administrator) };
                await DataProvider.AdministratorsInRolesDao.AddUserToRolesAsync(administrator.UserName, roles);

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool IsValid, string ErrorMessage)> UpdateAsync(Administrator administrator)
        {
            var admin = await GetByIdAsync(administrator.Id);
            var valid = await UpdateValidateAsync(admin, administrator.UserName, administrator.Email, administrator.Mobile);
            if (!valid.IsValid) return valid;

            await _repository.UpdateAsync(Q
                .Set(nameof(Administrator.LastActivityDate), administrator.LastActivityDate)
                .Set(nameof(Administrator.LastChangePasswordDate), administrator.LastChangePasswordDate)
                .Set(nameof(Administrator.CountOfLogin), administrator.CountOfLogin)
                .Set(nameof(Administrator.CountOfFailedLogin), administrator.CountOfFailedLogin)
                .Set(nameof(Administrator.IsLockedOut), administrator.Locked.ToString())
                .Set(Attr.SiteIdCollection, string.Join(",", administrator.SiteIds))
                .Set(nameof(Administrator.SiteId), administrator.SiteId)
                .Set(nameof(Administrator.DisplayName), administrator.DisplayName)
                .Set(nameof(Administrator.Mobile), administrator.Mobile)
                .Set(nameof(Administrator.Email), administrator.Email)
                .Set(nameof(Administrator.AvatarUrl), administrator.AvatarUrl)
                .Set(nameof(Administrator.UserName), administrator.UserName)
                .Where(nameof(Administrator.Id), administrator.Id)
            );

            AdminManager.UpdateCache(administrator);

            return (true, string.Empty);
        }

        public async Task<(bool IsValid, string ErrorMessage)> ChangePasswordAsync(Administrator adminEntity, string password)
        {
            var config = await DataProvider.ConfigDao.GetAsync();

            if (string.IsNullOrEmpty(password))
            {
                return (false, "密码不能为空");
            }
            if (password.Length < config.AdminPasswordMinLength)
            {
                return (false, $"密码长度必须大于等于{config.AdminPasswordMinLength}");
            }
            if (
                !EUserPasswordRestrictionUtils.IsValid(password, config.AdminPasswordRestriction))
            {
                return (false, $"密码不符合规则，请包含{EUserPasswordRestrictionUtils.GetText(EUserPasswordRestrictionUtils.GetEnumType(config.AdminPasswordRestriction))}");
            }

            password = EncodePassword(password, EPasswordFormat.Encrypted, out var passwordSalt);
            await ChangePasswordAsync(adminEntity, EPasswordFormat.Encrypted, passwordSalt, password);
            return (true, string.Empty);
        }

        public async Task<(bool IsValid, string UserName, string ErrorMessage)> ValidateAsync(string account, string password, bool isPasswordMd5)
        {
            var userName = string.Empty;

            if (string.IsNullOrEmpty(account))
            {
                return (false, userName, "账号不能为空");
            }
            if (string.IsNullOrEmpty(password))
            {
                return (false, userName, "密码不能为空");
            }

            var administrator = await GetByAccountAsync(account);
            if (string.IsNullOrEmpty(administrator?.UserName))
            {
                return (false, userName, "帐号或密码错误");
            }

            userName = administrator.UserName;

            if (administrator.Locked)
            {
                return (false, userName, "此账号被锁定，无法登录");
            }

            var config = await DataProvider.ConfigDao.GetAsync();

            if (config.IsAdminLockLogin)
            {
                if (administrator.CountOfFailedLogin > 0 &&
                    administrator.CountOfFailedLogin >= config.AdminLockLoginCount)
                {
                    var lockType = EUserLockTypeUtils.GetEnumType(config.AdminLockLoginType);
                    if (lockType == EUserLockType.Forever)
                    {
                        return (false, userName, "此账号错误登录次数过多，已被永久锁定");
                    }
                    if (lockType == EUserLockType.Hours && administrator.LastActivityDate.HasValue)
                    {
                        var ts = new TimeSpan(DateTime.Now.Ticks - administrator.LastActivityDate.Value.Ticks);
                        var hours = Convert.ToInt32(config.AdminLockLoginHours - ts.TotalHours);
                        if (hours > 0)
                        {
                            return (false, userName, $"此账号错误登录次数过多，已被锁定，请等待{hours}小时后重试");
                        }
                    }
                }
            }

            var adminEntity = await _repository.GetAsync(administrator.Id);

            return CheckPassword(password, isPasswordMd5, adminEntity.Password,
                EPasswordFormatUtils.GetEnumType(adminEntity.PasswordFormat), adminEntity.PasswordSalt)
                ? (true, userName, string.Empty)
                : (false, userName, "账号或密码错误");
        }

        private static string DecodePassword(string password, EPasswordFormat passwordFormat, string passwordSalt)
        {
            var retVal = string.Empty;
            if (passwordFormat == EPasswordFormat.Clear)
            {
                retVal = password;
            }
            else if (passwordFormat == EPasswordFormat.Hashed)
            {
                throw new Exception("can not decode hashed password");
            }
            else if (passwordFormat == EPasswordFormat.Encrypted)
            {
                var des = new DesEncryptor
                {
                    InputString = password,
                    DecryptKey = passwordSalt
                };
                des.DesDecrypt();

                retVal = des.OutString;
            }
            return retVal;
        }

        private static bool CheckPassword(string password, bool isPasswordMd5, string dbPassword, EPasswordFormat passwordFormat,
            string passwordSalt)
        {
            var decodePassword = DecodePassword(dbPassword, passwordFormat, passwordSalt);
            if (isPasswordMd5)
            {
                return password == AuthUtils.Md5ByString(decodePassword);
            }
            return password == decodePassword;
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
            var adminEntityToDelete = await GetByIdAsync(id);

            await _repository.DeleteAsync(id);

            return adminEntityToDelete;
        }
    }
}
