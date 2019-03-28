using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using SiteServer.CMS.Apis;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Core;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.Utils;
using SiteServer.Utils.Auth;
using SiteServer.Utils.Enumerations;
using Attr = SiteServer.CMS.Database.Attributes.UserAttribute;

namespace SiteServer.CMS.Database.Repositories
{
    public class UserRepository : GenericRepository<UserInfo>
    {
        private bool InsertValidate(string userName, string email, string mobile, string password, string ipAddress, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (!UserManager.IsIpAddressCached(ipAddress))
            {
                errorMessage = $"同一IP在{ConfigManager.Instance.UserRegistrationMinMinutes}分钟内只能注册一次";
                return false;
            }
            if (string.IsNullOrEmpty(password))
            {
                errorMessage = "密码不能为空";
                return false;
            }
            if (password.Length < ConfigManager.Instance.UserPasswordMinLength)
            {
                errorMessage = $"密码长度必须大于等于{ConfigManager.Instance.UserPasswordMinLength}";
                return false;
            }
            if (!EUserPasswordRestrictionUtils.IsValid(password, ConfigManager.Instance.UserPasswordRestriction))
            {
                errorMessage =
                    $"密码不符合规则，请包含{EUserPasswordRestrictionUtils.GetText(EUserPasswordRestrictionUtils.GetEnumType(ConfigManager.Instance.UserPasswordRestriction))}";
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

            if (!ConfigManager.Instance.IsUserRegistrationAllowed)
            {
                errorMessage = "对不起，系统已禁止新用户注册！";
                return 0;
            }

            try
            {
                userInfo.Checked = ConfigManager.Instance.IsUserRegistrationChecked;
                if (StringUtils.IsMobile(userInfo.UserName) && string.IsNullOrEmpty(userInfo.Mobile))
                {
                    userInfo.Mobile = userInfo.UserName;
                }

                if (!InsertValidate(userInfo.UserName, userInfo.Email, userInfo.Mobile, password, ipAddress, out errorMessage)) return 0;

                var passwordSalt = GenerateSalt();
                password = EncodePassword(password, EPasswordFormat.Encrypted, passwordSalt);
                userInfo.CreateDate = DateTime.Now;
                userInfo.LastActivityDate = DateTime.Now;
                userInfo.LastResetPasswordDate = DateTime.Now;

                userInfo.Id = InsertWithoutValidation(userInfo, password, EPasswordFormat.Encrypted, passwordSalt);

                UserManager.CacheIpAddress(ipAddress);

                return userInfo.Id;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return 0;
            }
        }

        private int InsertWithoutValidation(UserInfo userInfo, string password, EPasswordFormat passwordFormat, string passwordSalt)
        {
            //var sqlString = $"INSERT INTO {TableName} (UserName, Password, PasswordFormat, PasswordSalt, CreateDate, LastResetPasswordDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, GroupId, IsChecked, IsLockedOut, DisplayName, Email, Mobile, AvatarUrl, Gender, Birthday, WeiXin, QQ, WeiBo, Bio, SettingsXml) VALUES (@UserName, @Password, @PasswordFormat, @PasswordSalt, @CreateDate, @LastResetPasswordDate, @LastActivityDate, @CountOfLogin, @CountOfFailedLogin, @GroupId, @IsChecked, @IsLockedOut, @DisplayName, @Email, @Mobile, @AvatarUrl, @Gender, @Birthday, @WeiXin, @QQ, @WeiBo, @Bio, @SettingsXml)";

            //userInfo.CreateDate = DateTime.Now;
            //userInfo.LastActivityDate = DateTime.Now;
            //userInfo.LastResetPasswordDate = DateTime.Now;

            //userInfo.DisplayName = AttackUtils.FilterXss(userInfo.DisplayName);
            //userInfo.Email = AttackUtils.FilterXss(userInfo.Email);
            //userInfo.Mobile = AttackUtils.FilterXss(userInfo.Mobile);
            //userInfo.AvatarUrl = AttackUtils.FilterXss(userInfo.AvatarUrl);
            //userInfo.Gender = AttackUtils.FilterXss(userInfo.Gender);
            //userInfo.Birthday = AttackUtils.FilterXss(userInfo.Birthday);
            //userInfo.WeiXin = AttackUtils.FilterXss(userInfo.WeiXin);
            //userInfo.Qq = AttackUtils.FilterXss(userInfo.Qq);
            //userInfo.WeiBo = AttackUtils.FilterXss(userInfo.WeiBo);
            //userInfo.Bio = AttackUtils.FilterXss(userInfo.Bio);
            //var settingsXml = userInfo.ToString(UserAttribute.AllAttributes.Value);

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamUserName, userInfo.UserName),
            //    GetParameter(ParamPassword, password),
            //    GetParameter(ParamPasswordFormat, EPasswordFormatUtils.GetValueById(passwordFormat)),
            //    GetParameter(ParamPasswordSalt, passwordSalt),
            //    GetParameter(ParamCreateDate,userInfo.CreateDate),
            //    GetParameter(ParamLastResetPasswordDate,userInfo.LastResetPasswordDate),
            //    GetParameter(ParamLastActivityDate,userInfo.LastActivityDate),
            //    GetParameter(ParamCountOfLogin, userInfo.CountOfLogin),
            //    GetParameter(ParamCountOfFailedLogin, userInfo.CountOfFailedLogin),
            //    GetParameter(ParamGroupId, userInfo.GroupId),
            //    GetParameter(ParamIsChecked, userInfo.IsChecked.ToString()),
            //    GetParameter(ParamIsLockedOut, userInfo.IsLockedOut.ToString()),
            //    GetParameter(ParamDisplayName, userInfo.DisplayName),
            //    GetParameter(ParamEmail, userInfo.Email),
            //    GetParameter(ParamMobile, userInfo.Mobile),
            //    GetParameter(ParamAvatarUrl, userInfo.AvatarUrl),
            //    GetParameter(ParamGender, userInfo.Gender),
            //    GetParameter(ParamBirthday, userInfo.Birthday),
            //    GetParameter(ParamWeiXin, userInfo.WeiXin),
            //    GetParameter(ParamQq, userInfo.Qq),
            //    GetParameter(ParamWeiBo, userInfo.WeiBo),
            //    GetParameter(ParamBio,userInfo.Bio),
            //    GetParameter(ParamSettingsXml,settingsXml)
            //};

            //return DatabaseApi.ExecuteNonQueryAndReturnId(ConnectionString, TableName, UserAttribute.Id, sqlString, parameters);

            userInfo.Password = password;
            userInfo.PasswordFormat = EPasswordFormatUtils.GetValue(passwordFormat);
            userInfo.PasswordSalt = passwordSalt;
            userInfo.CreateDate = DateTime.Now;
            userInfo.LastActivityDate = DateTime.Now;
            userInfo.LastResetPasswordDate = DateTime.Now;

            return InsertObject(userInfo);
        }

