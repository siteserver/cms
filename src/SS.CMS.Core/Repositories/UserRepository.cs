using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using SS.CMS.Abstractions.Enums;
using SS.CMS.Abstractions.Models;
using SS.CMS.Abstractions.Repositories;
using SS.CMS.Abstractions.Services;
using SS.CMS.Core.Security;
using SS.CMS.Data;
using SS.CMS.Utils;
using SS.CMS.Utils.Enumerations;
using Attr = SS.CMS.Core.Models.Attributes.UserAttribute;

namespace SS.CMS.Core.Repositories
{
    public partial class UserRepository : IUserRepository
    {
        private static readonly string CacheKey = StringUtils.GetCacheKey(nameof(UserRepository));
        private readonly Repository<UserInfo> _repository;
        private readonly ISettingsManager _settingsManager;
        private readonly ICacheManager _cacheManager;
        private readonly IUserLogRepository _userLogRepository;

        public UserRepository(ISettingsManager settingsManager, ICacheManager cacheManager, IUserLogRepository userLogRepository)
        {
            _repository = new Repository<UserInfo>(new Db(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
            _settingsManager = settingsManager;
            _cacheManager = cacheManager;
            _userLogRepository = userLogRepository;
        }

        public IDb Db => _repository.Db;

        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private bool InsertValidate(string userName, string email, string mobile, string password, string ipAddress, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (!IsIpAddressCached(ipAddress))
            {
                errorMessage = $"同一IP在{_settingsManager.ConfigInfo.UserRegistrationMinMinutes}分钟内只能注册一次";
                return false;
            }
            if (string.IsNullOrEmpty(password))
            {
                errorMessage = "密码不能为空";
                return false;
            }
            if (password.Length < _settingsManager.ConfigInfo.UserPasswordMinLength)
            {
                errorMessage = $"密码长度必须大于等于{_settingsManager.ConfigInfo.UserPasswordMinLength}";
                return false;
            }
            if (!EUserPasswordRestrictionUtils.IsValid(password, _settingsManager.ConfigInfo.UserPasswordRestriction))
            {
                errorMessage =
                    $"密码不符合规则，请包含{EUserPasswordRestrictionUtils.GetText(EUserPasswordRestrictionUtils.GetEnumType(_settingsManager.ConfigInfo.UserPasswordRestriction))}";
                return false;
            }
            if (string.IsNullOrEmpty(userName))
            {
                errorMessage = "用户名为空，请填写用户名";
                return false;
            }
            if (!string.IsNullOrEmpty(userName) && IsUserNameExists(userName))
            {
                errorMessage = "用户名已被注册，请更换用户名";
                return false;
            }
            if (!IsUserNameCompliant(userName.Replace("@", string.Empty).Replace(".", string.Empty)))
            {
                errorMessage = "用户名包含不规则字符，请更换用户名";
                return false;
            }

            if (!string.IsNullOrEmpty(email) && IsEmailExists(email))
            {
                errorMessage = "电子邮件地址已被注册，请更换邮箱";
                return false;
            }
            if (!string.IsNullOrEmpty(mobile) && IsMobileExists(mobile))
            {
                errorMessage = "手机号码已被注册，请更换手机号码";
                return false;
            }

            return true;
        }

        private bool UpdateValidate(Dictionary<string, object> body, string userName, string email, string mobile, out string errorMessage)
        {
            errorMessage = string.Empty;

            var bodyUserName = string.Empty;
            if (body.ContainsKey("userName"))
            {
                bodyUserName = (string)body["userName"];
            }

            if (!string.IsNullOrEmpty(bodyUserName) && bodyUserName != userName)
            {
                if (!IsUserNameCompliant(bodyUserName.Replace("@", string.Empty).Replace(".", string.Empty)))
                {
                    errorMessage = "用户名包含不规则字符，请更换用户名";
                    return false;
                }
                if (!string.IsNullOrEmpty(bodyUserName) && IsUserNameExists(bodyUserName))
                {
                    errorMessage = "用户名已被注册，请更换用户名";
                    return false;
                }
            }

            var bodyEmail = string.Empty;
            if (body.ContainsKey("email"))
            {
                bodyEmail = (string)body["email"];
            }

            if (bodyEmail != null && bodyEmail != email)
            {
                if (!string.IsNullOrEmpty(bodyEmail) && IsEmailExists(bodyEmail))
                {
                    errorMessage = "电子邮件地址已被注册，请更换邮箱";
                    return false;
                }
            }

            var bodyMobile = string.Empty;
            if (body.ContainsKey("mobile"))
            {
                bodyMobile = (string)body["mobile"];
            }

            if (bodyMobile != null && bodyMobile != mobile)
            {
                if (!string.IsNullOrEmpty(bodyMobile) && IsMobileExists(bodyMobile))
                {
                    errorMessage = "手机号码已被注册，请更换手机号码";
                    return false;
                }
            }

            return true;
        }

        public int Insert(UserInfo userInfo, string password, string ipAddress, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (userInfo == null) return 0;

            if (!_settingsManager.ConfigInfo.IsUserRegistrationAllowed)
            {
                errorMessage = "对不起，系统已禁止新用户注册！";
                return 0;
            }

            try
            {
                userInfo.Checked = _settingsManager.ConfigInfo.IsUserRegistrationChecked;
                if (StringUtils.IsMobile(userInfo.UserName) && string.IsNullOrEmpty(userInfo.Mobile))
                {
                    userInfo.Mobile = userInfo.UserName;
                }

                if (!InsertValidate(userInfo.UserName, userInfo.Email, userInfo.Mobile, password, ipAddress, out errorMessage)) return 0;

                var passwordSalt = GenerateSalt();
                password = EncodePassword(password, PasswordFormat.Encrypted, passwordSalt);
                userInfo.CreateDate = DateTime.Now;
                userInfo.LastActivityDate = DateTime.Now;
                userInfo.LastResetPasswordDate = DateTime.Now;

                userInfo.Id = InsertWithoutValidation(userInfo, password, PasswordFormat.Encrypted, passwordSalt);

                CacheIpAddress(ipAddress);

                return userInfo.Id;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return 0;
            }
        }

        private int InsertWithoutValidation(UserInfo userInfo, string password, PasswordFormat passwordFormat, string passwordSalt)
        {
            userInfo.Password = password;
            userInfo.PasswordFormat = passwordFormat.Value;
            userInfo.PasswordSalt = passwordSalt;
            userInfo.CreateDate = DateTime.Now;
            userInfo.LastActivityDate = DateTime.Now;
            userInfo.LastResetPasswordDate = DateTime.Now;

            return _repository.Insert(userInfo);
        }

        public bool IsPasswordCorrect(string password, out string errorMessage)
        {
            errorMessage = null;
            if (string.IsNullOrEmpty(password))
            {
                errorMessage = "密码不能为空";
                return false;
            }
            if (password.Length < _settingsManager.ConfigInfo.UserPasswordMinLength)
            {
                errorMessage = $"密码长度必须大于等于{_settingsManager.ConfigInfo.UserPasswordMinLength}";
                return false;
            }
            if (!EUserPasswordRestrictionUtils.IsValid(password, _settingsManager.ConfigInfo.UserPasswordRestriction))
            {
                errorMessage =
                    $"密码不符合规则，请包含{EUserPasswordRestrictionUtils.GetText(EUserPasswordRestrictionUtils.GetEnumType(_settingsManager.ConfigInfo.UserPasswordRestriction))}";
                return false;
            }
            return true;
        }

        public UserInfo Update(UserInfo userInfo, Dictionary<string, object> body, out string errorMessage)
        {
            if (!UpdateValidate(body, userInfo.UserName, userInfo.Email, userInfo.Mobile, out errorMessage)) return null;

            foreach (var o in body)
            {
                userInfo.Set(o.Key, o.Value);
            }

            Update(userInfo);

            return userInfo;
        }

        public bool Update(UserInfo userInfo)
        {
            var updated = _repository.Update(userInfo);

            UpdateCache(userInfo);

            return updated;
        }

        private void UpdateLastActivityDateAndCountOfFailedLogin(UserInfo userInfo)
        {
            if (userInfo == null) return;

            userInfo.LastActivityDate = DateTime.Now;
            userInfo.CountOfFailedLogin += 1;

            _repository.Update(userInfo, Attr.LastActivityDate, Attr.CountOfFailedLogin);

            UpdateCache(userInfo);
        }

        public void UpdateLastActivityDateAndCountOfLogin(UserInfo userInfo)
        {
            if (userInfo == null) return;

            userInfo.LastActivityDate = DateTime.Now;
            userInfo.CountOfLogin += 1;
            userInfo.CountOfFailedLogin = 0;

            _repository.Update(userInfo, Attr.LastActivityDate, Attr.CountOfLogin, Attr.CountOfFailedLogin);

            UpdateCache(userInfo);
        }

        private string EncodePassword(string password, PasswordFormat passwordFormat, string passwordSalt)
        {
            var retVal = string.Empty;

            if (passwordFormat == PasswordFormat.Clear)
            {
                retVal = password;
            }
            else if (passwordFormat == PasswordFormat.Hashed)
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
            else if (passwordFormat == PasswordFormat.Encrypted)
            {
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

        private string DecodePassword(string password, PasswordFormat passwordFormat, string passwordSalt)
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

        private static string GenerateSalt()
        {
            var data = new byte[0x10];
            new RNGCryptoServiceProvider().GetBytes(data);
            return Convert.ToBase64String(data);
        }

        public bool ChangePassword(string userName, string password, out string errorMessage)
        {
            errorMessage = null;
            if (password.Length < _settingsManager.ConfigInfo.UserPasswordMinLength)
            {
                errorMessage = $"密码长度必须大于等于{_settingsManager.ConfigInfo.UserPasswordMinLength}";
                return false;
            }
            if (!EUserPasswordRestrictionUtils.IsValid(password, _settingsManager.ConfigInfo.UserPasswordRestriction))
            {
                errorMessage =
                    $"密码不符合规则，请包含{EUserPasswordRestrictionUtils.GetText(EUserPasswordRestrictionUtils.GetEnumType(_settingsManager.ConfigInfo.UserPasswordRestriction))}";
                return false;
            }

            var passwordSalt = GenerateSalt();
            password = EncodePassword(password, PasswordFormat.Encrypted, passwordSalt);
            ChangePassword(userName, PasswordFormat.Encrypted, passwordSalt, password);
            return true;
        }

        private void ChangePassword(string userName, PasswordFormat passwordFormat, string passwordSalt, string password)
        {
            var userInfo = GetUserInfoByUserName(userName);
            if (userInfo == null) return;

            userInfo.PasswordFormat = passwordFormat.Value;
            userInfo.Password = password;
            userInfo.PasswordSalt = passwordSalt;
            userInfo.LastResetPasswordDate = DateTime.Now;

            _repository.Update(userInfo, Attr.PasswordFormat, Attr.Password, Attr.PasswordSalt, Attr.LastResetPasswordDate);

            _userLogRepository.AddUserLog(string.Empty, userName, "修改密码", string.Empty);

            UpdateCache(userInfo);
        }

        public void Check(List<int> idList)
        {
            _repository.Update(Q
                .Set(Attr.IsChecked, true.ToString())
                .WhereIn(Attr.Id, idList)
            );

            ClearCache();
        }

        public void Lock(List<int> idList)
        {
            _repository.Update(Q
                .Set(Attr.IsLockedOut, true.ToString())
                .WhereIn(Attr.Id, idList)
            );

            ClearCache();
        }

        public void UnLock(List<int> idList)
        {
            _repository.Update(Q
                .Set(Attr.IsLockedOut, false.ToString())
                .WhereIn(Attr.Id, idList)
            );

            ClearCache();
        }

        private UserInfo GetByAccount(string account)
        {
            var userInfo = GetByUserName(account);
            if (userInfo != null) return userInfo;
            if (StringUtils.IsMobile(account)) return GetByMobile(account);
            if (StringUtils.IsEmail(account)) return GetByEmail(account);

            return null;
        }

        private UserInfo GetByUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return null;

            var userInfo = _repository.Get(Q.Where(Attr.UserName, userName));

            UpdateCache(userInfo);

            return userInfo;
        }

        private UserInfo GetByEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return null;

            var userInfo = _repository.Get(Q.Where(Attr.Email, email));

            UpdateCache(userInfo);

            return userInfo;
        }

        private UserInfo GetByMobile(string mobile)
        {
            if (string.IsNullOrEmpty(mobile)) return null;

            var userInfo = _repository.Get(Q.Where(Attr.Mobile, mobile));

            UpdateCache(userInfo);

            return userInfo;
        }

        private UserInfo GetByUserId(int id)
        {
            if (id <= 0) return null;

            var userInfo = _repository.Get(id);

            UpdateCache(userInfo);

            return userInfo;
        }

        public bool IsUserNameExists(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return false;

            return _repository.Exists(Q.Where(Attr.UserName, userName));
        }

        private bool IsUserNameCompliant(string userName)
        {
            if (userName.IndexOf("　", StringComparison.Ordinal) != -1 || userName.IndexOf(" ", StringComparison.Ordinal) != -1 || userName.IndexOf("'", StringComparison.Ordinal) != -1 || userName.IndexOf(":", StringComparison.Ordinal) != -1 || userName.IndexOf(".", StringComparison.Ordinal) != -1)
            {
                return false;
            }
            return DirectoryUtils.IsDirectoryNameCompliant(userName);
        }

        public bool IsEmailExists(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;

            var exists = IsUserNameExists(email);
            if (exists) return true;

            return _repository.Exists(Q.Where(Attr.Email, email));
        }

        public bool IsMobileExists(string mobile)
        {
            if (string.IsNullOrEmpty(mobile)) return false;

            var exists = IsUserNameExists(mobile);
            if (exists) return true;

            return _repository.Exists(Q.Where(Attr.Mobile, mobile));
        }

        public IList<int> GetIdList(bool isChecked)
        {
            return _repository.GetAll<int>(Q
                .Select(Attr.Id)
                .Where(Attr.IsChecked, isChecked.ToString())
                .OrderByDesc(Attr.Id)).ToList();
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

        public UserInfo Validate(string account, string password, bool isPasswordMd5, out string userName, out string errorMessage)
        {
            userName = string.Empty;
            errorMessage = string.Empty;

            if (string.IsNullOrEmpty(account))
            {
                errorMessage = "账号不能为空";
                return null;
            }
            if (string.IsNullOrEmpty(password))
            {
                errorMessage = "密码不能为空";
                return null;
            }

            var userInfo = GetByAccount(account);

            if (string.IsNullOrEmpty(userInfo?.UserName))
            {
                errorMessage = "帐号或密码错误";
                return null;
            }

            userName = userInfo.UserName;

            if (!userInfo.Checked)
            {
                errorMessage = "此账号未审核，无法登录";
                return null;
            }

            if (userInfo.Locked)
            {
                errorMessage = "此账号被锁定，无法登录";
                return null;
            }

            if (_settingsManager.ConfigInfo.IsUserLockLogin)
            {
                if (userInfo.CountOfFailedLogin > 0 && userInfo.CountOfFailedLogin >= _settingsManager.ConfigInfo.UserLockLoginCount)
                {
                    var lockType = EUserLockTypeUtils.GetEnumType(_settingsManager.ConfigInfo.UserLockLoginType);
                    if (lockType == EUserLockType.Forever)
                    {
                        errorMessage = "此账号错误登录次数过多，已被永久锁定";
                        return null;
                    }
                    if (lockType == EUserLockType.Hours)
                    {
                        if (userInfo.LastActivityDate.HasValue)
                        {
                            var ts = new TimeSpan(DateTime.Now.Ticks - userInfo.LastActivityDate.Value.Ticks);
                            var hours = Convert.ToInt32(_settingsManager.ConfigInfo.UserLockLoginHours - ts.TotalHours);
                            if (hours > 0)
                            {
                                errorMessage =
                                    $"此账号错误登录次数过多，已被锁定，请等待{hours}小时后重试";
                                return null;
                            }
                        }
                    }
                }
            }

            if (!CheckPassword(password, isPasswordMd5, userInfo.Password, PasswordFormat.Parse(userInfo.PasswordFormat), userInfo.PasswordSalt))
            {
                UpdateLastActivityDateAndCountOfFailedLogin(userInfo);
                _userLogRepository.AddUserLog(string.Empty, userInfo.UserName, "用户登录失败", "帐号或密码错误");
                errorMessage = "帐号或密码错误";
                return null;
            }

            return userInfo;
        }

        //         public Dictionary<DateTime, int> GetTrackingDictionary(DateTime dateFrom, DateTime dateTo, string xType)
        //         {
        //             var dict = new Dictionary<DateTime, int>();
        //             if (string.IsNullOrEmpty(xType))
        //             {
        //                 xType = EStatictisXTypeUtils.GetValue(EStatictisXType.Day);
        //             }

        //             var builder = new StringBuilder();
        //             builder.Append($" AND CreateDate >= {SqlUtils.GetComparableDate(dateFrom)}");
        //             builder.Append($" AND CreateDate < {SqlUtils.GetComparableDate(dateTo)}");

        //             string sqlString = $@"
        // SELECT COUNT(*) AS AddNum, AddYear, AddMonth, AddDay FROM (
        //     SELECT {SqlUtils.GetDatePartYear("CreateDate")} AS AddYear, {SqlUtils.GetDatePartMonth("CreateDate")} AS AddMonth, {SqlUtils.GetDatePartDay("CreateDate")} AS AddDay 
        //     FROM {TableName} 
        //     WHERE {SqlUtils.GetDateDiffLessThanDays("CreateDate", 30.ToString())} {builder}
        // ) DERIVEDTBL GROUP BY AddYear, AddMonth, AddDay ORDER BY AddYear, AddMonth, AddDay
        // ";//添加日统计

        //             if (EStatictisXTypeUtils.Equals(xType, EStatictisXType.Month))
        //             {
        //                 sqlString = $@"
        // SELECT COUNT(*) AS AddNum, AddYear, AddMonth FROM (
        //     SELECT {SqlUtils.GetDatePartYear("CreateDate")} AS AddYear, {SqlUtils.GetDatePartMonth("CreateDate")} AS AddMonth 
        //     FROM {TableName} 
        //     WHERE {SqlUtils.GetDateDiffLessThanMonths("CreateDate", 12.ToString())} {builder}
        // ) DERIVEDTBL GROUP BY AddYear, AddMonth ORDER BY AddYear, AddMonth
        // ";//添加月统计
        //             }
        //             else if (EStatictisXTypeUtils.Equals(xType, EStatictisXType.Year))
        //             {
        //                 sqlString = $@"
        // SELECT COUNT(*) AS AddNum, AddYear FROM (
        //     SELECT {SqlUtils.GetDatePartYear("CreateDate")} AS AddYear
        //     FROM {TableName} 
        //     WHERE {SqlUtils.GetDateDiffLessThanYears("CreateDate", 10.ToString())} {builder}
        // ) DERIVEDTBL GROUP BY AddYear ORDER BY AddYear
        // ";//添加年统计
        //             }

        //             using (var connection = _repository.Db.GetConnection())
        //             {
        //                 using (var rdr = connection.ExecuteReader(sqlString))
        //                 {
        //                     while (rdr.Read())
        //                     {
        //                         var accessNum = rdr.GetInt32(0);
        //                         if (EStatictisXTypeUtils.Equals(xType, EStatictisXType.Day))
        //                         {
        //                             var year = rdr.GetValue(1).ToString();
        //                             var month = rdr.GetValue(2).ToString();
        //                             var day = rdr.GetValue(3).ToString();
        //                             var dateTime = TranslateUtils.ToDateTime($"{year}-{month}-{day}");
        //                             dict.Add(dateTime, accessNum);
        //                         }
        //                         else if (EStatictisXTypeUtils.Equals(xType, EStatictisXType.Month))
        //                         {
        //                             var year = rdr.GetValue(1).ToString();
        //                             var month = rdr.GetValue(2).ToString();

        //                             var dateTime = TranslateUtils.ToDateTime($"{year}-{month}-1");
        //                             dict.Add(dateTime, accessNum);
        //                         }
        //                         else if (EStatictisXTypeUtils.Equals(xType, EStatictisXType.Year))
        //                         {
        //                             var year = rdr.GetValue(1).ToString();
        //                             var dateTime = TranslateUtils.ToDateTime($"{year}-1-1");
        //                             dict.Add(dateTime, accessNum);
        //                         }
        //                     }
        //                     rdr.Close();
        //                 }
        //             }

        //             return dict;
        //         }

        public int GetCount()
        {
            return _repository.Count();
        }

        public IList<UserInfo> GetUsers(int offset, int limit)
        {
            return _repository.GetAll(Q
                .Offset(offset)
                .Limit(limit)
                .OrderBy(Attr.Id)).ToList();
        }

        public bool IsExists(int id)
        {
            return _repository.Exists(id);
        }

        public void Delete(UserInfo userInfo)
        {
            _repository.Delete(userInfo.Id);

            RemoveCache(userInfo);
        }
    }
}

