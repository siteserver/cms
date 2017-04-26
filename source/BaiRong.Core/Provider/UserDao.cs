using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using BaiRong.Core.Cryptography;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;

namespace BaiRong.Core.Provider
{
    public class UserDao : DataProviderBase
    {
        public string TableName => "bairong_Users";

        private const string ParmUserId = "@UserID";
        private const string ParmUserName = "@UserName";
        private const string ParmPassword = "@Password";
        private const string ParmPasswordFormat = "@PasswordFormat";
        private const string ParmPasswordSalt = "@PasswordSalt";
        private const string ParmGroupId = "@GroupID";
        private const string ParmCreateDate = "@CreateDate";
        private const string ParmLastResetPasswordDate = "@LastResetPasswordDate";
        private const string ParmLastActivityDate = "@LastActivityDate";
        private const string ParmCountOfLogin = "@CountOfLogin";
        private const string ParmCountOfFailedLogin = "@CountOfFailedLogin";
        private const string ParmCountOfWriting = "@CountOfWriting";
        private const string ParmIsChecked = "@IsChecked";
        private const string ParmIsLockedOut = "@IsLockedOut";
        private const string ParmDisplayname = "@DisplayName";
        private const string ParmEmail = "@Email";
        private const string ParmMobile = "@Mobile";
        private const string ParmAvatarUrl = "@AvatarUrl";

        private const string ParmOrganization = "@Organization";
        private const string ParmDepartment = "@Department";
        private const string ParmPosition = "@Position";
        private const string ParmGender = "@Gender";
        private const string ParmBirthday = "@Birthday";
        private const string ParmEducation = "@Education";
        private const string ParmGraduation = "@Graduation";
        private const string ParmAddress = "@Address";
        private const string ParmWeixin = "@WeiXin";
        private const string ParmQq = "@QQ";
        private const string ParmWeibo = "@WeiBo";
        private const string ParmInterests = "@Interests";
        private const string ParmSignature = "@Signature";
        private const string ParmExtendValues = "@ExtendValues";

        private bool IpAddressIsRegisterAllowed(string ipAddress)
        {
            if (ConfigManager.UserConfigInfo.RegisterMinMinutesOfIpAddress == 0 || string.IsNullOrEmpty(ipAddress))
            {
                return true;
            }
            var obj = CacheManager.GetCache($"BaiRong.Core.Provider.UserDao.Insert.IpAddress.{ipAddress}");
            return obj == null;
        }

        private void IpAddressCache(string ipAddress)
        {
            if (ConfigManager.UserConfigInfo.RegisterMinMinutesOfIpAddress > 0 && !string.IsNullOrEmpty(ipAddress))
            {
                CacheManager.SetCache($"BaiRong.Core.Provider.UserDao.Insert.IpAddress.{ipAddress}", ipAddress, DateTime.Now.AddMinutes(ConfigManager.UserConfigInfo.RegisterMinMinutesOfIpAddress));
            }
        }