        public bool IsPasswordCorrect(string password, out string errorMessage)
        {
            errorMessage = null;
            if (string.IsNullOrEmpty(password))
            {
                errorMessage = "密码不能为空";
                return false;
            }
            if (password.Length < ConfigManager.Instance.UserPasswordMinLength)
            {
                errorMessage = $"密码长度必须大于等于{ConfigManager.Instance.UserPasswordMinLength}";
                return false;
            }
            if (!EUserPasswordRestrictionUtils.IsValid(password, ConfigManager.Instance.UserPasswordRestriction))
            {
                errorMessage =
                    $"密码不符合规则，请包含{EUserPasswordRestrictionUtils.GetText(EUserPasswordRestrictionUtils.GetEnumType(ConfigManager.Instance.UserPasswordRestriction))}";
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

        public void Update(UserInfo userInfo)
        {
            //if (userInfo == null) return;

            //userInfo.DisplayName = AttackUtils.FilterXss(userInfo.DisplayName);
            //userInfo.Email = AttackUtils.FilterXss(userInfo.Email);
            //userInfo.Mobile = AttackUtils.FilterXss(userInfo.Mobile);
            //userInfo.AvatarUrl = AttackUtils.FilterXss(userInfo.AvatarUrl);
            //userInfo.Gender = AttackUtils.FilterXss(userInfo.Gender);
            //userInfo.Birthday = AttackUtils.FilterXss(userInfo.Birthday);
            //userInfo.WeiXin = AttackUtils.FilterXss(userInfo.WeiXin);
            //userInfo.Qq = AttackUtils.FilterXss(userInfo.Qq);
            //userInfo.WeiBo = AttackUtils.FilterXss(userInfo.WeiBo);
            //userInfo.Bio = AttackUtils.FilterXss(userInfo.Bio);

            //var sqlString = $"UPDATE {TableName} SET UserName = @UserName, CreateDate = @CreateDate, LastResetPasswordDate = @LastResetPasswordDate, LastActivityDate = @LastActivityDate, CountOfLogin = @CountOfLogin, CountOfFailedLogin = @CountOfFailedLogin, GroupId = @GroupId, IsChecked = @IsChecked, IsLockedOut = @IsLockedOut, DisplayName = @DisplayName, Email = @Email, Mobile = @Mobile, AvatarUrl = @AvatarUrl, Gender = @Gender, Birthday = @Birthday, WeiXin = @WeiXin, QQ = @QQ, WeiBo = @WeiBo, Bio = @Bio, SettingsXml = @SettingsXml WHERE Id = @Id";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamUserName, userInfo.UserName),
            //    GetParameter(ParamCreateDate,userInfo.CreateDate),
            //    GetParameter(ParamLastResetPasswordDate,userInfo.LastResetPasswordDate),
            //    GetParameter(ParamLastActivityDate,userInfo.LastActivityDate),
            //    GetParameter(ParamCountOfLogin, userInfo.CountOfLogin),
            //    GetParameter(ParamCountOfFailedLogin, userInfo.CountOfFailedLogin),
            //    GetParameter(ParamGroupId, userInfo.GroupId),
            //    GetParameter(ParamIsChecked, userInfo.IsChecked.ToString()),
            //    GetParameter(ParamIsLockedOut, userInfo.IsLockedOut.ToString()),
            //    GetParameter(ParamDisplayName, userInfo.DisplayName),
            //    GetParameter(ParamEmail, userInfo.Email),
            //    GetParameter(ParamMobile, userInfo.Mobile),
            //    GetParameter(ParamAvatarUrl, userInfo.AvatarUrl),
            //    GetParameter(ParamGender, userInfo.Gender),
            //    GetParameter(ParamBirthday, userInfo.Birthday),
            //    GetParameter(ParamWeiXin, userInfo.WeiXin),
            //    GetParameter(ParamQq, userInfo.Qq),
            //    GetParameter(ParamWeiBo, userInfo.WeiBo),
            //    GetParameter(ParamBio,userInfo.Bio),
            //    GetParameter(ParamSettingsXml,userInfo.ToString(UserAttribute.AllAttributes.Value)),
            //    GetParameter(ParamId, userInfo.Id)
            //};

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

            UpdateObject(userInfo);

            UserManager.UpdateCache(userInfo);
        }

        private void UpdateLastActivityDateAndCountOfFailedLogin(UserInfo userInfo)
        {
            if (userInfo == null) return;

            userInfo.LastActivityDate = DateTime.Now;
            userInfo.CountOfFailedLogin += 1;

            //var sqlString = $"UPDATE {TableName} SET LastActivityDate = @LastActivityDate, CountOfFailedLogin = @CountOfFailedLogin WHERE Id = @Id";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamLastActivityDate, userInfo.LastActivityDate),
            //    GetParameter(ParamCountOfFailedLogin, userInfo.CountOfFailedLogin),
            //    GetParameter(ParamId, userInfo.Id)
            //};

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

            UpdateObject(userInfo, Attr.LastActivityDate, Attr.CountOfFailedLogin);

            UserManager.UpdateCache(userInfo);
        }

        public void UpdateLastActivityDateAndCountOfLogin(UserInfo userInfo)
        {
            if (userInfo == null) return;

            userInfo.LastActivityDate = DateTime.Now;
            userInfo.CountOfLogin += 1;
            userInfo.CountOfFailedLogin = 0;

            //var sqlString = $"UPDATE {TableName} SET LastActivityDate = @LastActivityDate, CountOfLogin = @CountOfLogin, CountOfFailedLogin = @CountOfFailedLogin WHERE Id = @Id";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamLastActivityDate, userInfo.LastActivityDate),
            //    GetParameter(ParamCountOfLogin, userInfo.CountOfLogin),
            //    GetParameter(ParamCountOfFailedLogin, userInfo.CountOfFailedLogin),
            //    GetParameter(ParamId, userInfo.Id)
            //};

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

            UpdateObject(userInfo, Attr.LastActivityDate, Attr.CountOfLogin, Attr.CountOfFailedLogin);

            UserManager.UpdateCache(userInfo);
        }

        private string EncodePassword(string password, EPasswordFormat passwordFormat, string passwordSalt)
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

        private string DecodePassword(string password, EPasswordFormat passwordFormat, string passwordSalt)
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
            if (password.Length < ConfigManager.Instance.UserPasswordMinLength)
            {
                errorMessage = $"密码长度必须大于等于{ConfigManager.Instance.UserPasswordMinLength}";
                return false;
            }
            if (!EUserPasswordRestrictionUtils.IsValid(password, ConfigManager.Instance.UserPasswordRestriction))
            {
                errorMessage =
                    $"密码不符合规则，请包含{EUserPasswordRestrictionUtils.GetText(EUserPasswordRestrictionUtils.GetEnumType(ConfigManager.Instance.UserPasswordRestriction))}";
                return false;
            }

            const EPasswordFormat passwordFormat = EPasswordFormat.Encrypted;
            var passwordSalt = GenerateSalt();
            password = EncodePassword(password, passwordFormat, passwordSalt);
            ChangePassword(userName, passwordFormat, passwordSalt, password);
            return true;
        }

        private void ChangePassword(string userName, EPasswordFormat passwordFormat, string passwordSalt, string password)
        {
            var userInfo = UserManager.GetUserInfoByUserName(userName);
            if (userInfo == null) return;

            userInfo.PasswordFormat = EPasswordFormatUtils.GetValue(passwordFormat);
            userInfo.Password = password;
            userInfo.PasswordSalt = passwordSalt;
            userInfo.LastResetPasswordDate = DateTime.Now;

            //var sqlString = $"UPDATE {TableName} SET Password = @Password, PasswordFormat = @PasswordFormat, PasswordSalt = @PasswordSalt, LastResetPasswordDate = @LastResetPasswordDate WHERE UserName = @UserName";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamPassword, userInfo.Password),
            //    GetParameter(ParamPasswordFormat, userInfo.PasswordFormat),
            //    GetParameter(ParamPasswordSalt, userInfo.PasswordSalt),
            //    GetParameter(ParamLastResetPasswordDate,userInfo.LastResetPasswordDate),
            //    GetParameter(ParamUserName, userName)
            //};

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

            UpdateObject(userInfo, Attr.PasswordFormat, Attr.Password, Attr.PasswordSalt, Attr.LastResetPasswordDate);

            LogUtils.AddUserLog(userName, "修改密码", string.Empty);

            UserManager.UpdateCache(userInfo);
        }

        public void Check(List<int> idList)
        {
            //var sqlString =
            //    $"UPDATE {TableName} SET IsChecked = '{true}' WHERE Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);

            UpdateAll(Q
                .Set(Attr.IsChecked, true.ToString())
                .WhereIn(Attr.Id, idList)
            );

            UserManager.ClearCache();
        }

        public void Lock(List<int> idList)
        {
            //var sqlString =
            //    $"UPDATE {TableName} SET IsLockedOut = '{true}' WHERE Id IN ({TranslateUtils.ToSqlInStringWithQuote(idList)})";

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);

            UpdateAll(Q
                .Set(Attr.IsLockedOut, true.ToString())
                .WhereIn(Attr.Id, idList)
            );

            UserManager.ClearCache();
        }

        public void UnLock(List<int> idList)
        {
            //var sqlString =
            //    $"UPDATE {TableName} SET IsLockedOut = '{false}', CountOfFailedLogin = 0 WHERE Id IN ({TranslateUtils.ToSqlInStringWithQuote(idList)})";

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);

            UpdateAll(Q
                .Set(Attr.IsLockedOut, false.ToString())
                .WhereIn(Attr.Id, idList)
            );

            UserManager.ClearCache();
        }

        private UserInfo GetByAccount(string account)
        {
            var userInfo = GetByUserName(account);
            if (userInfo != null) return userInfo;
            if (StringUtils.IsMobile(account)) return GetByMobile(account);
            if (StringUtils.IsEmail(account)) return GetByEmail(account);

            return null;
        }

        public UserInfo GetByUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return null;

