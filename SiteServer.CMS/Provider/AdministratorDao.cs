using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Datory;
using SiteServer.CMS.Core;
using SiteServer.CMS.Data;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Db;
using SiteServer.CMS.Model.Mappings;
using SiteServer.Utils.Auth;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Provider
{
    public class AdministratorDao : DataProviderBase
    {
        private readonly Repository<AdministratorInfo> _repository;

        public AdministratorDao()
        {
            _repository = new Repository<AdministratorInfo>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString));
        }

        public override string TableName => _repository.TableName;
        public override List<TableColumn> TableColumns => _repository.TableColumns;

        private static Administrator ToDto(AdministratorInfo adminInfo)
        {
            return MapperManager.MapTo<Administrator>(adminInfo);
        }

        private static AdministratorInfo ToDb(Administrator administrator)
        {
            return MapperManager.MapTo<AdministratorInfo>(administrator);
        }

        public async Task UpdateLastActivityDateAndCountOfFailedLoginAsync(Administrator administrator)
        {
            if (administrator == null) return;

            administrator.LastActivityDate = DateTime.Now;
            administrator.CountOfFailedLogin += 1;

            await _repository.UpdateAsync(Q
                .Set(nameof(AdministratorInfo.LastActivityDate), administrator.LastActivityDate)
                .Set(nameof(AdministratorInfo.CountOfFailedLogin), administrator.CountOfFailedLogin)
                .Where(nameof(AdministratorInfo.Id), administrator.Id)
            );

            AdminManager.UpdateCache(administrator);
        }

        public async Task UpdateLastActivityDateAsync(Administrator administrator)
        {
            if (administrator == null) return;

            administrator.LastActivityDate = DateTime.Now;

            await _repository.UpdateAsync(Q
                .Set(nameof(AdministratorInfo.LastActivityDate), administrator.LastActivityDate)
                .Where(nameof(AdministratorInfo.Id), administrator.Id)
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
                .Set(nameof(AdministratorInfo.LastActivityDate), administrator.LastActivityDate)
                .Set(nameof(AdministratorInfo.CountOfLogin), administrator.CountOfLogin)
                .Set(nameof(AdministratorInfo.CountOfFailedLogin), administrator.CountOfFailedLogin)
                .Where(nameof(AdministratorInfo.Id), administrator.Id)
            );

            AdminManager.UpdateCache(administrator);
        }

        public async Task UpdateSiteIdCollectionAsync(Administrator administrator, string siteIdCollection)
        {
            if (administrator == null) return;

            administrator.SiteIdCollection = siteIdCollection;

            await _repository.UpdateAsync(Q
                .Set(nameof(AdministratorInfo.SiteIdCollection), administrator.SiteIdCollection)
                .Where(nameof(AdministratorInfo.Id), administrator.Id)
            );

            AdminManager.UpdateCache(administrator);
        }

        public async Task<List<int>> UpdateSiteIdAsync(Administrator administrator, int siteId)
        {
            if (administrator == null) return null;

            var siteIdListLatestAccessed = TranslateUtils.StringCollectionToIntList(administrator.SiteIdCollection);
            if (administrator.SiteId != siteId || siteIdListLatestAccessed.FirstOrDefault() != siteId)
            {
                siteIdListLatestAccessed.Remove(siteId);
                siteIdListLatestAccessed.Insert(0, siteId);

                administrator.SiteIdCollection = TranslateUtils.ObjectCollectionToString(siteIdListLatestAccessed);
                administrator.SiteId = siteId;

                await _repository.UpdateAsync(Q
                    .Set(nameof(AdministratorInfo.SiteIdCollection), administrator.SiteIdCollection)
                    .Set(nameof(AdministratorInfo.SiteId), administrator.SiteId)
                    .Where(nameof(AdministratorInfo.Id), administrator.Id)
                );

                AdminManager.UpdateCache(administrator);
            }

            return siteIdListLatestAccessed;
        }

        private async Task ChangePasswordAsync(Administrator administrator, EPasswordFormat passwordFormat, string passwordSalt, string password)
        {
            administrator.LastChangePasswordDate = DateTime.Now;

            await _repository.UpdateAsync(Q
                .Set(nameof(AdministratorInfo.Password), password)
                .Set(nameof(AdministratorInfo.PasswordFormat), EPasswordFormatUtils.GetValue(passwordFormat))
                .Set(nameof(AdministratorInfo.PasswordSalt), passwordSalt)
                .Set(nameof(AdministratorInfo.LastChangePasswordDate), administrator.LastChangePasswordDate)
                .Where(nameof(AdministratorInfo.Id), administrator.Id)
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
                .Set(nameof(AdministratorInfo.IsLockedOut), true.ToString())
                .WhereIn(nameof(AdministratorInfo.UserName), userNameList)
            );

            AdminManager.ClearCache();
        }

        public async Task UnLockAsync(IEnumerable<string> userNameList)
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(AdministratorInfo.IsLockedOut), false.ToString())
                .Set(nameof(AdministratorInfo.CountOfFailedLogin), 0)
                .WhereIn(nameof(AdministratorInfo.UserName), userNameList)
            );

            AdminManager.ClearCache();
        }

        public async Task<Administrator> GetByIdAsync(int id)
        {
            var adminInfo = await _repository.GetAsync(id);
            return ToDto(adminInfo);
        }

        private async Task<Administrator> GetByAccountAsync(string account)
        {
            var administratorInfo = await GetByUserNameAsync(account);
            if (administratorInfo != null) return administratorInfo;
            if (StringUtils.IsMobile(account)) return await GetByMobileAsync(account);
            if (StringUtils.IsEmail(account)) return await GetByEmailAsync(account);

            return null;
        }

        public async Task<Administrator> GetByUserIdAsync(int userId)
        {
            var adminInfo = await _repository.GetAsync(userId);
            return ToDto(adminInfo);
        }

        public async Task<Administrator> GetByUserNameAsync(string userName)
        {
            var adminInfo = await _repository.GetAsync(Q.Where(nameof(AdministratorInfo.UserName), userName));
            return ToDto(adminInfo);
        }

        public async Task<Administrator> GetByMobileAsync(string mobile)
        {
            var adminInfo = await _repository.GetAsync(Q.Where(nameof(AdministratorInfo.Mobile), mobile));
            return ToDto(adminInfo);
        }

        public async Task<Administrator> GetByEmailAsync(string email)
        {
            var adminInfo = await _repository.GetAsync(Q.Where(nameof(AdministratorInfo.Email), email));
            return ToDto(adminInfo);
        }

        public async Task<int> GetCountAsync(string creatorUserName, string role, int lastActivityDate, string keyword)
        {
            var query = Q.NewQuery();
            if (!string.IsNullOrEmpty(creatorUserName))
            {
                query.Where(nameof(AdministratorInfo.CreatorUserName), creatorUserName);
            }
            if (lastActivityDate > 0)
            {
                var dateTime = DateTime.Now.AddDays(-lastActivityDate);
                query.WhereDate(nameof(AdministratorInfo.LastActivityDate), ">=", dateTime);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                var like = $"%{keyword}%";
                query.Where(q => q
                    .WhereLike(nameof(AdministratorInfo.UserName), like)
                    .OrWhereLike(nameof(AdministratorInfo.Mobile), like)
                    .OrWhereLike(nameof(AdministratorInfo.Email), like)
                    .OrWhereLike(nameof(AdministratorInfo.DisplayName), like)
                );
            }
            if (!string.IsNullOrEmpty(role))
            {
                var userNameList = DataProvider.AdministratorsInRolesDao.GetUsersInRole(role);
                if (userNameList != null && userNameList.Length > 0)
                {
                    query.WhereIn(nameof(AdministratorInfo.UserName), userNameList);
                }
            }

            return await _repository.CountAsync(query);
        }

        public async Task<List<Administrator>> GetAdministratorsAsync(string creatorUserName, string role, string order, int lastActivityDate, string keyword, int offset, int limit)
        {
            var query = Q.NewQuery();
            if (!string.IsNullOrEmpty(creatorUserName))
            {
                query.Where(nameof(AdministratorInfo.CreatorUserName), creatorUserName);
            }
            if (lastActivityDate > 0)
            {
                var dateTime = DateTime.Now.AddDays(-lastActivityDate);
                query.WhereDate(nameof(AdministratorInfo.LastActivityDate), ">=", dateTime);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                var like = $"%{keyword}%";
                query.Where(q => q
                    .WhereLike(nameof(AdministratorInfo.UserName), like)
                    .OrWhereLike(nameof(AdministratorInfo.Mobile), like)
                    .OrWhereLike(nameof(AdministratorInfo.Email), like)
                    .OrWhereLike(nameof(AdministratorInfo.DisplayName), like)
                );
            }
            if (!string.IsNullOrEmpty(role))
            {
                var userNameList = DataProvider.AdministratorsInRolesDao.GetUsersInRole(role);
                if (userNameList != null && userNameList.Length > 0)
                {
                    query.WhereIn(nameof(AdministratorInfo.UserName), userNameList);
                }
            }

            if (!string.IsNullOrEmpty(order))
            {
                if (StringUtils.EqualsIgnoreCase(order, nameof(AdministratorInfo.UserName)))
                {
                    query.OrderBy(nameof(AdministratorInfo.UserName));
                }
                else
                {
                    query.OrderByDesc(order);
                }
            }
            else
            {
                query.OrderByDesc(nameof(AdministratorInfo.Id));
            }

            query.Offset(offset).Limit(limit);

            var dbList = await _repository.GetAllAsync(query);
            var list = new List<Administrator>();

            if (dbList != null)
            {
                foreach (var dbInfo in dbList)
                {
                    if (dbInfo != null)
                    {
                        list.Add(ToDto(dbInfo));
                    }
                }
            }

            return list;
        }

        public async Task<bool> IsUserNameExistsAsync(string adminName)
        {
            return await _repository.ExistsAsync(Q.Where(nameof(AdministratorInfo.UserName), adminName));
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _repository.ExistsAsync(Q.Where(nameof(AdministratorInfo.Email), email));
        }

        public async Task<bool> IsMobileExistsAsync(string mobile)
        {
            return await _repository.ExistsAsync(Q.Where(nameof(AdministratorInfo.Mobile), mobile));
        }

        public async Task<IEnumerable<string>> GetUserNameListAsync()
        {
            return await _repository.GetAllAsync<string>(Q
                .Select(nameof(AdministratorInfo.UserName))
            );
        }

        public async Task<IEnumerable<int>> GetUserIdListAsync()
        {
            return await _repository.GetAllAsync<int>(Q
                .Select(nameof(AdministratorInfo.Id))
                .OrderByDesc(nameof(AdministratorInfo.Id))
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

        private async Task<(bool IsValid, string ErrorMessage)> UpdateValidateAsync(Administrator adminInfoToUpdate, string userName, string email, string mobile)
        {
            if (adminInfoToUpdate.UserName != null && adminInfoToUpdate.UserName != userName)
            {
                if (string.IsNullOrEmpty(adminInfoToUpdate.UserName))
                {
                    return (false, "用户名不能为空");
                }
                if (adminInfoToUpdate.UserName.Length < ConfigManager.SystemConfigInfo.AdminUserNameMinLength)
                {
                    return (false, $"用户名长度必须大于等于{ConfigManager.SystemConfigInfo.AdminUserNameMinLength}");
                }
                if (await IsUserNameExistsAsync(adminInfoToUpdate.UserName))
                {
                    return (false, "用户名已存在，请更换用户名");
                }
            }

            if (adminInfoToUpdate.Mobile != null && adminInfoToUpdate.Mobile != mobile)
            {
                if (!string.IsNullOrEmpty(adminInfoToUpdate.Mobile) && await IsMobileExistsAsync(adminInfoToUpdate.Mobile))
                {
                    return (false, "手机号码已被注册，请更换手机号码");
                }
            }

            if (adminInfoToUpdate.Email != null && adminInfoToUpdate.Email != email)
            {
                if (!string.IsNullOrEmpty(adminInfoToUpdate.Email) && await IsEmailExistsAsync(adminInfoToUpdate.Email))
                {
                    return (false, "电子邮件地址已被注册，请更换邮箱");
                }
            }

            return (true, string.Empty);
        }

        private async Task<(bool IsValid, string ErrorMessage)> InsertValidateAsync(string userName, string password, string email, string mobile)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return (false, "用户名不能为空");
            }
            if (userName.Length < ConfigManager.SystemConfigInfo.AdminUserNameMinLength)
            {
                return (false, $"用户名长度必须大于等于{ConfigManager.SystemConfigInfo.AdminUserNameMinLength}");
            }
            if (await IsUserNameExistsAsync(userName))
            {
                return (false, "用户名已存在，请更换用户名");
            }

            if (string.IsNullOrEmpty(password))
            {
                return (false, "密码不能为空");
            }
            if (password.Length < ConfigManager.SystemConfigInfo.AdminPasswordMinLength)
            {
                return (false, $"密码长度必须大于等于{ConfigManager.SystemConfigInfo.AdminPasswordMinLength}");
            }
            if (
                !EUserPasswordRestrictionUtils.IsValid(password,
                    ConfigManager.SystemConfigInfo.AdminPasswordRestriction))
            {
                return (false, $"密码不符合规则，请包含{EUserPasswordRestrictionUtils.GetText(EUserPasswordRestrictionUtils.GetEnumType(ConfigManager.SystemConfigInfo.AdminPasswordRestriction))}");
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
                var adminInfo = ToDb(administrator);
                adminInfo.CreationDate = DateTime.Now;
                adminInfo.LastActivityDate = DateTime.Now;
                adminInfo.LastChangePasswordDate = DateTime.Now;
                adminInfo.PasswordFormat = EPasswordFormatUtils.GetValue(EPasswordFormat.Encrypted);
                adminInfo.Password = EncodePassword(password, EPasswordFormatUtils.GetEnumType(adminInfo.PasswordFormat), out var passwordSalt);
                adminInfo.PasswordSalt = passwordSalt;

                await _repository.InsertAsync(adminInfo);

                var roles = new[] { EPredefinedRoleUtils.GetValue(EPredefinedRole.Administrator) };
                await DataProvider.AdministratorsInRolesDao.AddUserToRolesAsync(adminInfo.UserName, roles);

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
                .Set(nameof(AdministratorInfo.LastActivityDate), administrator.LastActivityDate)
                .Set(nameof(AdministratorInfo.LastChangePasswordDate), administrator.LastChangePasswordDate)
                .Set(nameof(AdministratorInfo.CountOfLogin), administrator.CountOfLogin)
                .Set(nameof(AdministratorInfo.CountOfFailedLogin), administrator.CountOfFailedLogin)
                .Set(nameof(AdministratorInfo.IsLockedOut), administrator.Locked.ToString())
                .Set(nameof(AdministratorInfo.SiteIdCollection), administrator.SiteIdCollection)
                .Set(nameof(AdministratorInfo.SiteId), administrator.SiteId)
                .Set(nameof(AdministratorInfo.DisplayName), administrator.DisplayName)
                .Set(nameof(AdministratorInfo.Mobile), administrator.Mobile)
                .Set(nameof(AdministratorInfo.Email), administrator.Email)
                .Set(nameof(AdministratorInfo.AvatarUrl), administrator.AvatarUrl)
                .Set(nameof(AdministratorInfo.UserName), administrator.UserName)
                .Where(nameof(AdministratorInfo.Id), administrator.Id)
            );

            AdminManager.UpdateCache(administrator);

            return (true, string.Empty);
        }

        public async Task<(bool IsValid, string ErrorMessage)> ChangePasswordAsync(Administrator adminInfo, string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return (false, "密码不能为空");
            }
            if (password.Length < ConfigManager.SystemConfigInfo.AdminPasswordMinLength)
            {
                return (false, $"密码长度必须大于等于{ConfigManager.SystemConfigInfo.AdminPasswordMinLength}");
            }
            if (
                !EUserPasswordRestrictionUtils.IsValid(password, ConfigManager.SystemConfigInfo.AdminPasswordRestriction))
            {
                return (false, $"密码不符合规则，请包含{EUserPasswordRestrictionUtils.GetText(EUserPasswordRestrictionUtils.GetEnumType(ConfigManager.SystemConfigInfo.AdminPasswordRestriction))}");
            }

            password = EncodePassword(password, EPasswordFormat.Encrypted, out var passwordSalt);
            await ChangePasswordAsync(adminInfo, EPasswordFormat.Encrypted, passwordSalt, password);
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

            if (ConfigManager.SystemConfigInfo.IsAdminLockLogin)
            {
                if (administrator.CountOfFailedLogin > 0 &&
                    administrator.CountOfFailedLogin >= ConfigManager.SystemConfigInfo.AdminLockLoginCount)
                {
                    var lockType = EUserLockTypeUtils.GetEnumType(ConfigManager.SystemConfigInfo.AdminLockLoginType);
                    if (lockType == EUserLockType.Forever)
                    {
                        return (false, userName, "此账号错误登录次数过多，已被永久锁定");
                    }
                    if (lockType == EUserLockType.Hours && administrator.LastActivityDate.HasValue)
                    {
                        var ts = new TimeSpan(DateTime.Now.Ticks - administrator.LastActivityDate.Value.Ticks);
                        var hours = Convert.ToInt32(ConfigManager.SystemConfigInfo.AdminLockLoginHours - ts.TotalHours);
                        if (hours > 0)
                        {
                            return (false, userName, $"此账号错误登录次数过多，已被锁定，请等待{hours}小时后重试");
                        }
                    }
                }
            }

            var adminInfo = await _repository.GetAsync(administrator.Id);

            return CheckPassword(password, isPasswordMd5, adminInfo.Password,
                EPasswordFormatUtils.GetEnumType(adminInfo.PasswordFormat), adminInfo.PasswordSalt)
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

            var dbList = await _repository.GetAllAsync(Q.Offset(offset).Limit(limit).OrderBy(nameof(AdministratorInfo.Id)));

            if (dbList != null)
            {
                foreach (var dbInfo in dbList)
                {
                    if (dbInfo != null)
                    {
                        list.Add(ToDto(dbInfo));
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
            var adminInfoToDelete = await GetByIdAsync(id);

            await _repository.DeleteAsync(id);

            return adminInfoToDelete;
        }
    }
}