        public bool Insert(UserInfo userInfo, string ipAddress, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (!IpAddressIsRegisterAllowed(ipAddress))
            {
                errorMessage = $"同一IP在{ConfigManager.UserConfigInfo.RegisterMinMinutesOfIpAddress}分钟内只能注册一次";
                return false;
            }
            if (string.IsNullOrEmpty(userInfo.Password))
            {
                errorMessage = "密码不能为空";
                return false;
            }
            if (userInfo.Password.Length < ConfigManager.UserConfigInfo.RegisterPasswordMinLength)
            {
                errorMessage = $"密码长度必须大于等于{ConfigManager.UserConfigInfo.RegisterPasswordMinLength}";
                return false;
            }
            if (!EUserPasswordRestrictionUtils.IsValid(userInfo.Password, ConfigManager.UserConfigInfo.RegisterPasswordRestriction))
            {
                errorMessage =
                    $"密码不符合规则，请包含{EUserPasswordRestrictionUtils.GetText(ConfigManager.UserConfigInfo.RegisterPasswordRestriction)}";
                return false;
            }
            if (!IsUserNameCompliant(userInfo.UserName.Replace("@", string.Empty).Replace(".", string.Empty)))
            {
                errorMessage = "用户名包含不规则字符，请更换用户名";
                return false;
            }
            if (!string.IsNullOrEmpty(userInfo.Email) && IsEmailExists(userInfo.Email))
            {
                errorMessage = "电子邮件地址已被注册，请更换邮箱";
                return false;
            }
            if (!string.IsNullOrEmpty(userInfo.Mobile) && IsMobileExists(userInfo.Mobile))
            {
                errorMessage = "手机号码已被注册，请更换手机号码";
                return false;
            }
            if (!string.IsNullOrEmpty(userInfo.UserName) && IsUserExists(userInfo.UserName))
            {
                errorMessage = "用户名已被注册，请更换用户名";
                return false;
            }

            try
            {
                userInfo.PasswordFormat = EPasswordFormatUtils.GetValue(EPasswordFormat.Encrypted);
                var passwordSalt = GenerateSalt();
                userInfo.Password = EncodePassword(userInfo.Password, EPasswordFormatUtils.GetEnumType(userInfo.PasswordFormat), passwordSalt);
                userInfo.PasswordSalt = passwordSalt;

                InsertWithoutValidation(userInfo);

                IpAddressCache(ipAddress);

                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public void InsertWithoutValidation(UserInfo userInfo)
        {
            const string sqlString = "INSERT INTO bairong_Users (UserName, Password, PasswordFormat, PasswordSalt, GroupID, CreateDate, LastResetPasswordDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, CountOfWriting, IsChecked, IsLockedOut, DisplayName, Email, Mobile, AvatarUrl, Organization, Department, Position, Gender, Birthday, Education, Graduation, Address, WeiXin, QQ, WeiBo, Interests, Signature, ExtendValues) VALUES (@UserName, @Password, @PasswordFormat, @PasswordSalt, @GroupID, @CreateDate, @LastResetPasswordDate, @LastActivityDate, @CountOfLogin, @CountOfFailedLogin, @CountOfWriting, @IsChecked, @IsLockedOut, @DisplayName, @Email, @Mobile, @AvatarUrl, @Organization, @Department, @Position, @Gender, @Birthday, @Education, @Graduation, @Address, @WeiXin, @QQ, @WeiBo, @Interests, @Signature, @ExtendValues)";

            userInfo.CreateDate = DateTime.Now;
            userInfo.LastActivityDate = DateTime.Now;
            userInfo.LastResetPasswordDate = DateUtils.SqlMinValue;

            var insertParms = new IDataParameter[]
            {
                GetParameter(ParmUserName, EDataType.NVarChar, 255, userInfo.UserName),
                GetParameter(ParmPassword, EDataType.NVarChar, 255, userInfo.Password),
                GetParameter(ParmPasswordFormat, EDataType.VarChar, 50, userInfo.PasswordFormat),
                GetParameter(ParmPasswordSalt, EDataType.NVarChar, 128, userInfo.PasswordSalt),
                GetParameter(ParmGroupId, EDataType.Integer, userInfo.GroupId),
                GetParameter(ParmCreateDate, EDataType.DateTime, userInfo.CreateDate),
                GetParameter(ParmLastResetPasswordDate, EDataType.DateTime, userInfo.LastResetPasswordDate),
                GetParameter(ParmLastActivityDate, EDataType.DateTime, userInfo.LastActivityDate),
                GetParameter(ParmCountOfLogin, EDataType.Integer, userInfo.CountOfLogin),
                GetParameter(ParmCountOfFailedLogin, EDataType.Integer, userInfo.CountOfFailedLogin),
                GetParameter(ParmCountOfWriting, EDataType.Integer, userInfo.CountOfWriting),
                GetParameter(ParmIsChecked, EDataType.VarChar, 18, userInfo.IsChecked.ToString()),
                GetParameter(ParmIsLockedOut, EDataType.VarChar, 18, userInfo.IsLockedOut.ToString()),
                GetParameter(ParmDisplayname, EDataType.NVarChar, 255, userInfo.DisplayName),
                GetParameter(ParmEmail, EDataType.NVarChar, 255, userInfo.Email),
                GetParameter(ParmMobile, EDataType.VarChar, 20, userInfo.Mobile),
                GetParameter(ParmAvatarUrl, EDataType.VarChar, 200, userInfo.AvatarUrl),
                GetParameter(ParmOrganization, EDataType.NVarChar, 255, userInfo.Organization),
                GetParameter(ParmDepartment, EDataType.NVarChar, 255, userInfo.Department),
                GetParameter(ParmPosition, EDataType.NVarChar, 255, userInfo.Position),
                GetParameter(ParmGender, EDataType.NVarChar, 255, userInfo.Gender),
                GetParameter(ParmBirthday, EDataType.VarChar, 50, userInfo.Birthday),
                GetParameter(ParmEducation, EDataType.NVarChar, 255, userInfo.Education),
                GetParameter(ParmGraduation, EDataType.NVarChar, 255, userInfo.Graduation),
                GetParameter(ParmAddress, EDataType.NVarChar, 255, userInfo.Address),
                GetParameter(ParmWeixin, EDataType.NVarChar, 255, userInfo.WeiXin),
                GetParameter(ParmQq, EDataType.NVarChar, 255, userInfo.Qq),
                GetParameter(ParmWeibo, EDataType.NVarChar, 255, userInfo.WeiBo),
                GetParameter(ParmInterests, EDataType.NVarChar, 255, userInfo.Interests),
                GetParameter(ParmSignature, EDataType.NVarChar, 255, userInfo.Signature),
                GetParameter(ParmExtendValues, EDataType.NText, userInfo.Additional.ToString())
            };

            ExecuteNonQuery(sqlString, insertParms);
        }

        public bool IsPasswordCorrect(string password, out string errorMessage)
        {
            errorMessage = null;
            if (string.IsNullOrEmpty(password))
            {
                errorMessage = "密码不能为空";
                return false;
            }
            if (password.Length < ConfigManager.UserConfigInfo.RegisterPasswordMinLength)
            {
                errorMessage = $"密码长度必须大于等于{ConfigManager.UserConfigInfo.RegisterPasswordMinLength}";
                return false;
            }
            if (!EUserPasswordRestrictionUtils.IsValid(password, ConfigManager.UserConfigInfo.RegisterPasswordRestriction))
            {
                errorMessage =
                    $"密码不符合规则，请包含{EUserPasswordRestrictionUtils.GetText(ConfigManager.UserConfigInfo.RegisterPasswordRestriction)}";
                return false;
            }
            return true;
        }

        public void Update(UserInfo userInfo)
        {
            const string sqlString = "UPDATE bairong_Users SET UserName = @UserName, Password = @Password, PasswordFormat = @PasswordFormat, PasswordSalt = @PasswordSalt, GroupID = @GroupID, CreateDate = @CreateDate, LastResetPasswordDate = @LastResetPasswordDate, LastActivityDate = @LastActivityDate, CountOfLogin = @CountOfLogin, CountOfFailedLogin = @CountOfFailedLogin, CountOfWriting = @CountOfWriting, IsChecked = @IsChecked, IsLockedOut = @IsLockedOut, DisplayName = @DisplayName, Email = @Email, Mobile = @Mobile, AvatarUrl = @AvatarUrl, Organization = @Organization, Department = @Department, Position = @Position, Gender = @Gender, Birthday = @Birthday, Education = @Education, Graduation = @Graduation, Address = @Address, WeiXin = @WeiXin, QQ = @QQ, WeiBo = @WeiBo, Interests = @Interests, Signature = @Signature, ExtendValues = @ExtendValues WHERE UserID = @UserID";

            var updateParms = new IDataParameter[]
            {
                GetParameter(ParmUserName, EDataType.NVarChar, 255, userInfo.UserName),
                GetParameter(ParmPassword, EDataType.NVarChar, 255, userInfo.Password),
                GetParameter(ParmPasswordFormat, EDataType.VarChar, 50, userInfo.PasswordFormat),
                GetParameter(ParmPasswordSalt, EDataType.NVarChar, 128, userInfo.PasswordSalt),
                GetParameter(ParmGroupId, EDataType.Integer, userInfo.GroupId),
                GetParameter(ParmCreateDate, EDataType.DateTime, userInfo.CreateDate),
                GetParameter(ParmLastResetPasswordDate, EDataType.DateTime, userInfo.LastResetPasswordDate),
                GetParameter(ParmLastActivityDate, EDataType.DateTime, userInfo.LastActivityDate),
                GetParameter(ParmCountOfLogin, EDataType.Integer, userInfo.CountOfLogin),
                GetParameter(ParmCountOfFailedLogin, EDataType.Integer, userInfo.CountOfFailedLogin),
                GetParameter(ParmCountOfWriting, EDataType.Integer, userInfo.CountOfWriting),
                GetParameter(ParmIsChecked, EDataType.VarChar, 18, userInfo.IsChecked.ToString()),
                GetParameter(ParmIsLockedOut, EDataType.VarChar, 18, userInfo.IsLockedOut.ToString()),
                GetParameter(ParmDisplayname, EDataType.NVarChar, 255, userInfo.DisplayName),
                GetParameter(ParmEmail, EDataType.NVarChar, 255, userInfo.Email),
                GetParameter(ParmMobile, EDataType.VarChar, 20, userInfo.Mobile),
                GetParameter(ParmAvatarUrl, EDataType.VarChar, 200, userInfo.AvatarUrl),
                GetParameter(ParmOrganization, EDataType.NVarChar, 255, userInfo.Organization),
                GetParameter(ParmDepartment, EDataType.NVarChar, 255, userInfo.Department),
                GetParameter(ParmPosition, EDataType.NVarChar, 255, userInfo.Position),
                GetParameter(ParmGender, EDataType.NVarChar, 255, userInfo.Gender),
                GetParameter(ParmBirthday, EDataType.VarChar, 50, userInfo.Birthday),
                GetParameter(ParmEducation, EDataType.NVarChar, 255, userInfo.Education),
                GetParameter(ParmGraduation, EDataType.NVarChar, 255, userInfo.Graduation),
                GetParameter(ParmAddress, EDataType.NVarChar, 255, userInfo.Address),
                GetParameter(ParmWeixin, EDataType.NVarChar, 255, userInfo.WeiXin),
                GetParameter(ParmQq, EDataType.NVarChar, 255, userInfo.Qq),
                GetParameter(ParmWeibo, EDataType.NVarChar, 255, userInfo.WeiBo),
                GetParameter(ParmInterests, EDataType.NVarChar, 255, userInfo.Interests),
                GetParameter(ParmSignature, EDataType.NVarChar, 255, userInfo.Signature),
                GetParameter(ParmExtendValues, EDataType.NText, userInfo.Additional.ToString()),
                GetParameter(ParmUserId, EDataType.Integer, userInfo.UserId)
            };

            ExecuteNonQuery(sqlString, updateParms);
        }

        public void UpdateLastActivityDate(string userName)
        {
            const string sqlString = "UPDATE bairong_Users SET LastActivityDate = @LastActivityDate WHERE UserName = @UserName";

            var updateParms = new IDataParameter[]
            {
                GetParameter(ParmLastActivityDate, EDataType.DateTime, DateTime.Now),
                GetParameter(ParmUserName, EDataType.NVarChar, 255, userName)
            };

            ExecuteNonQuery(sqlString, updateParms);
        }

        public string EncodePassword(string password, EPasswordFormat passwordFormat, string passwordSalt)
        {
            var retval = string.Empty;

            if (passwordFormat == EPasswordFormat.Clear)
            {
                retval = password;
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

                if (inArray != null) retval = Convert.ToBase64String(inArray);
            }
            else if (passwordFormat == EPasswordFormat.Encrypted)
            {
                var encryptor = new DESEncryptor
                {
                    InputString = password,
                    EncryptKey = passwordSalt
                };
                encryptor.DesEncrypt();

                retval = encryptor.OutString;
            }
            return retval;
        }

        public string DecodePassword(string password, EPasswordFormat passwordFormat, string passwordSalt)
        {
            var retval = string.Empty;
            if (passwordFormat == EPasswordFormat.Clear)
            {
                retval = password;
            }
            else if (passwordFormat == EPasswordFormat.Hashed)
            {
                throw new Exception("can not decode hashed password");
            }
            else if (passwordFormat == EPasswordFormat.Encrypted)
            {
                var encryptor = new DESEncryptor
                {
                    InputString = password,
                    DecryptKey = passwordSalt
                };
                encryptor.DesDecrypt();

                retval = encryptor.OutString;
            }
            return retval;
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
            if (password.Length < ConfigManager.UserConfigInfo.RegisterPasswordMinLength)
            {
                errorMessage = $"密码长度必须大于等于{ConfigManager.UserConfigInfo.RegisterPasswordMinLength}";
                return false;
            }
            if (!EUserPasswordRestrictionUtils.IsValid(password, ConfigManager.UserConfigInfo.RegisterPasswordRestriction))
            {
                errorMessage =
                    $"密码不符合规则，请包含{EUserPasswordRestrictionUtils.GetText(ConfigManager.UserConfigInfo.RegisterPasswordRestriction)}";
                return false;
            }

            var passwordFormat = EPasswordFormat.Encrypted;
            var passwordSalt = GenerateSalt();
            password = EncodePassword(password, passwordFormat, passwordSalt);
            return ChangePassword(userName, passwordFormat, passwordSalt, password);
        }

        private bool ChangePassword(string userName, EPasswordFormat passwordFormat, string passwordSalt, string password)
        {
            var isSuccess = false;

            const string sqlString = "UPDATE bairong_Users SET Password = @Password, PasswordFormat = @PasswordFormat, PasswordSalt = @PasswordSalt, LastResetPasswordDate = @LastResetPasswordDate WHERE UserName = @UserName";

            var updateParms = new IDataParameter[]
            {
                GetParameter(ParmPassword, EDataType.NVarChar, 255, password),
                GetParameter(ParmPasswordFormat, EDataType.VarChar, 50, EPasswordFormatUtils.GetValue(passwordFormat)),
                GetParameter(ParmPasswordSalt, EDataType.NVarChar, 128, passwordSalt),
                GetParameter(ParmLastResetPasswordDate, EDataType.DateTime, DateTime.Now),
                GetParameter(ParmUserName, EDataType.NVarChar, 255, userName)
            };

            try
            {
                ExecuteNonQuery(sqlString, updateParms);
                isSuccess = true;
            }
            catch
            {
                // ignored
            }
            return isSuccess;
        }

        public void Delete(int userId)
        {
            const string sqlString = "DELETE FROM bairong_Users WHERE UserID = @UserID";

            var deleteParms = new IDataParameter[]
            {
                GetParameter(ParmUserId, EDataType.Integer, userId)
            };

            ExecuteNonQuery(sqlString, deleteParms);
        }

        public void Check(List<int> userIdList)
        {
            string sqlString =
                $"UPDATE bairong_Users SET IsChecked = '{true}' WHERE UserID IN ({TranslateUtils.ToSqlInStringWithoutQuote(userIdList)})";

            ExecuteNonQuery(sqlString);
        }

        public void Check(int userId)
        {
            string sqlString = $"UPDATE bairong_Users SET IsChecked = '{true}' WHERE UserID = {userId}";

            ExecuteNonQuery(sqlString);
        }

        public void Lock(List<int> userIdList)
        {
            string sqlString =
                $"UPDATE bairong_Users SET IsLockedOut = '{true}' WHERE UserID IN ({TranslateUtils.ToSqlInStringWithQuote(userIdList)})";

            ExecuteNonQuery(sqlString);
        }

        public void UnLock(List<int> userIdList)
        {
            string sqlString =
                $"UPDATE bairong_Users SET IsLockedOut = '{false}', CountOfFailedLogin = 0 WHERE UserID IN ({TranslateUtils.ToSqlInStringWithQuote(userIdList)})";

            ExecuteNonQuery(sqlString);
        }

        public UserInfo GetUserInfoByAccount(string account)
        {
            if (string.IsNullOrEmpty(account)) return null;

            UserInfo userInfo = null;

            if (StringUtils.IsMobile(account))
            {
                userInfo = GetUserInfoByMobile(account);
            }
            else if (StringUtils.IsEmail(account))
            {
                userInfo = GetUserInfoByEmail(account);
            }

            return userInfo ?? GetUserInfoByUserName(account);
        }

        private UserInfo GetUserInfo(IDataReader rdr)
        {
            var i = 0;
            var userInfo = new UserInfo
            {
                UserId = GetInt(rdr, i++),
                UserName = GetString(rdr, i++),
                Password = GetString(rdr, i++),
                PasswordFormat = GetString(rdr, i++),
                PasswordSalt = GetString(rdr, i++),
                GroupId = GetInt(rdr, i++),
                CreateDate = GetDateTime(rdr, i++),
                LastResetPasswordDate = GetDateTime(rdr, i++),
                LastActivityDate = GetDateTime(rdr, i++),
                CountOfLogin = GetInt(rdr, i++),
                CountOfFailedLogin = GetInt(rdr, i++),
                CountOfWriting = GetInt(rdr, i++),
                IsChecked = GetBool(rdr, i++),
                IsLockedOut = GetBool(rdr, i++),
                DisplayName = GetString(rdr, i++),
                Email = GetString(rdr, i++),
                Mobile = GetString(rdr, i++),
                AvatarUrl = GetString(rdr, i++),
                Organization = GetString(rdr, i++),
                Department = GetString(rdr, i++),
                Position = GetString(rdr, i++),
                Gender = GetString(rdr, i++),
                Birthday = GetString(rdr, i++),
                Education = GetString(rdr, i++),
                Graduation = GetString(rdr, i++),
                Address = GetString(rdr, i++),
                WeiXin = GetString(rdr, i++),
                Qq = GetString(rdr, i++),
                WeiBo = GetString(rdr, i++),
                Interests = GetString(rdr, i++),
                Signature = GetString(rdr, i++),
                ExtendValues = GetString(rdr, i)
            };
            if (string.IsNullOrEmpty(userInfo.DisplayName))
            {
                userInfo.DisplayName = userInfo.UserName;
            }
            return userInfo;
        }

        public UserInfo GetUserInfoByUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return null;

            UserInfo userInfo = null;
            const string sqlString = "SELECT UserID, UserName, Password, PasswordFormat, PasswordSalt, GroupID, CreateDate, LastResetPasswordDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, CountOfWriting, IsChecked, IsLockedOut, DisplayName, Email, Mobile, AvatarUrl, Organization, Department, Position, Gender, Birthday, Education, Graduation, Address, WeiXin, QQ, WeiBo, Interests, Signature, ExtendValues FROM bairong_Users WHERE UserName = @UserName";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmUserName, EDataType.NVarChar, 255, userName)
            };

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    userInfo = GetUserInfo(rdr);
                }
                rdr.Close();
            }
            return userInfo;
        }

        public UserInfo GetUserInfoByEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return null;

            UserInfo userInfo = null;
            const string sqlString = "SELECT UserID, UserName, Password, PasswordFormat, PasswordSalt, GroupID, CreateDate, LastResetPasswordDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, CountOfWriting, IsChecked, IsLockedOut, DisplayName, Email, Mobile, AvatarUrl, Organization, Department, Position, Gender, Birthday, Education, Graduation, Address, WeiXin, QQ, WeiBo, Interests, Signature, ExtendValues FROM bairong_Users WHERE Email = @Email";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmEmail, EDataType.NVarChar, 255, email)
            };

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    userInfo = GetUserInfo(rdr);
                }
                rdr.Close();
            }
            return userInfo;
        }

        public UserInfo GetUserInfoByMobile(string mobile)
        {
            if (string.IsNullOrEmpty(mobile)) return null;

            UserInfo userInfo = null;
            const string sqlString = "SELECT UserID, UserName, Password, PasswordFormat, PasswordSalt, GroupID, CreateDate, LastResetPasswordDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, CountOfWriting, IsChecked, IsLockedOut, DisplayName, Email, Mobile, AvatarUrl, Organization, Department, Position, Gender, Birthday, Education, Graduation, Address, WeiXin, QQ, WeiBo, Interests, Signature, ExtendValues FROM bairong_Users WHERE Mobile = @Mobile";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmMobile, EDataType.VarChar, 20, mobile)
            };

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    userInfo = GetUserInfo(rdr);
                }
                rdr.Close();
            }
            return userInfo;
        }

        public string GetUserNameByMobile(string mobile)
        {
            if (string.IsNullOrEmpty(mobile)) return string.Empty;

            var userName = string.Empty;
            const string sqlString = "SELECT UserName FROM bairong_Users WHERE Mobile = @Mobile";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmMobile, EDataType.VarChar, 20, mobile)
            };

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    userName = GetString(rdr, 0);
                }
                rdr.Close();
            }
            return userName;
        }

        public UserInfo GetUserInfo(int userId)
        {
            if (userId <= 0) return null;

            UserInfo userInfo = null;
            const string sqlString = "SELECT UserID, UserName, Password, PasswordFormat, PasswordSalt, GroupID, CreateDate, LastResetPasswordDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, CountOfWriting, IsChecked, IsLockedOut, DisplayName, Email, Mobile, AvatarUrl, Organization, Department, Position, Gender, Birthday, Education, Graduation, Address, WeiXin, QQ, WeiBo, Interests, Signature, ExtendValues FROM bairong_Users WHERE UserID = @UserID";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmUserId, EDataType.Integer, userId)
            };

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    userInfo = GetUserInfo(rdr);
                }
                rdr.Close();
            }
            return userInfo;
        }

        public bool IsUserExists(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return false;

            var exists = false;

            const string sqlString = "SELECT UserID FROM bairong_Users WHERE UserName = @UserName";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmUserName, EDataType.NVarChar, 255, userName.ToLower())
            };

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    exists = true;
                }
                rdr.Close();
            }

            return exists;
        }

        public bool IsUserNameCompliant(string userName)
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

            var exists = false;

            const string sqlSelect = "SELECT Email FROM bairong_Users WHERE Email = @Email";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmEmail, EDataType.VarChar, 200, email.ToLower())
            };

            using (var rdr = ExecuteReader(sqlSelect, parms))
            {
                if (rdr.Read())
                {
                    exists = true;
                }
                rdr.Close();
            }

            return exists;
        }

        public bool IsMobileExists(string mobile)
        {
            if (string.IsNullOrEmpty(mobile)) return false;

            var exists = false;
            const string sqlString = "SELECT Mobile FROM bairong_Users WHERE Mobile = @Mobile";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmMobile, EDataType.VarChar, 20, mobile)
            };

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    exists = true;
                }
                rdr.Close();
            }

            return exists;
        }

        public string GetUserName(int userId)
        {
            var userName = string.Empty;

            const string sqlString = "SELECT UserName FROM bairong_Users WHERE UserID = @UserID";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmUserId, EDataType.Integer, userId)
            };

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    userName = GetString(rdr, 0);
                }
                rdr.Close();
            }
            return userName;
        }

        public string GetEmail(int userId)
        {
            var email = string.Empty;

            const string sqlString = "SELECT Email FROM bairong_Users WHERE UserID = @UserID";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmUserId, EDataType.Integer, userId)
            };

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    email = GetString(rdr, 0);
                }
                rdr.Close();
            }
            return email;
        }

        public int GetUserId(string userName)
        {
            var userId = 0;

            const string sqlString = "SELECT UserID FROM bairong_Users WHERE UserName = @UserName";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmUserName, EDataType.NVarChar, 255, userName)
            };

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    userId = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return userId;
        }

        public string GetDisplayName(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return string.Empty;
            var displayName = string.Empty;

            const string sqlString = "SELECT DisplayName FROM bairong_Users WHERE UserName = @UserName";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmUserName, EDataType.NVarChar, 255, userName)
            };

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    displayName = GetString(rdr, 0);
                }
                rdr.Close();
            }

            if (string.IsNullOrEmpty(displayName))
            {
                displayName = userName;
            }

            return displayName;
        }

        public int GetUserIdByEmailOrMobile(string email, string mobile)
        {
            var userId = 0;

            if (!string.IsNullOrEmpty(email))
            {
                const string sqlString = @"SELECT UserID FROM bairong_Users WHERE Email = @Email";

                var parms = new IDataParameter[]
                {
                    GetParameter(ParmEmail, EDataType.VarChar, 200, email)
                };

                using (var rdr = ExecuteReader(sqlString, parms))
                {
                    if (rdr.Read())
                    {
                        userId = GetInt(rdr, 0);
                    }
                    rdr.Close();
                }
            }
            else if (!string.IsNullOrEmpty(mobile))
            {
                const string sqlString = "SELECT UserID FROM bairong_Users WHERE Mobile = @Mobile";

                var parms = new IDataParameter[]
                {
                    GetParameter(ParmMobile, EDataType.VarChar, 20, mobile)
                };

                using (var rdr = ExecuteReader(sqlString, parms))
                {
                    if (rdr.Read())
                    {
                        userId = GetInt(rdr, 0);
                    }
                    rdr.Close();
                }
            }

            return userId;
        }

        public string GetMobile(int userId)
        {
            var mobile = string.Empty;

            const string sqlString = "SELECT Mobile FROM bairong_Users WHERE UserID = @UserID";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmUserId, EDataType.Integer, userId)
            };

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    mobile = GetString(rdr, 0);
                }
                rdr.Close();
            }
            return mobile;
        }

        public string GetMobileByAccount(string account)
        {
            if (string.IsNullOrEmpty(account)) return string.Empty;

            var mobile = string.Empty;

            string sqlString;
            IDataParameter[] parms;

            if (StringUtils.IsMobile(account))
            {
                sqlString = "SELECT Mobile FROM bairong_Users WHERE Mobile = @Mobile";
                parms = new IDataParameter[]
                {
                    GetParameter(ParmMobile, EDataType.VarChar, 20, account)
                };
            }
            else if (StringUtils.IsEmail(account))
            {
                sqlString = "SELECT Mobile FROM bairong_Users WHERE Email = @Email";
                parms = new IDataParameter[]
                {
                    GetParameter(ParmEmail, EDataType.VarChar, 200, account)
                };
            }
            else
            {
                sqlString = "SELECT Mobile FROM bairong_Users WHERE UserName = @UserName";
                parms = new IDataParameter[]
                {
                    GetParameter(ParmUserName, EDataType.NVarChar, 255, account)
                };
            }

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    mobile = GetString(rdr, 0);
                }
                rdr.Close();
            }

            return mobile;
        }

        public int GetTotalCount()
        {
            var count = 0;

            using (var rdr = ExecuteReader("SELECT COUNT(*) AS TotalNum FROM bairong_Users"))
            {
                if (rdr.Read())
                {
                    count = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return count;
        }

        public List<string> GetUserNameList(bool isChecked)
        {
            var userNameList = new List<string>();
            string sqlSelect =
                $"SELECT UserName FROM bairong_Users WHERE IsChecked = '{isChecked}' ORDER BY UserID DESC";

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    userNameList.Add(GetString(rdr, 0));
                }
                rdr.Close();
            }
            return userNameList;
        }

        public List<int> GetUserIdList(bool isChecked)
        {
            var userIdList = new List<int>();

            string sqlSelect =
                $"SELECT UserID FROM bairong_Users WHERE IsChecked = '{isChecked}' ORDER BY UserID DESC";

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    userIdList.Add(GetInt(rdr, 0));
                }
                rdr.Close();
            }

            return userIdList;
        }

        public List<string> GetUserNameListByGroupIdCollection(string groupIdCollection)
        {
            if (string.IsNullOrEmpty(groupIdCollection)) return new List<string>();

            var list = new List<string>();
            string sqlSelect =
                $"SELECT UserName FROM bairong_Users WHERE GroupID IN ({groupIdCollection}) ORDER BY UserID DESC";

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    list.Add(GetString(rdr, 0));
                }
                rdr.Close();
            }
            return list;
        }

        public List<int> GetUserIdListByGroupIdCollection(string groupIdCollection)
        {
            if (string.IsNullOrEmpty(groupIdCollection)) return new List<int>();

            var userIdList = new List<int>();
            string sqlSelect =
                $"SELECT UserID FROM bairong_Users WHERE GroupID IN ({groupIdCollection}) ORDER BY UserID DESC";

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    userIdList.Add(GetInt(rdr, 0));
                }
                rdr.Close();
            }
            return userIdList;
        }

        public List<string> GetUserNameList(string searchWord, int dayOfCreate, int dayOfLastActivity, bool isChecked)
        {
            var list = new List<string>();

            var whereString = string.Empty;

            if (dayOfCreate > 0)
            {
                var dateTime = DateTime.Now.AddDays(-dayOfCreate);
                whereString += $" AND (CreateDate >= '{dateTime:yyyy-MM-dd}') ";
            }
            if (dayOfLastActivity > 0)
            {
                var dateTime = DateTime.Now.AddDays(-dayOfLastActivity);
                whereString += $" AND (LastActivityDate >= '{dateTime:yyyy-MM-dd}') ";
            }
            if (!string.IsNullOrEmpty(searchWord))
            {
                var word = PageUtils.FilterSql(searchWord);
                whereString += $" AND (UserName LIKE '%{word}%' OR EMAIL LIKE '%{word}%' OR MOBILE = '{word}') ";
            }
            string sqlString =
                $"SELECT UserName FROM bairong_Users WHERE IsChecked = '{isChecked}' {whereString} ORDER BY UserID DESC";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    list.Add(GetString(rdr, 0));
                }
                rdr.Close();
            }
            return list;
        }

        public string GetSelectCommand(bool isChecked)
        {
            string whereString = $"WHERE IsChecked = '{isChecked}'";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
        }

        public string GetSelectCommand(string searchWord, int dayOfCreate, int dayOfLastActivity, bool isChecked, int groupId, int loginCount, string searchType)
        {
            var whereBuilder = new StringBuilder();

            if (dayOfCreate > 0)
            {
                whereBuilder.Append(" AND ");

                var dateTime = DateTime.Now.AddDays(-dayOfCreate);
                whereBuilder.Append($"(CreateDate >= '{dateTime:yyyy-MM-dd}')");
            }

            if (dayOfLastActivity > 0)
            {
                whereBuilder.Append(" AND ");

                var dateTime = DateTime.Now.AddDays(-dayOfLastActivity);
                whereBuilder.Append($"(LastActivityDate >= '{dateTime:yyyy-MM-dd}') ");
            }

            if (groupId > 0)
            {
                whereBuilder.Append(" AND ");
                whereBuilder.Append($" GroupID = {groupId}");
            }

            if (string.IsNullOrEmpty(searchType))
            {
                whereBuilder.Append(" AND ");
                whereBuilder.Append(
                    $"(UserName LIKE '%{PageUtils.FilterSql(searchWord)}%' OR EMAIL LIKE '%{PageUtils.FilterSql(searchWord)}%')");
            }
            else
            {
                whereBuilder.Append(" AND ");
                whereBuilder.Append($"({searchType} LIKE '%{searchWord}%') ");
            }

            if (loginCount > 0)
            {
                whereBuilder.Append(" AND ");
                whereBuilder.Append($"(CountOfLogin > {loginCount})");
            }

            var whereString = string.Empty;
            if (whereBuilder.Length > 0)
            {
                whereString = $"WHERE IsChecked = '{isChecked}' {whereBuilder}";
            }

            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
        }

        public string GetSortFieldName()
        {
            return "UserID";
        }

        public IEnumerable GetStlDataSource(int startNum, int totalNum, string orderByString, string whereString)
        {
            string sqlWhereString = $"WHERE IsChecked = '{true}' {whereString}";
            if (string.IsNullOrEmpty(orderByString))
            {
                orderByString = "ORDER BY UserID DESC";
            }

            IEnumerable enumerable;

            if (startNum <= 1)
            {
                var sqlString = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, totalNum, SqlUtils.Asterisk, sqlWhereString, orderByString);
                enumerable = (IEnumerable)ExecuteReader(sqlString);
            }
            else
            {
                var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, startNum, totalNum, SqlUtils.Asterisk, sqlWhereString, orderByString);
                enumerable = (IEnumerable)ExecuteReader(sqlSelect);
            }

            return enumerable;
        }

        public void SetGroupId(string userName, int groupId)
        {
            const string sqlString = "UPDATE bairong_Users SET GroupID = @GroupID WHERE UserName = @UserName";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmGroupId, EDataType.Integer, groupId),
                GetParameter(ParmUserName, EDataType.NVarChar, 255, userName)
            };

            ExecuteNonQuery(sqlString, parms);
        }

        public void SetGroupId(List<string> userNameList, int groupId)
        {
            string sqlString =
                $"UPDATE bairong_Users SET GroupID = @GroupID WHERE UserName IN ({TranslateUtils.ToSqlInStringWithQuote(userNameList)})";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmGroupId, EDataType.Integer, groupId)
            };

            ExecuteNonQuery(sqlString, parms);
        }

        public bool CheckPassword(string password, string dbpassword, EPasswordFormat passwordFormat, string passwordSalt)
        {
            var decodePassword = DecodePassword(dbpassword, passwordFormat, passwordSalt);
            return password == decodePassword;
        }

        public bool Import(UserInfo userInfo)
        {
            if (string.IsNullOrEmpty(userInfo.UserName))
            {
                return false;
            }
            if (string.IsNullOrEmpty(userInfo.Password))
            {
                return false;
            }
            if (IsUserExists(userInfo.UserName))
            {
                return false;
            }
            try
            {
                InsertWithoutValidation(userInfo);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void UpdateLastActivityDateAndCountOfFailedLogin(string userName)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                var sqlString = $"UPDATE bairong_Users SET LastActivityDate = @LastActivityDate, {SqlUtils.GetAddOne("CountOfFailedLogin")} WHERE UserName = @UserName";

                IDataParameter[] updateParms = {
                    GetParameter(ParmLastActivityDate, EDataType.DateTime, DateTime.Now),
                    GetParameter(ParmUserName, EDataType.NVarChar, 255, userName)
                };

                ExecuteNonQuery(sqlString, updateParms);
            }
        }

        public void UpdateLastActivityDateAndCountOfLogin(string userName)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                var sqlString = $"UPDATE bairong_Users SET LastActivityDate = @LastActivityDate, {SqlUtils.GetAddOne("CountOfLogin")}, CountOfFailedLogin = 0 WHERE UserName = @UserName";

                IDataParameter[] updateParms = {
                    GetParameter(ParmLastActivityDate, EDataType.DateTime, DateTime.Now),
                    GetParameter(ParmUserName, EDataType.NVarChar, 255, userName)
                };

                ExecuteNonQuery(sqlString, updateParms);
            }
        }

        public bool ValidateAccount(string account, string password, out string userName, out string errorMessage)
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

            var userInfo = GetUserInfoByAccount(account);

            if (string.IsNullOrEmpty(userInfo?.UserName))
            {
                errorMessage = "帐号或密码错误";
                return false;
            }

            userName = userInfo.UserName;

            if (!userInfo.IsChecked)
            {
                errorMessage = "此账号未审核，无法登录";
                return false;
            }

            if (userInfo.IsLockedOut)
            {
                errorMessage = "此账号被锁定，无法登录";
                return false;
            }

            if (ConfigManager.UserConfigInfo.IsLoginFailToLock)
            {
                if (userInfo.CountOfFailedLogin > 0 && userInfo.CountOfFailedLogin >= ConfigManager.UserConfigInfo.LoginFailToLockCount)
                {
                    var lockType = EUserLockTypeUtils.GetEnumType(ConfigManager.UserConfigInfo.LoginLockingType);
                    if (lockType == EUserLockType.Forever)
                    {
                        errorMessage = "此账号错误登录次数过多，已被永久锁定";
                        return false;
                    }
                    if (lockType == EUserLockType.Hours)
                    {
                        var ts = new TimeSpan(DateTime.Now.Ticks - userInfo.LastActivityDate.Ticks);
                        var hours = Convert.ToInt32(ConfigManager.UserConfigInfo.LoginLockingHours - ts.TotalHours);
                        if (hours > 0)
                        {
                            errorMessage =
                                $"此账号错误登录次数过多，已被锁定，请等待{hours}小时后重试";
                            return false;
                        }
                    }
                }
            }

            if (!CheckPassword(password, userInfo.Password, EPasswordFormatUtils.GetEnumType(userInfo.PasswordFormat), userInfo.PasswordSalt))
            {
                errorMessage = "帐号或密码错误";
                return false;
            }

            UpdateLastActivityDate(userInfo.UserName);

            return true;
        }

        public Dictionary<DateTime, int> GetTrackingDictionary(DateTime dateFrom, DateTime dateTo, string xType)
        {
            var dict = new Dictionary<DateTime, int>();
            if (string.IsNullOrEmpty(xType))
            {
                xType = EStatictisXTypeUtils.GetValue(EStatictisXType.Day);
            }

            var builder = new StringBuilder();
            builder.Append($" AND CreateDate >= '{dateFrom}'");
            builder.Append($" AND CreateDate < '{dateTo}'");

            string sqlString = $@"
SELECT COUNT(*) AS AddNum, AddYear, AddMonth, AddDay FROM (
    SELECT {SqlUtils.GetDatePartYear("CreateDate")} AS AddYear, {SqlUtils.GetDatePartMonth("CreateDate")} AS AddMonth, {SqlUtils.GetDatePartDay("CreateDate")} AS AddDay 
    FROM bairong_Users 
    WHERE {SqlUtils.GetDateDiffLessThanDays("CreateDate", 30.ToString())} {builder}
) DERIVEDTBL GROUP BY AddYear, AddMonth, AddDay ORDER BY AddYear, AddMonth, AddDay
";//添加日统计

            if (EStatictisXTypeUtils.Equals(xType, EStatictisXType.Month))
            {
                sqlString = $@"
SELECT COUNT(*) AS AddNum, AddYear, AddMonth FROM (
    SELECT {SqlUtils.GetDatePartYear("CreateDate")} AS AddYear, {SqlUtils.GetDatePartMonth("CreateDate")} AS AddMonth 
    FROM bairong_Users 
    WHERE {SqlUtils.GetDateDiffLessThanMonths("CreateDate", 12.ToString())} {builder}
) DERIVEDTBL GROUP BY AddYear, AddMonth ORDER BY AddYear, AddMonth
";//添加月统计
            }
            else if (EStatictisXTypeUtils.Equals(xType, EStatictisXType.Year))
            {
                sqlString = $@"
SELECT COUNT(*) AS AddNum, AddYear FROM (
    SELECT {SqlUtils.GetDatePartYear("CreateDate")} AS AddYear
    FROM bairong_Users 
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
    }
}