            //UserInfo userInfo = null;
            //var sqlString = $"SELECT Id, UserName, Password, PasswordFormat, PasswordSalt, CreateDate, LastResetPasswordDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, GroupId, IsChecked, IsLockedOut, DisplayName, Email, Mobile, AvatarUrl, Gender, Birthday, WeiXin, QQ, WeiBo, Bio, SettingsXml FROM {TableName} WHERE UserName = @UserName";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamUserName, userName)
            //};

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
            //{
            //    if (rdr.Read())
            //    {
            //        userInfo = new UserInfo(rdr);
            //    }
            //    rdr.Close();
            //}

            var userInfo = GetObject(Q.Where(Attr.UserName, userName));

            UserManager.UpdateCache(userInfo);

            return userInfo;
        }

        public UserInfo GetByEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return null;

            //UserInfo userInfo = null;
            //var sqlString = $"SELECT Id, UserName, Password, PasswordFormat, PasswordSalt, CreateDate, LastResetPasswordDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, GroupId, IsChecked, IsLockedOut, DisplayName, Email, Mobile, AvatarUrl, Gender, Birthday, WeiXin, QQ, WeiBo, Bio, SettingsXml FROM {TableName} WHERE Email = @Email";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamEmail, email)
            //};

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
            //{
            //    if (rdr.Read())
            //    {
            //        userInfo = new UserInfo(rdr);
            //    }
            //    rdr.Close();
            //}

            var userInfo = GetObject(Q.Where(Attr.Email, email));

            UserManager.UpdateCache(userInfo);

            return userInfo;
        }

        public UserInfo GetByMobile(string mobile)
        {
            if (string.IsNullOrEmpty(mobile)) return null;

            //UserInfo userInfo = null;
            //var sqlString = $"SELECT Id, UserName, Password, PasswordFormat, PasswordSalt, CreateDate, LastResetPasswordDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, GroupId, IsChecked, IsLockedOut, DisplayName, Email, Mobile, AvatarUrl, Gender, Birthday, WeiXin, QQ, WeiBo, Bio, SettingsXml FROM {TableName} WHERE Mobile = @Mobile";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamMobile, mobile)
            //};

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
            //{
            //    if (rdr.Read())
            //    {
            //        userInfo = new UserInfo(rdr);
            //    }
            //    rdr.Close();
            //}

            var userInfo = GetObject(Q.Where(Attr.Mobile, mobile));

            UserManager.UpdateCache(userInfo);

            return userInfo;
        }

        public UserInfo GetByUserId(int id)
        {
            if (id <= 0) return null;

            //UserInfo userInfo = null;
            //var sqlString = $"SELECT Id, UserName, Password, PasswordFormat, PasswordSalt, CreateDate, LastResetPasswordDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, GroupId, IsChecked, IsLockedOut, DisplayName, Email, Mobile, AvatarUrl, Gender, Birthday, WeiXin, QQ, WeiBo, Bio, SettingsXml FROM {TableName} WHERE Id = @Id";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamId, id)
            //};

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
            //{
            //    if (rdr.Read())
            //    {
            //        userInfo = new UserInfo(rdr);
            //    }
            //    rdr.Close();
            //}

            var userInfo = GetObjectById(id);

            UserManager.UpdateCache(userInfo);

            return userInfo;
        }

        public bool IsUserNameExists(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return false;

            //var exists = false;

            //var sqlString = $"SELECT Id FROM {TableName} WHERE UserName = @UserName";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamUserName, userName)
            //};

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
            //{
            //    if (rdr.Read() && !rdr.IsDBNull(0))
            //    {
            //        exists = true;
            //    }
            //    rdr.Close();
            //}

            //return exists;

            return Exists(Q.Where(Attr.UserName, userName));
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

            //var sqlSelect = $"SELECT Email FROM {TableName} WHERE Email = @Email";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamEmail, email)
            //};

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlSelect, parameters))
            //{
            //    if (rdr.Read())
            //    {
            //        exists = true;
            //    }
            //    rdr.Close();
            //}

            return Exists(Q.Where(Attr.Email, email));
        }

        public bool IsMobileExists(string mobile)
        {
            if (string.IsNullOrEmpty(mobile)) return false;

            var exists = IsUserNameExists(mobile);
            if (exists) return true;

            //var sqlString = $"SELECT Mobile FROM {TableName} WHERE Mobile = @Mobile";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamMobile, mobile)
            //};

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
            //{
            //    if (rdr.Read())
            //    {
            //        exists = true;
            //    }
            //    rdr.Close();
            //}

            //return exists;

            return Exists(Q.Where(Attr.Mobile, mobile));
        }

        public IList<int> GetIdList(bool isChecked)
        {
            //var idList = new List<int>();

            //var sqlSelect =
            //    $"SELECT Id FROM {TableName} WHERE IsChecked = '{isChecked}' ORDER BY Id DESC";

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlSelect))
            //{
            //    while (rdr.Read())
            //    {
            //        idList.Add(DatabaseApi.GetInt(rdr, 0));
            //    }
            //    rdr.Close();
            //}

            //return idList;

            return GetValueList<int>(Q
                .Select(Attr.Id)
                .Where(Attr.IsChecked, isChecked.ToString())
                .OrderByDesc(Attr.Id));
        }

        public string GetSelectCommand()
        {
            return DatabaseApi.Instance.GetSelectSqlString(TableName, string.Empty);
        }

        public string GetSelectCommand(int groupId, string searchWord, int dayOfCreate, int dayOfLastActivity, int loginCount, string searchType)
        {
            var whereBuilder = new StringBuilder();

            if (dayOfCreate > 0)
            {
                if (whereBuilder.Length > 0) whereBuilder.Append(" AND ");
                var dateTime = DateTime.Now.AddDays(-dayOfCreate);
                whereBuilder.Append($"(CreateDate >= {SqlUtils.GetComparableDate(dateTime)})");
            }

            if (dayOfLastActivity > 0)
            {
                if (whereBuilder.Length > 0) whereBuilder.Append(" AND ");
                var dateTime = DateTime.Now.AddDays(-dayOfLastActivity);
                whereBuilder.Append($"(LastActivityDate >= {SqlUtils.GetComparableDate(dateTime)}) ");
            }

            if (groupId > -1)
            {
                if (whereBuilder.Length > 0) whereBuilder.Append(" AND ");
                whereBuilder.Append(groupId == 0 ? "(GroupId = 0 OR GroupId IS NULL)" : $"GroupId = {groupId}");
            }

            searchWord = AttackUtils.FilterSql(searchWord);

            if (string.IsNullOrEmpty(searchType))
            {
                if (whereBuilder.Length > 0) whereBuilder.Append(" AND ");
                whereBuilder.Append(
                    $"(UserName LIKE '%{searchWord}%' OR EMAIL LIKE '%{searchWord}%')");
            }
            else
            {
                if (whereBuilder.Length > 0) whereBuilder.Append(" AND ");
                whereBuilder.Append($"({searchType} LIKE '%{searchWord}%') ");
            }

            if (loginCount > 0)
            {
                if (whereBuilder.Length > 0) whereBuilder.Append(" AND ");
                whereBuilder.Append($"(CountOfLogin > {loginCount})");
            }

            var whereString = string.Empty;
            if (whereBuilder.Length > 0)
            {
                whereString = $"WHERE {whereBuilder}";
            }

            return DatabaseApi.Instance.GetSelectSqlString(TableName, whereString);
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

            if (ConfigManager.Instance.IsUserLockLogin)
            {
                if (userInfo.CountOfFailedLogin > 0 && userInfo.CountOfFailedLogin >= ConfigManager.Instance.UserLockLoginCount)
                {
                    var lockType = EUserLockTypeUtils.GetEnumType(ConfigManager.Instance.UserLockLoginType);
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
                            var hours = Convert.ToInt32(ConfigManager.Instance.UserLockLoginHours - ts.TotalHours);
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

            if (!CheckPassword(password, isPasswordMd5, userInfo.Password, EPasswordFormatUtils.GetEnumType(userInfo.PasswordFormat), userInfo.PasswordSalt))
            {
                DataProvider.User.UpdateLastActivityDateAndCountOfFailedLogin(userInfo);
                LogUtils.AddUserLog(userInfo.UserName, "用户登录失败", "帐号或密码错误");
                errorMessage = "帐号或密码错误";
                return null;
            }

            return userInfo;
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

            using (var rdr = DatabaseApi.Instance.ExecuteReader(WebConfigUtils.ConnectionString, sqlString))
            {
                while (rdr.Read())
                {
                    var accessNum = DatabaseApi.Instance.GetInt(rdr, 0);
                    if (EStatictisXTypeUtils.Equals(xType, EStatictisXType.Day))
                    {
                        var year = DatabaseApi.Instance.GetString(rdr, 1);
                        var month = DatabaseApi.Instance.GetString(rdr, 2);
                        var day = DatabaseApi.Instance.GetString(rdr, 3);
                        var dateTime = TranslateUtils.ToDateTime($"{year}-{month}-{day}");
                        dict.Add(dateTime, accessNum);
                    }
                    else if (EStatictisXTypeUtils.Equals(xType, EStatictisXType.Month))
                    {
                        var year = DatabaseApi.Instance.GetString(rdr, 1);
                        var month = DatabaseApi.Instance.GetString(rdr, 2);

                        var dateTime = TranslateUtils.ToDateTime($"{year}-{month}-1");
                        dict.Add(dateTime, accessNum);
                    }
                    else if (EStatictisXTypeUtils.Equals(xType, EStatictisXType.Year))
                    {
                        var year = DatabaseApi.Instance.GetString(rdr, 1);
                        var dateTime = TranslateUtils.ToDateTime($"{year}-1-1");
                        dict.Add(dateTime, accessNum);
                    }
                }
                rdr.Close();
            }
            return dict;
        }

        public int GetCount()
        {
            //return DatabaseApi.GetCount(TableName);
            return Count();
        }

        public IList<UserInfo> GetUsers(int offset, int limit)
        {
            //var list = new List<UserInfo>();
            //List<int> dbList;

            //var sqlString =
            //    SqlDifferences.GetSqlString(TableName, new List<string>
            //    {
            //        nameof(UserInfo.Id)
            //    }, string.Empty, "ORDER BY Id", offset, limit);

            //using (var connection = GetConnection())
            //{
            //    dbList = connection.Query<int>(sqlString).ToList();
            //}

            //if (dbList.Count > 0)
            //{
            //    foreach (var userId in dbList)
            //    {
            //        list.Add(UserManager.GetUserInfoByUserId(userId));
            //    }
            //}

            //return list;

            return GetObjectList(Q
                .Offset(offset)
                .Limit(limit)
                .OrderBy(Attr.Id));
        }

        public bool IsExists(int id)
        {
            //var sqlString = $"SELECT COUNT(1) FROM {TableName} WHERE Id = @Id";

            //using (var connection = GetConnection())
            //{
            //    return connection.ExecuteScalar<bool>(sqlString, new { Id = id });
            //}

            return Exists(id);
        }

        public void Delete(UserInfo userInfo)
        {
            //var sqlString = $"DELETE FROM {TableName} WHERE Id = @Id";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamId, userInfo.Id)
            //};

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

            DeleteById(userInfo.Id);

            UserManager.RemoveCache(userInfo);
        }
    }
}



