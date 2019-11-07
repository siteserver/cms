using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Datory;
using SiteServer.CMS.Core;
using SiteServer.CMS.Data;
using SiteServer.CMS.Model;
using SiteServer.Utils.Auth;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model.Db;
using SiteServer.CMS.Model.Mappings;
using SqlKata;

namespace SiteServer.CMS.Provider
{
    public class UserDao : DataProviderBase
    {
        private readonly Repository<UserInfo> _repository;

        public UserDao()
        {
            _repository = new Repository<UserInfo>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString));
        }

        public override string TableName => _repository.TableName;
        public override List<TableColumn> TableColumns => _repository.TableColumns;

        private static User ToDto(UserInfo userInfo)
        {
            return MapperManager.MapTo<User>(userInfo);
        }

        private static UserInfo ToDb(User user)
        {
            return MapperManager.MapTo<UserInfo>(user);
        }

        private async Task<(bool IsValid, string ErrorMessage)> InsertValidateAsync(string userName, string email, string mobile, string password, string ipAddress)
        {
            if (!UserManager.IsIpAddressCached(ipAddress))
            {
                return (false, $"同一IP在{ConfigManager.SystemConfigInfo.UserRegistrationMinMinutes}分钟内只能注册一次");
            }
            if (string.IsNullOrEmpty(password))
            {
                return (false, "密码不能为空");
            }
            if (password.Length < ConfigManager.SystemConfigInfo.UserPasswordMinLength)
            {
                return (false, $"密码长度必须大于等于{ConfigManager.SystemConfigInfo.UserPasswordMinLength}");
            }
            if (!EUserPasswordRestrictionUtils.IsValid(password, ConfigManager.SystemConfigInfo.UserPasswordRestriction))
            {
                return (false, $"密码不符合规则，请包含{EUserPasswordRestrictionUtils.GetText(EUserPasswordRestrictionUtils.GetEnumType(ConfigManager.SystemConfigInfo.UserPasswordRestriction))}");
            }
            if (string.IsNullOrEmpty(userName))
            {
                return (false, "用户名为空，请填写用户名");
            }
            if (!string.IsNullOrEmpty(userName) && await IsUserNameExistsAsync(userName))
            {
                return (false, "用户名已被注册，请更换用户名");
            }
            if (!IsUserNameCompliant(userName.Replace("@", string.Empty).Replace(".", string.Empty)))
            {
                return (false, "用户名包含不规则字符，请更换用户名");
            }
            
            if (!string.IsNullOrEmpty(email) && await IsEmailExistsAsync(email))
            {
                return (false, "电子邮件地址已被注册，请更换邮箱");
            }
            if (!string.IsNullOrEmpty(mobile) && await IsMobileExistsAsync(mobile))
            {
                return (false, "手机号码已被注册，请更换手机号码");
            }

            return (true, string.Empty);
        }

        private async Task<(bool IsValid, string ErrorMessage)> UpdateValidateAsync(Dictionary<string, object> body, string userName, string email, string mobile)
        {
            var bodyUserName = string.Empty;
            if (body.ContainsKey("userName"))
            {
                bodyUserName = (string) body["userName"];
            }

            if (!string.IsNullOrEmpty(bodyUserName) && bodyUserName != userName)
            {
                if (!IsUserNameCompliant(bodyUserName.Replace("@", string.Empty).Replace(".", string.Empty)))
                {
                    return (false, "用户名包含不规则字符，请更换用户名");
                }
                if (!string.IsNullOrEmpty(bodyUserName) && await IsUserNameExistsAsync(bodyUserName))
                {
                    return (false, "用户名已被注册，请更换用户名");
                }
            }

            var bodyEmail = string.Empty;
            if (body.ContainsKey("email"))
            {
                bodyEmail = (string)body["email"];
            }

            if (bodyEmail != null && bodyEmail != email)
            {
                if (!string.IsNullOrEmpty(bodyEmail) && await IsEmailExistsAsync(bodyEmail))
                {
                    return (false, "电子邮件地址已被注册，请更换邮箱");
                }
            }

            var bodyMobile = string.Empty;
            if (body.ContainsKey("mobile"))
            {
                bodyMobile = (string)body["mobile"];
            }

            if (bodyMobile != null && bodyMobile != mobile)
            {
                if (!string.IsNullOrEmpty(bodyMobile) && await IsMobileExistsAsync(bodyMobile))
                {
                    return (false, "手机号码已被注册，请更换手机号码");
                }
            }

            return (true, string.Empty);
        }

        public async Task<(int UserId, string ErrorMessage)> InsertAsync(User user, string password, string ipAddress)
        {
            if (!ConfigManager.SystemConfigInfo.IsUserRegistrationAllowed)
            {
                return (0, "对不起，系统已禁止新用户注册！");
            }

            try
            {
                user.Checked = ConfigManager.SystemConfigInfo.IsUserRegistrationChecked;
                if (StringUtils.IsMobile(user.UserName) && string.IsNullOrEmpty(user.Mobile))
                {
                    user.Mobile = user.UserName;
                }

                var valid = await InsertValidateAsync(user.UserName, user.Email, user.Mobile, password, ipAddress);
                if (!valid.IsValid)
                {
                    return (0, valid.ErrorMessage);
                }

                var passwordSalt = GenerateSalt();
                password = EncodePassword(password, EPasswordFormat.Encrypted, passwordSalt);
                user.CreateDate = DateTime.Now;
                user.LastActivityDate = DateTime.Now;
                user.LastResetPasswordDate = DateTime.Now;

                user.Id = await InsertWithoutValidationAsync(user, password, EPasswordFormat.Encrypted, passwordSalt);

                UserManager.CacheIpAddress(ipAddress);

                return (user.Id, string.Empty);
            }
            catch (Exception ex)
            {
                return (0, ex.Message);
            }
        }

        private async Task<int> InsertWithoutValidationAsync(User user, string password, EPasswordFormat passwordFormat, string passwordSalt)
        {
            user.CreateDate = DateTime.Now;
            user.LastActivityDate = DateTime.Now;
            user.LastResetPasswordDate = DateTime.Now;

            var userInfo = ToDb(user);
            userInfo.Password = password;
            userInfo.PasswordFormat = EPasswordFormatUtils.GetValue(passwordFormat);
            userInfo.PasswordSalt = passwordSalt;

            user.Id = await _repository.InsertAsync(ToDb(user));

            return user.Id;
        }

        public static bool IsPasswordCorrect(string password, out string errorMessage)
        {
            errorMessage = null;
            if (string.IsNullOrEmpty(password))
            {
                errorMessage = "密码不能为空";
                return false;
            }
            if (password.Length < ConfigManager.SystemConfigInfo.UserPasswordMinLength)
            {
                errorMessage = $"密码长度必须大于等于{ConfigManager.SystemConfigInfo.UserPasswordMinLength}";
                return false;
            }
            if (!EUserPasswordRestrictionUtils.IsValid(password, ConfigManager.SystemConfigInfo.UserPasswordRestriction))
            {
                errorMessage =
                    $"密码不符合规则，请包含{EUserPasswordRestrictionUtils.GetText(EUserPasswordRestrictionUtils.GetEnumType(ConfigManager.SystemConfigInfo.UserPasswordRestriction))}";
                return false;
            }
            return true;
        }

        public async Task<(User User, string ErrorMessage)> UpdateAsync(User user, Dictionary<string, object> body)
        {
            var valid = await UpdateValidateAsync(body, user.UserName, user.Email, user.Mobile);
            if (!valid.IsValid)
            {
                return (null, valid.ErrorMessage);
            }

            user.Load(body);

            await UpdateAsync(user);

            return (user, string.Empty);
        }

        public async Task UpdateAsync(User user)
        {
            if (user == null) return;

            var userInfoDb = await _repository.GetAsync(user.Id);
            var userInfo = ToDb(user);
            userInfo.Password = userInfoDb.Password;
            userInfo.PasswordFormat = userInfoDb.PasswordFormat;
            userInfo.PasswordSalt = userInfoDb.PasswordSalt;

            await _repository.UpdateAsync(userInfo);

            UserManager.UpdateCache(user);
        }

        private async Task UpdateLastActivityDateAndCountOfFailedLoginAsync(User user)
        {
            if (user == null) return;

            user.LastActivityDate = DateTime.Now;
            user.CountOfFailedLogin += 1;

            await _repository.UpdateAsync(Q
                .Set(nameof(UserInfo.LastActivityDate), user.LastActivityDate)
                .Set(nameof(UserInfo.CountOfFailedLogin), user.CountOfFailedLogin)
                .Where(nameof(UserInfo.Id), user.Id)
            );

            UserManager.UpdateCache(user);
        }

        public async Task UpdateLastActivityDateAndCountOfLoginAsync(User user)
        {
            if (user == null) return;

            user.LastActivityDate = DateTime.Now;
            user.CountOfLogin += 1;
            user.CountOfFailedLogin = 0;

            await _repository.UpdateAsync(Q
                .Set(nameof(UserInfo.LastActivityDate), user.LastActivityDate)
                .Set(nameof(UserInfo.CountOfLogin), user.CountOfLogin)
                .Set(nameof(UserInfo.CountOfFailedLogin), user.CountOfFailedLogin)
                .Where(nameof(UserInfo.Id), user.Id)
            );

            UserManager.UpdateCache(user);
        }

        private static string EncodePassword(string password, EPasswordFormat passwordFormat, string passwordSalt)
        {
            var retVal = string.Empty;

            if (passwordFormat == EPasswordFormat.Clear)
            {
                retVal = password;
            }
            else if (passwordFormat == EPasswordFormat.Hashed)
            {
                var src = Encoding.Unicode.GetBytes(password);
                var buffer2 = Convert.FromBase64String(passwordSalt);
                var dst = new byte[buffer2.Length + src.Length];
                byte[] inArray = null;
                Buffer.BlockCopy(buffer2, 0, dst, 0, buffer2.Length);
                Buffer.BlockCopy(src, 0, dst, buffer2.Length, src.Length);
                var algorithm = HashAlgorithm.Create("SHA1");
                if (algorithm != null) inArray = algorithm.ComputeHash(dst);

                if (inArray != null) retVal = Convert.ToBase64String(inArray);
            }
            else if (passwordFormat == EPasswordFormat.Encrypted)
            {
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

        private static string GenerateSalt()
        {
            var data = new byte[0x10];
            new RNGCryptoServiceProvider().GetBytes(data);
            return Convert.ToBase64String(data);
        }

        public async Task<(bool IsValid, string ErrorMessage)> ChangePasswordAsync(string userName, string password)
        {
            if (password.Length < ConfigManager.SystemConfigInfo.UserPasswordMinLength)
            {
                return (false, $"密码长度必须大于等于{ConfigManager.SystemConfigInfo.UserPasswordMinLength}");
            }
            if (!EUserPasswordRestrictionUtils.IsValid(password, ConfigManager.SystemConfigInfo.UserPasswordRestriction))
            {
                return (false, $"密码不符合规则，请包含{EUserPasswordRestrictionUtils.GetText(EUserPasswordRestrictionUtils.GetEnumType(ConfigManager.SystemConfigInfo.UserPasswordRestriction))}");
            }

            var passwordSalt = GenerateSalt();
            password = EncodePassword(password, EPasswordFormat.Encrypted, passwordSalt);
            await ChangePasswordAsync(userName, EPasswordFormat.Encrypted, passwordSalt, password);
            return (true, string.Empty);
        }

        private async Task ChangePasswordAsync(string userName, EPasswordFormat passwordFormat, string passwordSalt, string password)
        {
            var user = await UserManager.GetUserByUserNameAsync(userName);
            if (user == null) return;

            user.LastResetPasswordDate = DateTime.Now;

            await _repository.UpdateAsync(Q
                .Set(nameof(UserInfo.Password), password)
                .Set(nameof(UserInfo.PasswordFormat), EPasswordFormatUtils.GetValue(passwordFormat))
                .Set(nameof(UserInfo.PasswordSalt), passwordSalt)
                .Set(nameof(UserInfo.LastResetPasswordDate), user.LastResetPasswordDate)
                .Where(nameof(UserInfo.Id), user.Id)
            );

            LogUtils.AddUserLog(userName, "修改密码", string.Empty);

            UserManager.UpdateCache(user);
        }

        public async Task CheckAsync(List<int> idList)
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(UserInfo.IsChecked), true.ToString())
                .WhereIn(nameof(UserInfo.Id), idList)
            );

            UserManager.ClearCache();
        }

        public async Task LockAsync(List<int> idList)
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(UserInfo.IsLockedOut), true.ToString())
                .WhereIn(nameof(UserInfo.Id), idList)
            );

            UserManager.ClearCache();
        }

        public async Task UnLockAsync(List<int> idList)
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(UserInfo.IsLockedOut), false.ToString())
                .Set(nameof(UserInfo.CountOfFailedLogin), 0)
                .WhereIn(nameof(UserInfo.Id), idList)
            );

            UserManager.ClearCache();
        }

        private async Task<User> GetByAccountAsync(string account)
        {
            var userInfo = await GetByUserNameAsync(account);
            if (userInfo != null) return userInfo;
            if (StringUtils.IsMobile(account)) return await GetByMobileAsync(account);
            if (StringUtils.IsEmail(account)) return await GetByEmailAsync(account);

            return null;
        }

        public async Task<User> GetByUserNameAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return null;

            var userInfo = await _repository.GetAsync(Q.Where(nameof(User.UserName), userName));
            if (userInfo == null) return null;

            var user = ToDto(userInfo);

            UserManager.UpdateCache(user);

            return user;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email)) return null;

            var userInfo = await _repository.GetAsync(Q.Where(nameof(User.Email), email));
            if (userInfo == null) return null;

            var user = ToDto(userInfo);

            UserManager.UpdateCache(user);

            return user;
        }

        public async Task<User> GetByMobileAsync(string mobile)
        {
            if (string.IsNullOrEmpty(mobile)) return null;

            var userInfo = await _repository.GetAsync(Q.Where(nameof(User.Mobile), mobile));
            if (userInfo == null) return null;

            var user = ToDto(userInfo);

            UserManager.UpdateCache(user);

            return user;
        }

        public async Task<User> GetByUserIdAsync(int id)
        {
            if (id <= 0) return null;

            var userInfo = await _repository.GetAsync(id);
            if (userInfo == null) return null;

            var user = ToDto(userInfo);

            UserManager.UpdateCache(user);

            return user;
        }

        public async Task<bool> IsUserNameExistsAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return false;

            return await _repository.ExistsAsync(Q.Where(nameof(UserInfo.UserName), userName));
        }

        private static bool IsUserNameCompliant(string userName)
        {
            if (userName.IndexOf("　", StringComparison.Ordinal) != -1 || userName.IndexOf(" ", StringComparison.Ordinal) != -1 || userName.IndexOf("'", StringComparison.Ordinal) != -1 || userName.IndexOf(":", StringComparison.Ordinal) != -1 || userName.IndexOf(".", StringComparison.Ordinal) != -1)
            {
                return false;
            }
            return DirectoryUtils.IsDirectoryNameCompliant(userName);
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;

            var exists = await IsUserNameExistsAsync(email);
            if (exists) return true;

            return await _repository.ExistsAsync(Q.Where(nameof(UserInfo.Email), email));
        }

        public async Task<bool> IsMobileExistsAsync(string mobile)
        {
            if (string.IsNullOrEmpty(mobile)) return false;

            var exists = await IsUserNameExistsAsync(mobile);
            if (exists) return true;

            return await _repository.ExistsAsync(Q.Where(nameof(UserInfo.Mobile), mobile));
        }

        public async Task<IEnumerable<int>> GetIdListAsync(bool isChecked)
        {
            return await _repository.GetAllAsync<int>(Q
                .Where(nameof(UserInfo.IsChecked), isChecked.ToString())
                .OrderByDesc(nameof(UserInfo.Id))
            );
        }

        public bool CheckPassword(string password, bool isPasswordMd5, string dbPassword, EPasswordFormat passwordFormat, string passwordSalt)
        {
            var decodePassword = DecodePassword(dbPassword, passwordFormat, passwordSalt);
            if (isPasswordMd5)
            {
                return password == AuthUtils.Md5ByString(decodePassword);
            }
            return password == decodePassword;
        }

        public async Task<(User User, string UserName, string ErrorMessage)> ValidateAsync(string account, string password, bool isPasswordMd5)
        {
            if (string.IsNullOrEmpty(account))
            {
                return (null, null, "账号不能为空");
            }
            if (string.IsNullOrEmpty(password))
            {
                return (null, null, "密码不能为空");
            }

            var user = await GetByAccountAsync(account);

            if (string.IsNullOrEmpty(user?.UserName))
            {
                return (null, null, "帐号或密码错误");
            }

            if (!user.Checked)
            {
                return (null, user.UserName, "此账号未审核，无法登录");
            }

            if (user.Locked)
            {
                return (null, user.UserName, "此账号被锁定，无法登录");
            }

            if (ConfigManager.SystemConfigInfo.IsUserLockLogin)
            {
                if (user.CountOfFailedLogin > 0 && user.CountOfFailedLogin >= ConfigManager.SystemConfigInfo.UserLockLoginCount)
                {
                    var lockType = EUserLockTypeUtils.GetEnumType(ConfigManager.SystemConfigInfo.UserLockLoginType);
                    if (lockType == EUserLockType.Forever)
                    {
                        return (null, user.UserName, "此账号错误登录次数过多，已被永久锁定");
                    }
                    if (lockType == EUserLockType.Hours && user.LastActivityDate.HasValue)
                    {
                        var ts = new TimeSpan(DateTime.Now.Ticks - user.LastActivityDate.Value.Ticks);
                        var hours = Convert.ToInt32(ConfigManager.SystemConfigInfo.UserLockLoginHours - ts.TotalHours);
                        if (hours > 0)
                        {
                            return (null, user.UserName, $"此账号错误登录次数过多，已被锁定，请等待{hours}小时后重试");
                        }
                    }
                }
            }

            var userInfo = await _repository.GetAsync(user.Id);

            if (!CheckPassword(password, isPasswordMd5, userInfo.Password, EPasswordFormatUtils.GetEnumType(userInfo.PasswordFormat), userInfo.PasswordSalt))
            {
                await DataProvider.UserDao.UpdateLastActivityDateAndCountOfFailedLoginAsync(user);
                LogUtils.AddUserLog(userInfo.UserName, "用户登录失败", "帐号或密码错误");
                return (null, user.UserName, "帐号或密码错误");
            }

            return (user, user.UserName, string.Empty);
        }

        public Dictionary<DateTime, int> GetTrackingDictionary(DateTime dateFrom, DateTime dateTo, string xType)
        {
            var dict = new Dictionary<DateTime, int>();
            if (string.IsNullOrEmpty(xType))
            {
                xType = EStatictisXTypeUtils.GetValue(EStatictisXType.Day);
            }

            var builder = new StringBuilder();
            builder.Append($" AND CreateDate >= {SqlUtils.GetComparableDate(dateFrom)}");
            builder.Append($" AND CreateDate < {SqlUtils.GetComparableDate(dateTo)}");

            string sqlString = $@"
SELECT COUNT(*) AS AddNum, AddYear, AddMonth, AddDay FROM (
    SELECT {SqlUtils.GetDatePartYear("CreateDate")} AS AddYear, {SqlUtils.GetDatePartMonth("CreateDate")} AS AddMonth, {SqlUtils.GetDatePartDay("CreateDate")} AS AddDay 
    FROM {TableName} 
    WHERE {SqlUtils.GetDateDiffLessThanDays("CreateDate", 30.ToString())} {builder}
) DERIVEDTBL GROUP BY AddYear, AddMonth, AddDay ORDER BY AddYear, AddMonth, AddDay
";//添加日统计

            if (EStatictisXTypeUtils.Equals(xType, EStatictisXType.Month))
            {
                sqlString = $@"
SELECT COUNT(*) AS AddNum, AddYear, AddMonth FROM (
    SELECT {SqlUtils.GetDatePartYear("CreateDate")} AS AddYear, {SqlUtils.GetDatePartMonth("CreateDate")} AS AddMonth 
    FROM {TableName} 
    WHERE {SqlUtils.GetDateDiffLessThanMonths("CreateDate", 12.ToString())} {builder}
) DERIVEDTBL GROUP BY AddYear, AddMonth ORDER BY AddYear, AddMonth
";//添加月统计
            }
            else if (EStatictisXTypeUtils.Equals(xType, EStatictisXType.Year))
            {
                sqlString = $@"
SELECT COUNT(*) AS AddNum, AddYear FROM (
    SELECT {SqlUtils.GetDatePartYear("CreateDate")} AS AddYear
    FROM {TableName} 
    WHERE {SqlUtils.GetDateDiffLessThanYears("CreateDate", 10.ToString())} {builder}
) DERIVEDTBL GROUP BY AddYear ORDER BY AddYear
";//添加年统计
            }

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var accessNum = GetInt(rdr, 0);
                    if (EStatictisXTypeUtils.Equals(xType, EStatictisXType.Day))
                    {
                        var year = GetString(rdr, 1);
                        var month = GetString(rdr, 2);
                        var day = GetString(rdr, 3);
                        var dateTime = TranslateUtils.ToDateTime($"{year}-{month}-{day}");
                        dict.Add(dateTime, accessNum);
                    }
                    else if (EStatictisXTypeUtils.Equals(xType, EStatictisXType.Month))
                    {
                        var year = GetString(rdr, 1);
                        var month = GetString(rdr, 2);

                        var dateTime = TranslateUtils.ToDateTime($"{year}-{month}-1");
                        dict.Add(dateTime, accessNum);
                    }
                    else if (EStatictisXTypeUtils.Equals(xType, EStatictisXType.Year))
                    {
                        var year = GetString(rdr, 1);
                        var dateTime = TranslateUtils.ToDateTime($"{year}-1-1");
                        dict.Add(dateTime, accessNum);
                    }
                }
                rdr.Close();
            }
            return dict;
        }

        public async Task<int> GetCountAsync()
        {
            return await _repository.CountAsync();
        }

        private Query GetQuery(ETriState state, int groupId, int dayOfLastActivity, string keyword, string order)
        {
            var query = Q.NewQuery();

            if (state != ETriState.All)
            {
                query.Where(nameof(UserInfo.IsChecked), state.ToString());
            }

            if (dayOfLastActivity > 0)
            {
                var dateTime = DateTime.Now.AddDays(-dayOfLastActivity);
                query.WhereDate(nameof(UserInfo.LastActivityDate), ">=", dateTime);
            }

            if (groupId > -1)
            {
                if (groupId > 0)
                {
                    query.Where(nameof(UserInfo.GroupId), groupId);
                }
                else
                {
                    query.Where(q => q
                        .Where(nameof(UserInfo.GroupId), 0)
                        .OrWhereNull(nameof(UserInfo.GroupId))
                    );
                }
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                var like = $"%{keyword}%";
                query.Where(q => q
                    .WhereLike(nameof(UserInfo.UserName), like)
                    .OrWhereLike(nameof(UserInfo.Email), like)
                    .OrWhereLike(nameof(UserInfo.Mobile), like)
                    .OrWhereLike(nameof(UserInfo.DisplayName), like)
                );
            }

            if (!string.IsNullOrEmpty(order))
            {
                if (StringUtils.EqualsIgnoreCase(order, nameof(UserInfo.UserName)))
                {
                    query.OrderBy(nameof(UserInfo.UserName));
                }
                else
                {
                    query.OrderByDesc(order);
                }
            }
            else
            {
                query.OrderByDesc(nameof(UserInfo.Id));
            }

            return query;
        }

        public async Task<int> GetCountAsync(ETriState state, int groupId, int dayOfLastActivity, string keyword)
        {
            var query = GetQuery(state, groupId, dayOfLastActivity, keyword, string.Empty);
            return await _repository.CountAsync(query);
        }

        public async Task<List<User>> GetUsersAsync(ETriState state, int groupId, int dayOfLastActivity, string keyword, string order, int offset, int limit)
        {
            var list = new List<User>();

            var query = GetQuery(state, groupId, dayOfLastActivity, keyword, order);
            query
                .Select(nameof(User.Id))
                .Offset(offset)
                .Limit(limit);

            var dbList = await _repository.GetAllAsync<int>(query);

            if (dbList != null)
            {
                foreach (var userId in dbList)
                {
                    list.Add(await UserManager.GetUserByUserIdAsync(userId));
                }
            }

            return list;
        }

        public async Task<bool> IsExistsAsync(int id)
        {
            return await _repository.ExistsAsync(id);
        }

        public async Task DeleteAsync(User user)
        {
            await _repository.DeleteAsync(user.Id);
            
            UserManager.RemoveCache(user);
        }
    }
}

