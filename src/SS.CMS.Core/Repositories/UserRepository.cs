using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
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
        private static readonly string CacheKey = StringUtils.GetCacheKey(nameof(AccessTokenRepository));
        private readonly Repository<UserInfo> _repository;
        private readonly ISettingsManager _settingsManager;
        private readonly ICacheManager _cacheManager;
        private readonly IConfigRepository _configRepository;
        private readonly IUserRoleRepository _userRoleRepository;

        public UserRepository(ISettingsManager settingsManager, ICacheManager cacheManager, IConfigRepository configRepository, IUserRoleRepository userRoleRepository)
        {
            _repository = new Repository<UserInfo>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
            _settingsManager = settingsManager;
            _cacheManager = cacheManager;
            _configRepository = configRepository;
            _userRoleRepository = userRoleRepository;
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

        public async Task<(int UserId, string ErrorMessage)> InsertAsync(UserInfo userInfo)
        {
            var errorMessage = string.Empty;

            if (!InsertValidate(userInfo.UserName, userInfo.Password, userInfo.Email, userInfo.Mobile, out errorMessage)) return (0, errorMessage);

            try
            {
                userInfo.CreatedDate = DateTime.Now;
                userInfo.PasswordFormat = PasswordFormat.Encrypted.Value;
                userInfo.Password = EncodePassword(userInfo.Password, PasswordFormat.Parse(userInfo.PasswordFormat), out var passwordSalt);
                userInfo.PasswordSalt = passwordSalt;

                var identity = await _repository.InsertAsync(userInfo);

                return (identity, null);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return (0, errorMessage);
            }
        }

        public bool Update(UserInfo UserInfo, out string errorMessage)
        {
            var userInfo = GetUserInfoByUserId(UserInfo.Id);

            UserInfo.Password = userInfo.Password;
            UserInfo.PasswordFormat = userInfo.PasswordFormat;
            UserInfo.PasswordSalt = userInfo.PasswordSalt;

            if (!UpdateValidate(UserInfo, userInfo.UserName, userInfo.Email, userInfo.Mobile, out errorMessage)) return false;

            var updated = _repository.Update(UserInfo);

            if (updated)
            {
                UpdateCache(UserInfo);
            }

            return updated;
        }

        public bool UpdateLastActivityDateAndCountOfFailedLogin(UserInfo userInfo)
        {
            if (userInfo == null) return false;

            userInfo.LastActivityDate = DateTime.Now;
            userInfo.CountOfFailedLogin += 1;

            var updated = Update(userInfo, out _);
            if (updated)
            {
                UpdateCache(userInfo);
            }
            return updated;
        }

        public void UpdateLastActivityDateAndCountOfLogin(UserInfo userInfo)
        {
            if (userInfo == null) return;

            userInfo.LastActivityDate = DateTime.Now;
            userInfo.CountOfLogin += 1;
            userInfo.CountOfFailedLogin = 0;

            var updated = Update(userInfo, out _);
            if (updated)
            {
                UpdateCache(userInfo);
            }
        }

        public void UpdateSiteIdCollection(UserInfo userInfo, string siteIdCollection)
        {
            if (userInfo == null) return;

            userInfo.SiteIdCollection = siteIdCollection;

            _repository.Update(Q
                    .Set(Attr.SiteIdCollection, siteIdCollection)
                    .Where(Attr.Id, userInfo.Id)
                );

            UpdateCache(userInfo);
        }

        public List<int> UpdateSiteId(UserInfo userInfo, int siteId)
        {
            if (userInfo == null) return null;

            var siteIdListLatestAccessed = TranslateUtils.StringCollectionToIntList(userInfo.SiteIdCollection);
            if (userInfo.SiteId != siteId || siteIdListLatestAccessed.FirstOrDefault() != siteId)
            {
                siteIdListLatestAccessed.Remove(siteId);
                siteIdListLatestAccessed.Insert(0, siteId);

                userInfo.SiteIdCollection = TranslateUtils.ObjectCollectionToString(siteIdListLatestAccessed);
                userInfo.SiteId = siteId;

                _repository.Update(Q
                    .Set(Attr.SiteId, siteId)
                    .Set(Attr.SiteIdCollection, TranslateUtils.ObjectCollectionToString(siteIdListLatestAccessed))
                    .Where(Attr.Id, userInfo.Id)
                );

                RemoveCache(userInfo);
            }

            return siteIdListLatestAccessed;
        }

        private void ChangePassword(UserInfo userInfo, PasswordFormat passwordFormat, string passwordSalt, string password)
        {
            userInfo.Password = password;
            userInfo.PasswordFormat = passwordFormat.Value;
            userInfo.PasswordSalt = passwordSalt;

            _repository.Update(userInfo, Attr.Password, Attr.PasswordFormat, Attr.PasswordSalt);

            RemoveCache(userInfo);
        }

        public bool Delete(UserInfo userInfo)
        {
            if (userInfo == null) return false;

            var deleted = _repository.Delete(userInfo.Id);

            if (deleted)
            {
                RemoveCache(userInfo);

                _userRoleRepository.RemoveUser(userInfo.UserName);
            }

            return deleted;
        }

        public void Lock(List<int> userIdList)
        {
            _repository.Update(Q
                .Set(Attr.IsLockedOut, true.ToString())
                .WhereIn(Attr.Id, userIdList)
            );

            ClearCache();
        }

        public void UnLock(List<int> userIdList)
        {
            _repository.Update(Q
                .Set(Attr.IsLockedOut, false.ToString())
                .WhereIn(Attr.Id, userIdList)
            );

            ClearCache();
        }

        private UserInfo GetByAccount(string account)
        {
            var UserInfo = GetByUserName(account);
            if (UserInfo != null) return UserInfo;
            if (StringUtils.IsMobile(account)) return GetByMobile(account);
            if (StringUtils.IsEmail(account)) return GetByEmail(account);

            return null;
        }

        public UserInfo GetByUserId(int userId)
        {
            return _repository.Get(userId);
        }

        public UserInfo GetByUserName(string userName)
        {
            return _repository.Get(Q.Where(Attr.UserName, userName));
        }

        public async Task<UserInfo> GetByUserNameAsync(string userName)
        {
            return await _repository.GetAsync(Q.Where(Attr.UserName, userName));
        }

        public UserInfo GetByMobile(string mobile)
        {
            return _repository.Get(Q.Where(Attr.Mobile, mobile));
        }

        public UserInfo GetByEmail(string email)
        {
            return _repository.Get(Q.Where(Attr.Email, email));
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

        private bool UpdateValidate(UserInfo adminInfoToUpdate, string userName, string email, string mobile, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (adminInfoToUpdate.UserName != null && adminInfoToUpdate.UserName != userName)
            {
                if (string.IsNullOrEmpty(adminInfoToUpdate.UserName))
                {
                    errorMessage = "用户名不能为空";
                    return false;
                }
                if (adminInfoToUpdate.UserName.Length < _configRepository.Instance.AdminUserNameMinLength)
                {
                    errorMessage = $"用户名长度必须大于等于{_configRepository.Instance.AdminUserNameMinLength}";
                    return false;
                }
                if (IsUserNameExists(adminInfoToUpdate.UserName))
                {
                    errorMessage = "用户名已存在，请更换用户名";
                    return false;
                }
            }

            if (adminInfoToUpdate.Mobile != null && adminInfoToUpdate.Mobile != mobile)
            {
                if (!string.IsNullOrEmpty(adminInfoToUpdate.Mobile) && IsMobileExists(adminInfoToUpdate.Mobile))
                {
                    errorMessage = "手机号码已被注册，请更换手机号码";
                    return false;
                }
            }

            if (adminInfoToUpdate.Email != null && adminInfoToUpdate.Email != email)
            {
                if (!string.IsNullOrEmpty(adminInfoToUpdate.Email) && IsEmailExists(adminInfoToUpdate.Email))
                {
                    errorMessage = "电子邮件地址已被注册，请更换邮箱";
                    return false;
                }
            }

            return true;
        }

        private bool InsertValidate(string userName, string password, string email, string mobile, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrEmpty(userName))
            {
                errorMessage = "用户名不能为空";
                return false;
            }
            if (userName.Length < _configRepository.Instance.AdminUserNameMinLength)
            {
                errorMessage = $"用户名长度必须大于等于{_configRepository.Instance.AdminUserNameMinLength}";
                return false;
            }
            if (IsUserNameExists(userName))
            {
                errorMessage = "用户名已存在，请更换用户名";
                return false;
            }

            if (string.IsNullOrEmpty(password))
            {
                errorMessage = "密码不能为空";
                return false;
            }
            if (password.Length < _configRepository.Instance.AdminPasswordMinLength)
            {
                errorMessage = $"密码长度必须大于等于{_configRepository.Instance.AdminPasswordMinLength}";
                return false;
            }
            if (
                !EUserPasswordRestrictionUtils.IsValid(password,
                    _configRepository.Instance.AdminPasswordRestriction))
            {
                errorMessage =
                    $"密码不符合规则，请包含{EUserPasswordRestrictionUtils.GetText(EUserPasswordRestrictionUtils.GetEnumType(_configRepository.Instance.AdminPasswordRestriction))}";
                return false;
            }

            if (!string.IsNullOrEmpty(mobile) && IsMobileExists(mobile))
            {
                errorMessage = "手机号码已被注册，请更换手机号码";
                return false;
            }
            if (!string.IsNullOrEmpty(email) && IsEmailExists(email))
            {
                errorMessage = "电子邮件地址已被注册，请更换邮箱";
                return false;
            }

            return true;
        }

        public bool ChangePassword(UserInfo userInfo, string password, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrEmpty(password))
            {
                errorMessage = "密码不能为空";
                return false;
            }
            if (password.Length < _configRepository.Instance.AdminPasswordMinLength)
            {
                errorMessage = $"密码长度必须大于等于{_configRepository.Instance.AdminPasswordMinLength}";
                return false;
            }
            if (
                !EUserPasswordRestrictionUtils.IsValid(password, _configRepository.Instance.AdminPasswordRestriction))
            {
                errorMessage =
                    $"密码不符合规则，请包含{EUserPasswordRestrictionUtils.GetText(EUserPasswordRestrictionUtils.GetEnumType(_configRepository.Instance.AdminPasswordRestriction))}";
                return false;
            }

            password = EncodePassword(password, PasswordFormat.Encrypted, out var passwordSalt);
            ChangePassword(userInfo, PasswordFormat.Encrypted, passwordSalt, password);
            return true;
        }

        public bool Validate(string account, string password, bool isPasswordMd5, out string userName, out string errorMessage)
        {
            userName = string.Empty;
            errorMessage = string.Empty;

            if (string.IsNullOrEmpty(account))
            {
                errorMessage = "账号不能为空";
                return false;
            }
            if (string.IsNullOrEmpty(password))
            {
                errorMessage = "密码不能为空";
                return false;
            }

            var userInfo = GetByAccount(account);
            if (string.IsNullOrEmpty(userInfo?.UserName))
            {
                errorMessage = "帐号或密码错误";
                return false;
            }

            userName = userInfo.UserName;

            if (userInfo.IsLockedOut)
            {
                errorMessage = "此账号被锁定，无法登录";
                return false;
            }

            if (_configRepository.Instance.IsAdminLockLogin)
            {
                if (userInfo.CountOfFailedLogin > 0 &&
                    userInfo.CountOfFailedLogin >= _configRepository.Instance.AdminLockLoginCount)
                {
                    var lockType = EUserLockTypeUtils.GetEnumType(_configRepository.Instance.AdminLockLoginType);
                    if (lockType == EUserLockType.Forever)
                    {
                        errorMessage = "此账号错误登录次数过多，已被永久锁定";
                        return false;
                    }
                    if (lockType == EUserLockType.Hours)
                    {
                        if (userInfo.LastActivityDate.HasValue)
                        {
                            var ts = new TimeSpan(DateTime.Now.Ticks - userInfo.LastActivityDate.Value.Ticks);
                            var hours = Convert.ToInt32(_configRepository.Instance.AdminLockLoginHours - ts.TotalHours);
                            if (hours > 0)
                            {
                                errorMessage =
                                    $"此账号错误登录次数过多，已被锁定，请等待{hours}小时后重试";
                                return false;
                            }
                        }
                    }
                }
            }

            if (CheckPassword(password, isPasswordMd5, userInfo.Password, PasswordFormat.Parse(userInfo.PasswordFormat), userInfo.PasswordSalt))
                return true;

            errorMessage = "账号或密码错误";
            return false;
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