//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Security.Cryptography;
//using System.Text;
//using Dapper;
//using SiteServer.CMS.Core;
//using SiteServer.CMS.Core.Attributes;
//using SiteServer.CMS.Database.Caches;
//using SiteServer.CMS.Database.Core;
//using SiteServer.CMS.Database.Models;
//using SiteServer.Plugin;
//using SiteServer.Utils;
//using SiteServer.Utils.Auth;
//using SiteServer.Utils.Enumerations;

//namespace SiteServer.CMS.Database.Repositories
//{
//    public class User : DataProviderBase
//    {
//        public const string DatabaseTableName = "siteserver_User";

//        public override string TableName => DatabaseTableName;

//        public override List<TableColumn> TableColumns => new List<TableColumn>
//        {
//            new TableColumn
//            {
//                AttributeName = nameof(UserInfo.Id),
//                DataType = DataType.Integer,
//                IsIdentity = true,
//                IsPrimaryKey = true
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(UserInfo.UserName),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(UserInfo.Password),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(UserInfo.PasswordFormat),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(UserInfo.PasswordSalt),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(UserInfo.CreateDate),
//                DataType = DataType.DateTime
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(UserInfo.LastResetPasswordDate),
//                DataType = DataType.DateTime
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(UserInfo.LastActivityDate),
//                DataType = DataType.DateTime
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(UserInfo.CountOfLogin),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(UserInfo.CountOfFailedLogin),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(UserInfo.GroupId),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(UserInfo.IsChecked),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(UserInfo.IsLockedOut),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(UserInfo.DisplayName),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(UserInfo.Email),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(UserInfo.Mobile),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(UserInfo.AvatarUrl),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(UserInfo.Gender),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(UserInfo.Birthday),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(UserInfo.WeiXin),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(UserInfo.Qq),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(UserInfo.WeiBo),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(UserInfo.Bio),
//                DataType = DataType.Text
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(UserInfo.SettingsXml),
//                DataType = DataType.Text
//            }
//        };

//        private const string ParamId = "@Id";
//        private const string ParamUserName = "@UserName";
//        private const string ParamPassword = "@Password";
//        private const string ParamPasswordFormat = "@PasswordFormat";
//        private const string ParamPasswordSalt = "@PasswordSalt";
//        private const string ParamCreateDate = "@CreateDate";
//        private const string ParamLastResetPasswordDate = "@LastResetPasswordDate";
//        private const string ParamLastActivityDate = "@LastActivityDate";
//        private const string ParamCountOfLogin = "@CountOfLogin";
//        private const string ParamCountOfFailedLogin = "@CountOfFailedLogin";
//        private const string ParamGroupId = "@GroupId";
//        private const string ParamIsChecked = "@IsChecked";
//        private const string ParamIsLockedOut = "@IsLockedOut";
//        private const string ParamDisplayName = "@DisplayName";
//        private const string ParamEmail = "@Email";
//        private const string ParamMobile = "@Mobile";
//        private const string ParamAvatarUrl = "@AvatarUrl";
//        private const string ParamGender = "@Gender";
//        private const string ParamBirthday = "@Birthday";
//        private const string ParamWeiXin = "@WeiXin";
//        private const string ParamQq = "@QQ";
//        private const string ParamWeiBo = "@WeiBo";
//        private const string ParamBio = "@Bio";
//        private const string ParamSettingsXml = "@SettingsXml";

//        private bool InsertValidate(string userName, string email, string mobile, string password, string ipAddress, out string errorMessage)
//        {
//            errorMessage = string.Empty;

//            if (!UserManager.IsIpAddressCached(ipAddress))
//            {
//                errorMessage = $"同一IP在{ConfigManager.Instance.UserRegistrationMinMinutes}分钟内只能注册一次";
//                return false;
//            }
//            if (string.IsNullOrEmpty(password))
//            {
//                errorMessage = "密码不能为空";
//                return false;
//            }
//            if (password.Length < ConfigManager.Instance.UserPasswordMinLength)
//            {
//                errorMessage = $"密码长度必须大于等于{ConfigManager.Instance.UserPasswordMinLength}";
//                return false;
//            }
//            if (!EUserPasswordRestrictionUtils.IsValid(password, ConfigManager.Instance.UserPasswordRestriction))
//            {
//                errorMessage =
//                    $"密码不符合规则，请包含{EUserPasswordRestrictionUtils.GetText(EUserPasswordRestrictionUtils.GetEnumType(ConfigManager.Instance.UserPasswordRestriction))}";
//                return false;
//            }
//            if (string.IsNullOrEmpty(userName))
//            {
//                errorMessage = "用户名为空，请填写用户名";
//                return false;
//            }
//            if (!string.IsNullOrEmpty(userName) && IsUserNameExists(userName))
//            {
//                errorMessage = "用户名已被注册，请更换用户名";
//                return false;
//            }
//            if (!IsUserNameCompliant(userName.Replace("@", string.Empty).Replace(".", string.Empty)))
//            {
//                errorMessage = "用户名包含不规则字符，请更换用户名";
//                return false;
//            }

//            if (!string.IsNullOrEmpty(email) && IsEmailExists(email))
//            {
//                errorMessage = "电子邮件地址已被注册，请更换邮箱";
//                return false;
//            }
//            if (!string.IsNullOrEmpty(mobile) && IsMobileExists(mobile))
//            {
//                errorMessage = "手机号码已被注册，请更换手机号码";
//                return false;
//            }

//            return true;
//        }

//        private bool UpdateValidate(Dictionary<string, object> body, string userName, string email, string mobile, out string errorMessage)
//        {
//            errorMessage = string.Empty;

//            var bodyUserName = string.Empty;
//            if (body.ContainsKey("userName"))
//            {
//                bodyUserName = (string) body["userName"];
//            }

//            if (!string.IsNullOrEmpty(bodyUserName) && bodyUserName != userName)
//            {
//                if (!IsUserNameCompliant(bodyUserName.Replace("@", string.Empty).Replace(".", string.Empty)))
//                {
//                    errorMessage = "用户名包含不规则字符，请更换用户名";
//                    return false;
//                }
//                if (!string.IsNullOrEmpty(bodyUserName) && IsUserNameExists(bodyUserName))
//                {
//                    errorMessage = "用户名已被注册，请更换用户名";
//                    return false;
//                }
//            }

//            var bodyEmail = string.Empty;
//            if (body.ContainsKey("email"))
//            {
//                bodyEmail = (string)body["email"];
//            }

//            if (bodyEmail != null && bodyEmail != email)
//            {
//                if (!string.IsNullOrEmpty(bodyEmail) && IsEmailExists(bodyEmail))
//                {
//                    errorMessage = "电子邮件地址已被注册，请更换邮箱";
//                    return false;
//                }
//            }

//            var bodyMobile = string.Empty;
//            if (body.ContainsKey("mobile"))
//            {
//                bodyMobile = (string)body["mobile"];
//            }

//            if (bodyMobile != null && bodyMobile != mobile)
//            {
//                if (!string.IsNullOrEmpty(bodyMobile) && IsMobileExists(bodyMobile))
//                {
//                    errorMessage = "手机号码已被注册，请更换手机号码";
//                    return false;
//                }
//            }

//            return true;
//        }

//        public int InsertObject(UserInfo userInfo, string password, string ipAddress, out string errorMessage)
//        {
//            errorMessage = string.Empty;
//            if (userInfo == null) return 0;

//            if (!ConfigManager.Instance.IsUserRegistrationAllowed)
//            {
//                errorMessage = "对不起，系统已禁止新用户注册！";
//                return 0;
//            }

//            try
//            {
//                userInfo.IsChecked = ConfigManager.Instance.IsUserRegistrationChecked;
//                if (StringUtils.IsMobile(userInfo.UserName) && string.IsNullOrEmpty(userInfo.Mobile))
//                {
//                    userInfo.Mobile = userInfo.UserName;
//                }

//                if (!InsertValidate(userInfo.UserName, userInfo.Email, userInfo.Mobile, password, ipAddress, out errorMessage)) return 0;

//                var passwordSalt = GenerateSalt();
//                password = EncodePassword(password, EPasswordFormat.Encrypted, passwordSalt);
//                userInfo.CreateDate = DateTime.Now;
//                userInfo.LastActivityDate = DateTime.Now;
//                userInfo.LastResetPasswordDate = DateTime.Now;

//                userInfo.Id = InsertWithoutValidation(userInfo, password, EPasswordFormat.Encrypted, passwordSalt);

//                UserManager.CacheIpAddress(ipAddress);

//                return userInfo.Id;
//            }
//            catch (Exception ex)
//            {
//                errorMessage = ex.Message;
//                return 0;
//            }
//        }

//        private int InsertWithoutValidation(UserInfo userInfo, string password, EPasswordFormat passwordFormat, string passwordSalt)
//        {
//            var sqlString = $"INSERT INTO {TableName} (UserName, Password, PasswordFormat, PasswordSalt, CreateDate, LastResetPasswordDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, GroupId, IsChecked, IsLockedOut, DisplayName, Email, Mobile, AvatarUrl, Gender, Birthday, WeiXin, QQ, WeiBo, Bio, SettingsXml) VALUES (@UserName, @Password, @PasswordFormat, @PasswordSalt, @CreateDate, @LastResetPasswordDate, @LastActivityDate, @CountOfLogin, @CountOfFailedLogin, @GroupId, @IsChecked, @IsLockedOut, @DisplayName, @Email, @Mobile, @AvatarUrl, @Gender, @Birthday, @WeiXin, @QQ, @WeiBo, @Bio, @SettingsXml)";

//            userInfo.CreateDate = DateTime.Now;
//            userInfo.LastActivityDate = DateTime.Now;
//            userInfo.LastResetPasswordDate = DateTime.Now;

//            userInfo.DisplayName = AttackUtils.FilterXss(userInfo.DisplayName);
//            userInfo.Email = AttackUtils.FilterXss(userInfo.Email);
//            userInfo.Mobile = AttackUtils.FilterXss(userInfo.Mobile);
//            userInfo.AvatarUrl = AttackUtils.FilterXss(userInfo.AvatarUrl);
//            userInfo.Gender = AttackUtils.FilterXss(userInfo.Gender);
//            userInfo.Birthday = AttackUtils.FilterXss(userInfo.Birthday);
//            userInfo.WeiXin = AttackUtils.FilterXss(userInfo.WeiXin);
//            userInfo.Qq = AttackUtils.FilterXss(userInfo.Qq);
//            userInfo.WeiBo = AttackUtils.FilterXss(userInfo.WeiBo);
//            userInfo.Bio = AttackUtils.FilterXss(userInfo.Bio);
//            var settingsXml = userInfo.ToString(UserAttribute.AllAttributes.Value);

//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamUserName, userInfo.UserName),
//                GetParameter(ParamPassword, password),
//                GetParameter(ParamPasswordFormat, EPasswordFormatUtils.GetValueById(passwordFormat)),
//                GetParameter(ParamPasswordSalt, passwordSalt),
//                GetParameter(ParamCreateDate,userInfo.CreateDate),
//                GetParameter(ParamLastResetPasswordDate,userInfo.LastResetPasswordDate),
//                GetParameter(ParamLastActivityDate,userInfo.LastActivityDate),
//                GetParameter(ParamCountOfLogin, userInfo.CountOfLogin),
//                GetParameter(ParamCountOfFailedLogin, userInfo.CountOfFailedLogin),
//                GetParameter(ParamGroupId, userInfo.GroupId),
//                GetParameter(ParamIsChecked, userInfo.IsChecked.ToString()),
//                GetParameter(ParamIsLockedOut, userInfo.IsLockedOut.ToString()),
//                GetParameter(ParamDisplayName, userInfo.DisplayName),
//                GetParameter(ParamEmail, userInfo.Email),
//                GetParameter(ParamMobile, userInfo.Mobile),
//                GetParameter(ParamAvatarUrl, userInfo.AvatarUrl),
//                GetParameter(ParamGender, userInfo.Gender),
//                GetParameter(ParamBirthday, userInfo.Birthday),
//                GetParameter(ParamWeiXin, userInfo.WeiXin),
//                GetParameter(ParamQq, userInfo.Qq),
//                GetParameter(ParamWeiBo, userInfo.WeiBo),
//                GetParameter(ParamBio,userInfo.Bio),
//                GetParameter(ParamSettingsXml,settingsXml)
//            };

//            return DatabaseApi.ExecuteNonQueryAndReturnId(ConnectionString, TableName, UserAttribute.Id, sqlString, parameters);
//        }

//        public bool IsPasswordCorrect(string password, out string errorMessage)
//        {
//            errorMessage = null;
//            if (string.IsNullOrEmpty(password))
//            {
//                errorMessage = "密码不能为空";
//                return false;
//            }
//            if (password.Length < ConfigManager.Instance.UserPasswordMinLength)
//            {
//                errorMessage = $"密码长度必须大于等于{ConfigManager.Instance.UserPasswordMinLength}";
//                return false;
//            }
//            if (!EUserPasswordRestrictionUtils.IsValid(password, ConfigManager.Instance.UserPasswordRestriction))
//            {
//                errorMessage =
//                    $"密码不符合规则，请包含{EUserPasswordRestrictionUtils.GetText(EUserPasswordRestrictionUtils.GetEnumType(ConfigManager.Instance.UserPasswordRestriction))}";
//                return false;
//            }
//            return true;
//        }

//        public UserInfo UpdateObject(UserInfo userInfo, Dictionary<string, object> body, out string errorMessage)
//        {
//            if (!UpdateValidate(body, userInfo.UserName, userInfo.Email, userInfo.Mobile, out errorMessage)) return null;

//            userInfo.Load(body);

//            UpdateObject(userInfo);

//            return userInfo;
//        }

//        public void UpdateObject(UserInfo userInfo)
//        {
//            if (userInfo == null) return;

//            userInfo.DisplayName = AttackUtils.FilterXss(userInfo.DisplayName);
//            userInfo.Email = AttackUtils.FilterXss(userInfo.Email);
//            userInfo.Mobile = AttackUtils.FilterXss(userInfo.Mobile);
//            userInfo.AvatarUrl = AttackUtils.FilterXss(userInfo.AvatarUrl);
//            userInfo.Gender = AttackUtils.FilterXss(userInfo.Gender);
//            userInfo.Birthday = AttackUtils.FilterXss(userInfo.Birthday);
//            userInfo.WeiXin = AttackUtils.FilterXss(userInfo.WeiXin);
//            userInfo.Qq = AttackUtils.FilterXss(userInfo.Qq);
//            userInfo.WeiBo = AttackUtils.FilterXss(userInfo.WeiBo);
//            userInfo.Bio = AttackUtils.FilterXss(userInfo.Bio);

//            var sqlString = $"UPDATE {TableName} SET UserName = @UserName, CreateDate = @CreateDate, LastResetPasswordDate = @LastResetPasswordDate, LastActivityDate = @LastActivityDate, CountOfLogin = @CountOfLogin, CountOfFailedLogin = @CountOfFailedLogin, GroupId = @GroupId, IsChecked = @IsChecked, IsLockedOut = @IsLockedOut, DisplayName = @DisplayName, Email = @Email, Mobile = @Mobile, AvatarUrl = @AvatarUrl, Gender = @Gender, Birthday = @Birthday, WeiXin = @WeiXin, QQ = @QQ, WeiBo = @WeiBo, Bio = @Bio, SettingsXml = @SettingsXml WHERE Id = @Id";

//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamUserName, userInfo.UserName),
//                GetParameter(ParamCreateDate,userInfo.CreateDate),
//                GetParameter(ParamLastResetPasswordDate,userInfo.LastResetPasswordDate),
//                GetParameter(ParamLastActivityDate,userInfo.LastActivityDate),
//                GetParameter(ParamCountOfLogin, userInfo.CountOfLogin),
//                GetParameter(ParamCountOfFailedLogin, userInfo.CountOfFailedLogin),
//                GetParameter(ParamGroupId, userInfo.GroupId),
//                GetParameter(ParamIsChecked, userInfo.IsChecked.ToString()),
//                GetParameter(ParamIsLockedOut, userInfo.IsLockedOut.ToString()),
//                GetParameter(ParamDisplayName, userInfo.DisplayName),
//                GetParameter(ParamEmail, userInfo.Email),
//                GetParameter(ParamMobile, userInfo.Mobile),
//                GetParameter(ParamAvatarUrl, userInfo.AvatarUrl),
//                GetParameter(ParamGender, userInfo.Gender),
//                GetParameter(ParamBirthday, userInfo.Birthday),
//                GetParameter(ParamWeiXin, userInfo.WeiXin),
//                GetParameter(ParamQq, userInfo.Qq),
//                GetParameter(ParamWeiBo, userInfo.WeiBo),
//                GetParameter(ParamBio,userInfo.Bio),
//                GetParameter(ParamSettingsXml,userInfo.ToString(UserAttribute.AllAttributes.Value)),
//                GetParameter(ParamId, userInfo.Id)
//            };

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

//            UserManager.UpdateCache(userInfo);
//        }

//        private void UpdateLastActivityDateAndCountOfFailedLogin(UserInfo userInfo)
//        {
//            if (userInfo == null) return;

//            userInfo.LastActivityDate = DateTime.Now;
//            userInfo.CountOfFailedLogin += 1;

//            var sqlString = $"UPDATE {TableName} SET LastActivityDate = @LastActivityDate, CountOfFailedLogin = @CountOfFailedLogin WHERE Id = @Id";

//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamLastActivityDate, userInfo.LastActivityDate),
//                GetParameter(ParamCountOfFailedLogin, userInfo.CountOfFailedLogin),
//                GetParameter(ParamId, userInfo.Id)
//            };

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

//            UserManager.UpdateCache(userInfo);
//        }

//        public void UpdateLastActivityDateAndCountOfLogin(UserInfo userInfo)
//        {
//            if (userInfo == null) return;

//            userInfo.LastActivityDate = DateTime.Now;
//            userInfo.CountOfLogin += 1;
//            userInfo.CountOfFailedLogin = 0;

//            var sqlString = $"UPDATE {TableName} SET LastActivityDate = @LastActivityDate, CountOfLogin = @CountOfLogin, CountOfFailedLogin = @CountOfFailedLogin WHERE Id = @Id";

//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamLastActivityDate, userInfo.LastActivityDate),
//                GetParameter(ParamCountOfLogin, userInfo.CountOfLogin),
//                GetParameter(ParamCountOfFailedLogin, userInfo.CountOfFailedLogin),
//                GetParameter(ParamId, userInfo.Id)
//            };

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

//            UserManager.UpdateCache(userInfo);
//        }

//        private string EncodePassword(string password, EPasswordFormat passwordFormat, string passwordSalt)
//        {
//            var retVal = string.Empty;

//            if (passwordFormat == EPasswordFormat.Clear)
//            {
//                retVal = password;
//            }
//            else if (passwordFormat == EPasswordFormat.Hashed)
//            {
//                var src = Encoding.Unicode.GetBytes(password);
//                var buffer2 = Convert.FromBase64String(passwordSalt);
//                var dst = new byte[buffer2.Length + src.Length];
//                byte[] inArray = null;
//                Buffer.BlockCopy(buffer2, 0, dst, 0, buffer2.Length);
//                Buffer.BlockCopy(src, 0, dst, buffer2.Length, src.Length);
//                var algorithm = HashAlgorithm.Create("SHA1");
//                if (algorithm != null) inArray = algorithm.ComputeHash(dst);

//                if (inArray != null) retVal = Convert.ToBase64String(inArray);
//            }
//            else if (passwordFormat == EPasswordFormat.Encrypted)
//            {
//                var encrypt = new DesEncryptor
//                {
//                    InputString = password,
//                    EncryptKey = passwordSalt
//                };
//                encrypt.DesEncrypt();

//                retVal = encrypt.OutString;
//            }
//            return retVal;
//        }

//        private string DecodePassword(string password, EPasswordFormat passwordFormat, string passwordSalt)
//        {
//            var retVal = string.Empty;
//            if (passwordFormat == EPasswordFormat.Clear)
//            {
//                retVal = password;
//            }
//            else if (passwordFormat == EPasswordFormat.Hashed)
//            {
//                throw new Exception("can not decode hashed password");
//            }
//            else if (passwordFormat == EPasswordFormat.Encrypted)
//            {
//                var encrypt = new DesEncryptor
//                {
//                    InputString = password,
//                    DecryptKey = passwordSalt
//                };
//                encrypt.DesDecrypt();

//                retVal = encrypt.OutString;
//            }
//            return retVal;
//        }

//        private static string GenerateSalt()
//        {
//            var data = new byte[0x10];
//            new RNGCryptoServiceProvider().GetBytes(data);
//            return Convert.ToBase64String(data);
//        }

//        public bool ChangePassword(string userName, string password, out string errorMessage)
//        {
//            errorMessage = null;
//            if (password.Length < ConfigManager.Instance.UserPasswordMinLength)
//            {
//                errorMessage = $"密码长度必须大于等于{ConfigManager.Instance.UserPasswordMinLength}";
//                return false;
//            }
//            if (!EUserPasswordRestrictionUtils.IsValid(password, ConfigManager.Instance.UserPasswordRestriction))
//            {
//                errorMessage =
//                    $"密码不符合规则，请包含{EUserPasswordRestrictionUtils.GetText(EUserPasswordRestrictionUtils.GetEnumType(ConfigManager.Instance.UserPasswordRestriction))}";
//                return false;
//            }

//            var passwordFormat = EPasswordFormat.Encrypted;
//            var passwordSalt = GenerateSalt();
//            password = EncodePassword(password, passwordFormat, passwordSalt);
//            ChangePassword(userName, passwordFormat, passwordSalt, password);
//            return true;
//        }

//        private void ChangePassword(string userName, EPasswordFormat passwordFormat, string passwordSalt, string password)
//        {
//            var userInfo = UserManager.GetUserInfoByUserName(userName);
//            if (userInfo == null) return;

//            userInfo.PasswordFormat = EPasswordFormatUtils.GetValueById(passwordFormat);
//            userInfo.Password = password;
//            userInfo.PasswordSalt = passwordSalt;
//            userInfo.LastResetPasswordDate = DateTime.Now;

//            var sqlString = $"UPDATE {TableName} SET Password = @Password, PasswordFormat = @PasswordFormat, PasswordSalt = @PasswordSalt, LastResetPasswordDate = @LastResetPasswordDate WHERE UserName = @UserName";

//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamPassword, userInfo.Password),
//                GetParameter(ParamPasswordFormat, userInfo.PasswordFormat),
//                GetParameter(ParamPasswordSalt, userInfo.PasswordSalt),
//                GetParameter(ParamLastResetPasswordDate,userInfo.LastResetPasswordDate),
//                GetParameter(ParamUserName, userName)
//            };

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
//            LogUtils.AddUserLog(userName, "修改密码", string.Empty);

//            UserManager.UpdateCache(userInfo);
//        }

//        public void Check(List<int> idList)
//        {
//            var sqlString =
//                $"UPDATE {TableName} SET IsChecked = '{true}' WHERE Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);

//            UserManager.ClearCache();
//        }

//        public void Lock(List<int> idList)
//        {
//            var sqlString =
//                $"UPDATE {TableName} SET IsLockedOut = '{true}' WHERE Id IN ({TranslateUtils.ToSqlInStringWithQuote(idList)})";

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);

//            UserManager.ClearCache();
//        }

//        public void UnLock(List<int> idList)
//        {
//            var sqlString =
//                $"UPDATE {TableName} SET IsLockedOut = '{false}', CountOfFailedLogin = 0 WHERE Id IN ({TranslateUtils.ToSqlInStringWithQuote(idList)})";

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);

//            UserManager.ClearCache();
//        }

//        private UserInfo GetByAccount(string account)
//        {
//            var userInfo = GetByUserName(account);
//            if (userInfo != null) return userInfo;
//            if (StringUtils.IsMobile(account)) return GetByMobile(account);
//            if (StringUtils.IsEmail(account)) return GetByEmail(account);

//            return null;
//        }

//        public UserInfo GetByUserName(string userName)
//        {
//            if (string.IsNullOrEmpty(userName)) return null;

//            UserInfo userInfo = null;
//            var sqlString = $"SELECT Id, UserName, Password, PasswordFormat, PasswordSalt, CreateDate, LastResetPasswordDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, GroupId, IsChecked, IsLockedOut, DisplayName, Email, Mobile, AvatarUrl, Gender, Birthday, WeiXin, QQ, WeiBo, Bio, SettingsXml FROM {TableName} WHERE UserName = @UserName";

//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamUserName, userName)
//            };

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
//            {
//                if (rdr.Read())
//                {
//                    userInfo = new UserInfo(rdr);
//                }
//                rdr.Close();
//            }

//            UserManager.UpdateCache(userInfo);

//            return userInfo;
//        }

//        public UserInfo GetByEmail(string email)
//        {
//            if (string.IsNullOrEmpty(email)) return null;

//            UserInfo userInfo = null;
//            var sqlString = $"SELECT Id, UserName, Password, PasswordFormat, PasswordSalt, CreateDate, LastResetPasswordDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, GroupId, IsChecked, IsLockedOut, DisplayName, Email, Mobile, AvatarUrl, Gender, Birthday, WeiXin, QQ, WeiBo, Bio, SettingsXml FROM {TableName} WHERE Email = @Email";

//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamEmail, email)
//            };

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
//            {
//                if (rdr.Read())
//                {
//                    userInfo = new UserInfo(rdr);
//                }
//                rdr.Close();
//            }

//            UserManager.UpdateCache(userInfo);

//            return userInfo;
//        }

//        public UserInfo GetByMobile(string mobile)
//        {
//            if (string.IsNullOrEmpty(mobile)) return null;

//            UserInfo userInfo = null;
//            var sqlString = $"SELECT Id, UserName, Password, PasswordFormat, PasswordSalt, CreateDate, LastResetPasswordDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, GroupId, IsChecked, IsLockedOut, DisplayName, Email, Mobile, AvatarUrl, Gender, Birthday, WeiXin, QQ, WeiBo, Bio, SettingsXml FROM {TableName} WHERE Mobile = @Mobile";

//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamMobile, mobile)
//            };

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
//            {
//                if (rdr.Read())
//                {
//                    userInfo = new UserInfo(rdr);
//                }
//                rdr.Close();
//            }

//            UserManager.UpdateCache(userInfo);

//            return userInfo;
//        }

//        public UserInfo GetByUserId(int id)
//        {
//            if (id <= 0) return null;

//            UserInfo userInfo = null;
//            var sqlString = $"SELECT Id, UserName, Password, PasswordFormat, PasswordSalt, CreateDate, LastResetPasswordDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, GroupId, IsChecked, IsLockedOut, DisplayName, Email, Mobile, AvatarUrl, Gender, Birthday, WeiXin, QQ, WeiBo, Bio, SettingsXml FROM {TableName} WHERE Id = @Id";

//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamId, id)
//            };

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
//            {
//                if (rdr.Read())
//                {
//                    userInfo = new UserInfo(rdr);
//                }
//                rdr.Close();
//            }

//            UserManager.UpdateCache(userInfo);

//            return userInfo;
//        }

//        public bool IsUserNameExists(string userName)
//        {
//            if (string.IsNullOrEmpty(userName)) return false;

//            var exists = false;

//            var sqlString = $"SELECT Id FROM {TableName} WHERE UserName = @UserName";

//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamUserName, userName)
//            };

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
//            {
//                if (rdr.Read() && !rdr.IsDBNull(0))
//                {
//                    exists = true;
//                }
//                rdr.Close();
//            }

//            return exists;
//        }

//        private bool IsUserNameCompliant(string userName)
//        {
//            if (userName.IndexOf("　", StringComparison.Ordinal) != -1 || userName.IndexOf(" ", StringComparison.Ordinal) != -1 || userName.IndexOf("'", StringComparison.Ordinal) != -1 || userName.IndexOf(":", StringComparison.Ordinal) != -1 || userName.IndexOf(".", StringComparison.Ordinal) != -1)
//            {
//                return false;
//            }
//            return DirectoryUtils.IsDirectoryNameCompliant(userName);
//        }

//        public bool IsEmailExists(string email)
//        {
//            if (string.IsNullOrEmpty(email)) return false;

//            var exists = IsUserNameExists(email);
//            if (exists) return true;

//            var sqlSelect = $"SELECT Email FROM {TableName} WHERE Email = @Email";

//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamEmail, email)
//            };

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlSelect, parameters))
//            {
//                if (rdr.Read())
//                {
//                    exists = true;
//                }
//                rdr.Close();
//            }

//            return exists;
//        }

//        public bool IsMobileExists(string mobile)
//        {
//            if (string.IsNullOrEmpty(mobile)) return false;

//            var exists = IsUserNameExists(mobile);
//            if (exists) return true;

//            var sqlString = $"SELECT Mobile FROM {TableName} WHERE Mobile = @Mobile";

//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamMobile, mobile)
//            };

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
//            {
//                if (rdr.Read())
//                {
//                    exists = true;
//                }
//                rdr.Close();
//            }

//            return exists;
//        }

//        public List<int> GetIdList(bool isChecked)
//        {
//            var idList = new List<int>();

//            var sqlSelect =
//                $"SELECT Id FROM {TableName} WHERE IsChecked = '{isChecked}' ORDER BY Id DESC";

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlSelect))
//            {
//                while (rdr.Read())
//                {
//                    idList.Add(DatabaseApi.GetInt(rdr, 0));
//                }
//                rdr.Close();
//            }

//            return idList;
//        }

//        public string GetSelectCommand()
//        {
//            return DatabaseApi.Instance.GetSelectSqlString(TableName, string.Empty);
//        }

//        public string GetSelectCommand(int groupId, string searchWord, int dayOfCreate, int dayOfLastActivity, int loginCount, string searchType)
//        {
//            var whereBuilder = new StringBuilder();

//            if (dayOfCreate > 0)
//            {
//                if (whereBuilder.Length > 0) whereBuilder.Append(" AND ");
//                var dateTime = DateTime.Now.AddDays(-dayOfCreate);
//                whereBuilder.Append($"(CreateDate >= {SqlUtils.GetComparableDate(dateTime)})");
//            }

//            if (dayOfLastActivity > 0)
//            {
//                if (whereBuilder.Length > 0) whereBuilder.Append(" AND ");
//                var dateTime = DateTime.Now.AddDays(-dayOfLastActivity);
//                whereBuilder.Append($"(LastActivityDate >= {SqlUtils.GetComparableDate(dateTime)}) ");
//            }

//            if (groupId > -1)
//            {
//                if (whereBuilder.Length > 0) whereBuilder.Append(" AND ");
//                whereBuilder.Append(groupId == 0 ? "(GroupId = 0 OR GroupId IS NULL)" : $"GroupId = {groupId}");
//            }

//            searchWord = AttackUtils.FilterSql(searchWord);

//            if (string.IsNullOrEmpty(searchType))
//            {
//                if (whereBuilder.Length > 0) whereBuilder.Append(" AND ");
//                whereBuilder.Append(
//                    $"(UserName LIKE '%{searchWord}%' OR EMAIL LIKE '%{searchWord}%')");
//            }
//            else
//            {
//                if (whereBuilder.Length > 0) whereBuilder.Append(" AND ");
//                whereBuilder.Append($"({searchType} LIKE '%{searchWord}%') ");
//            }

//            if (loginCount > 0)
//            {
//                if (whereBuilder.Length > 0) whereBuilder.Append(" AND ");
//                whereBuilder.Append($"(CountOfLogin > {loginCount})");
//            }

//            var whereString = string.Empty;
//            if (whereBuilder.Length > 0)
//            {
//                whereString = $"WHERE {whereBuilder}";
//            }

//            return DatabaseApi.Instance.GetSelectSqlString(TableName, whereString);
//        }

//        public bool CheckPassword(string password, bool isPasswordMd5, string dbPassword, EPasswordFormat passwordFormat, string passwordSalt)
//        {
//            var decodePassword = DecodePassword(dbPassword, passwordFormat, passwordSalt);
//            if (isPasswordMd5)
//            {
//                return password == AuthUtils.Md5ByString(decodePassword);
//            }
//            return password == decodePassword;
//        }

//        public UserInfo Validate(string account, string password, bool isPasswordMd5, out string userName, out string errorMessage)
//        {
//            userName = string.Empty;
//            errorMessage = string.Empty;

//            if (string.IsNullOrEmpty(account))
//            {
//                errorMessage = "账号不能为空";
//                return null;
//            }
//            if (string.IsNullOrEmpty(password))
//            {
//                errorMessage = "密码不能为空";
//                return null;
//            }

//            var userInfo = GetByAccount(account);

//            if (string.IsNullOrEmpty(userInfo?.UserName))
//            {
//                errorMessage = "帐号或密码错误";
//                return null;
//            }

//            userName = userInfo.UserName;

//            if (!userInfo.IsChecked)
//            {
//                errorMessage = "此账号未审核，无法登录";
//                return null;
//            }

//            if (userInfo.IsLockedOut)
//            {
//                errorMessage = "此账号被锁定，无法登录";
//                return null;
//            }

//            if (ConfigManager.Instance.IsUserLockLogin)
//            {
//                if (userInfo.CountOfFailedLogin > 0 && userInfo.CountOfFailedLogin >= ConfigManager.Instance.UserLockLoginCount)
//                {
//                    var lockType = EUserLockTypeUtils.GetEnumType(ConfigManager.Instance.UserLockLoginType);
//                    if (lockType == EUserLockType.Forever)
//                    {
//                        errorMessage = "此账号错误登录次数过多，已被永久锁定";
//                        return null;
//                    }
//                    if (lockType == EUserLockType.Hours)
//                    {
//                        var ts = new TimeSpan(DateTime.Now.Ticks - userInfo.LastActivityDate.Ticks);
//                        var hours = Convert.ToInt32(ConfigManager.Instance.UserLockLoginHours - ts.TotalHours);
//                        if (hours > 0)
//                        {
//                            errorMessage =
//                                $"此账号错误登录次数过多，已被锁定，请等待{hours}小时后重试";
//                            return null;
//                        }
//                    }
//                }
//            }

//            if (!CheckPassword(password, isPasswordMd5, userInfo.Password, EPasswordFormatUtils.GetEnumType(userInfo.PasswordFormat), userInfo.PasswordSalt))
//            {
//                DataProvider.User.UpdateLastActivityDateAndCountOfFailedLogin(userInfo);
//                LogUtils.AddUserLog(userInfo.UserName, "用户登录失败", "帐号或密码错误");
//                errorMessage = "帐号或密码错误";
//                return null;
//            }

//            return userInfo;
//        }

//        public Dictionary<DateTime, int> GetTrackingDictionary(DateTime dateFrom, DateTime dateTo, string xType)
//        {
//            var dict = new Dictionary<DateTime, int>();
//            if (string.IsNullOrEmpty(xType))
//            {
//                xType = EStatictisXTypeUtils.GetValueById(EStatictisXType.Day);
//            }

//            var builder = new StringBuilder();
//            builder.Append($" AND CreateDate >= {SqlUtils.GetComparableDate(dateFrom)}");
//            builder.Append($" AND CreateDate < {SqlUtils.GetComparableDate(dateTo)}");

//            string sqlString = $@"
//SELECT COUNT(*) AS AddNum, AddYear, AddMonth, AddDay FROM (
//    SELECT {SqlUtils.GetDatePartYear("CreateDate")} AS AddYear, {SqlUtils.GetDatePartMonth("CreateDate")} AS AddMonth, {SqlUtils.GetDatePartDay("CreateDate")} AS AddDay 
//    FROM {TableName} 
//    WHERE {SqlUtils.GetDateDiffLessThanDays("CreateDate", 30.ToString())} {builder}
//) DERIVEDTBL GROUP BY AddYear, AddMonth, AddDay ORDER BY AddYear, AddMonth, AddDay
//";//添加日统计

//            if (EStatictisXTypeUtils.Equals(xType, EStatictisXType.Month))
//            {
//                sqlString = $@"
//SELECT COUNT(*) AS AddNum, AddYear, AddMonth FROM (
//    SELECT {SqlUtils.GetDatePartYear("CreateDate")} AS AddYear, {SqlUtils.GetDatePartMonth("CreateDate")} AS AddMonth 
//    FROM {TableName} 
//    WHERE {SqlUtils.GetDateDiffLessThanMonths("CreateDate", 12.ToString())} {builder}
//) DERIVEDTBL GROUP BY AddYear, AddMonth ORDER BY AddYear, AddMonth
//";//添加月统计
//            }
//            else if (EStatictisXTypeUtils.Equals(xType, EStatictisXType.Year))
//            {
//                sqlString = $@"
//SELECT COUNT(*) AS AddNum, AddYear FROM (
//    SELECT {SqlUtils.GetDatePartYear("CreateDate")} AS AddYear
//    FROM {TableName} 
//    WHERE {SqlUtils.GetDateDiffLessThanYears("CreateDate", 10.ToString())} {builder}
//) DERIVEDTBL GROUP BY AddYear ORDER BY AddYear
//";//添加年统计
//            }

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
//            {
//                while (rdr.Read())
//                {
//                    var accessNum = DatabaseApi.GetInt(rdr, 0);
//                    if (EStatictisXTypeUtils.Equals(xType, EStatictisXType.Day))
//                    {
//                        var year = DatabaseApi.GetString(rdr, 1);
//                        var month = DatabaseApi.GetString(rdr, 2);
//                        var day = DatabaseApi.GetString(rdr, 3);
//                        var dateTime = TranslateUtils.ToDateTime($"{year}-{month}-{day}");
//                        dict.Add(dateTime, accessNum);
//                    }
//                    else if (EStatictisXTypeUtils.Equals(xType, EStatictisXType.Month))
//                    {
//                        var year = DatabaseApi.GetString(rdr, 1);
//                        var month = DatabaseApi.GetString(rdr, 2);

//                        var dateTime = TranslateUtils.ToDateTime($"{year}-{month}-1");
//                        dict.Add(dateTime, accessNum);
//                    }
//                    else if (EStatictisXTypeUtils.Equals(xType, EStatictisXType.Year))
//                    {
//                        var year = DatabaseApi.GetString(rdr, 1);
//                        var dateTime = TranslateUtils.ToDateTime($"{year}-1-1");
//                        dict.Add(dateTime, accessNum);
//                    }
//                }
//                rdr.Close();
//            }
//            return dict;
//        }

//        public int GetCount()
//        {
//            return DatabaseApi.Instance.GetCount(TableName);
//        }

//        public List<UserInfo> GetUsers(int offset, int limit)
//        {
//            var list = new List<UserInfo>();
//            List<int> dbList;

//            var sqlString =
//                SqlDifferences.GetSqlString(TableName, new List<string>
//                {
//                    nameof(UserInfo.Id)
//                }, string.Empty, "ORDER BY Id", offset, limit);

//            using (var connection = GetConnection())
//            {
//                dbList = connection.Query<int>(sqlString).ToList();
//            }

//            if (dbList.Count > 0)
//            {
//                foreach (var userId in dbList)
//                {
//                    list.Add(UserManager.GetUserInfoByUserId(userId));
//                }
//            }

//            return list;
//        }

//        public bool IsExists(int id)
//        {
//            var sqlString = $"SELECT COUNT(1) FROM {TableName} WHERE Id = @Id";

//            using (var connection = GetConnection())
//            {
//                return connection.ExecuteScalar<bool>(sqlString, new { Id = id });
//            }
//        }

//        public void DeleteById(UserInfo userInfo)
//        {
//            var sqlString = $"DELETE FROM {TableName} WHERE Id = @Id";

//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamId, userInfo.Id)
//            };

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

//            UserManager.RemoveCache(userInfo);
//        }
//    }
//}

