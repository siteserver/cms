using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using SiteServer.CMS.Core;
using SiteServer.CMS.Data;
using SiteServer.CMS.Model;
using SiteServer.Utils.Auth;
using SiteServer.Plugin;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;
using Dapper.Contrib.Extensions;

namespace SiteServer.CMS.Provider
{
    public class UserDao : DataProviderBase
    {
        public const string DatabaseTableName = "siteserver_User";

        public override string TableName => DatabaseTableName;

        public override List<TableColumn> TableColumns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(UserInfoDatabase.Id),
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfoDatabase.UserName),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfoDatabase.Password),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfoDatabase.PasswordFormat),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfoDatabase.PasswordSalt),
                DataType = DataType.VarChar,
                DataLength = 128
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfoDatabase.CreateDate),
                DataType = DataType.DateTime
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfoDatabase.LastResetPasswordDate),
                DataType = DataType.DateTime
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfoDatabase.LastActivityDate),
                DataType = DataType.DateTime
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfoDatabase.CountOfLogin),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfoDatabase.CountOfFailedLogin),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfoDatabase.CountOfWriting),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfoDatabase.IsChecked),
                DataType = DataType.VarChar,
                DataLength = 18
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfoDatabase.IsLockedOut),
                DataType = DataType.VarChar,
                DataLength = 18
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfoDatabase.DisplayName),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfoDatabase.Email),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfoDatabase.Mobile),
                DataType = DataType.VarChar,
                DataLength = 20
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfoDatabase.AvatarUrl),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfoDatabase.Organization),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfoDatabase.Department),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfoDatabase.Position),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfoDatabase.Gender),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfoDatabase.Birthday),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfoDatabase.Education),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfoDatabase.Graduation),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfoDatabase.Address),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfoDatabase.WeiXin),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfoDatabase.Qq),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfoDatabase.WeiBo),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfoDatabase.Interests),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfoDatabase.Signature),
                DataType = DataType.VarChar,
                DataLength = 255
            }
        };

        private const string ParmId = "@Id";
        private const string ParmUserName = "@UserName";
        private const string ParmPassword = "@Password";
        private const string ParmPasswordFormat = "@PasswordFormat";
        private const string ParmPasswordSalt = "@PasswordSalt";
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

        private bool IpAddressIsRegisterAllowed(string ipAddress)
        {
            if (ConfigManager.SystemConfigInfo.UserRegistrationMinMinutes == 0 || string.IsNullOrEmpty(ipAddress))
            {
                return true;
            }
            var obj = CacheUtils.Get($"SiteServer.CMS.Provider.UserDao.Insert.IpAddress.{ipAddress}");
            return obj == null;
        }

        private void IpAddressCache(string ipAddress)
        {
            if (ConfigManager.SystemConfigInfo.UserRegistrationMinMinutes > 0 && !string.IsNullOrEmpty(ipAddress))
            {
                CacheUtils.InsertMinutes($"SiteServer.CMS.Provider.UserDao.Insert.IpAddress.{ipAddress}", ipAddress, ConfigManager.SystemConfigInfo.UserRegistrationMinMinutes);
            }
        }

        private bool InsertValidate(string userName, string email, string mobile, string password, string ipAddress, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (!IpAddressIsRegisterAllowed(ipAddress))
            {
                errorMessage = $"同一IP在{ConfigManager.SystemConfigInfo.UserRegistrationMinMinutes}分钟内只能注册一次";
                return false;
            }
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

        private bool UpdateValidate(UserInfoCreateUpdate userInfoToUpdate, string userName, string email, string mobile, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (userInfoToUpdate.UserName != null && userInfoToUpdate.UserName != userName)
            {
                if (!IsUserNameCompliant(userInfoToUpdate.UserName.Replace("@", string.Empty).Replace(".", string.Empty)))
                {
                    errorMessage = "用户名包含不规则字符，请更换用户名";
                    return false;
                }
                if (!string.IsNullOrEmpty(userInfoToUpdate.UserName) && IsUserNameExists(userInfoToUpdate.UserName))
                {
                    errorMessage = "用户名已被注册，请更换用户名";
                    return false;
                }
            }

            if (userInfoToUpdate.Email != null && userInfoToUpdate.Email != email)
            {
                if (!string.IsNullOrEmpty(userInfoToUpdate.Email) && IsEmailExists(userInfoToUpdate.Email))
                {
                    errorMessage = "电子邮件地址已被注册，请更换邮箱";
                    return false;
                }
            }

            if (userInfoToUpdate.Mobile != null && userInfoToUpdate.Mobile != mobile)
            {
                if (!string.IsNullOrEmpty(userInfoToUpdate.Mobile) && IsMobileExists(userInfoToUpdate.Mobile))
                {
                    errorMessage = "手机号码已被注册，请更换手机号码";
                    return false;
                }
            }

            return true;
        }

        public bool Insert(IUserInfo userInfo, string password, string ipAddress, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (!InsertValidate(userInfo.UserName, userInfo.Email, userInfo.Mobile, password, ipAddress, out errorMessage)) return false;

            try
            {
                var passwordSalt = GenerateSalt();
                password = EncodePassword(password, EPasswordFormat.Encrypted, passwordSalt);

                InsertWithoutValidation(userInfo, password, EPasswordFormat.Encrypted, passwordSalt);

                IpAddressCache(ipAddress);

                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        private void InsertWithoutValidation(IUserInfo userInfo, string password, EPasswordFormat passwordFormat, string passwordSalt)
        {
            var sqlString = $"INSERT INTO {TableName} (UserName, Password, PasswordFormat, PasswordSalt, CreateDate, LastResetPasswordDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, CountOfWriting, IsChecked, IsLockedOut, DisplayName, Email, Mobile, AvatarUrl, Organization, Department, Position, Gender, Birthday, Education, Graduation, Address, WeiXin, QQ, WeiBo, Interests, Signature) VALUES (@UserName, @Password, @PasswordFormat, @PasswordSalt, @CreateDate, @LastResetPasswordDate, @LastActivityDate, @CountOfLogin, @CountOfFailedLogin, @CountOfWriting, @IsChecked, @IsLockedOut, @DisplayName, @Email, @Mobile, @AvatarUrl, @Organization, @Department, @Position, @Gender, @Birthday, @Education, @Graduation, @Address, @WeiXin, @QQ, @WeiBo, @Interests, @Signature)";

            userInfo.CreateDate = DateTime.Now;
            userInfo.LastActivityDate = DateTime.Now;
            userInfo.LastResetPasswordDate = DateTime.Now;

            var parameters = new IDataParameter[]
            {
                GetParameter(ParmUserName, DataType.VarChar, 255, userInfo.UserName),
                GetParameter(ParmPassword, DataType.VarChar, 255, password),
                GetParameter(ParmPasswordFormat, DataType.VarChar, 50, EPasswordFormatUtils.GetValue(passwordFormat)),
                GetParameter(ParmPasswordSalt, DataType.VarChar, 128, passwordSalt),
                GetParameter(ParmCreateDate, DataType.DateTime, userInfo.CreateDate),
                GetParameter(ParmLastResetPasswordDate, DataType.DateTime, userInfo.LastResetPasswordDate),
                GetParameter(ParmLastActivityDate, DataType.DateTime, userInfo.LastActivityDate),
                GetParameter(ParmCountOfLogin, DataType.Integer, userInfo.CountOfLogin),
                GetParameter(ParmCountOfFailedLogin, DataType.Integer, userInfo.CountOfFailedLogin),
                GetParameter(ParmCountOfWriting, DataType.Integer, userInfo.CountOfWriting),
                GetParameter(ParmIsChecked, DataType.VarChar, 18, userInfo.IsChecked.ToString()),
                GetParameter(ParmIsLockedOut, DataType.VarChar, 18, userInfo.IsLockedOut.ToString()),
                GetParameter(ParmDisplayname, DataType.VarChar, 255, userInfo.DisplayName),
                GetParameter(ParmEmail, DataType.VarChar, 255, userInfo.Email),
                GetParameter(ParmMobile, DataType.VarChar, 20, userInfo.Mobile),
                GetParameter(ParmAvatarUrl, DataType.VarChar, 200, userInfo.AvatarUrl),
                GetParameter(ParmOrganization, DataType.VarChar, 255, userInfo.Organization),
                GetParameter(ParmDepartment, DataType.VarChar, 255, userInfo.Department),
                GetParameter(ParmPosition, DataType.VarChar, 255, userInfo.Position),
                GetParameter(ParmGender, DataType.VarChar, 255, userInfo.Gender),
                GetParameter(ParmBirthday, DataType.VarChar, 50, userInfo.Birthday),
                GetParameter(ParmEducation, DataType.VarChar, 255, userInfo.Education),
                GetParameter(ParmGraduation, DataType.VarChar, 255, userInfo.Graduation),
                GetParameter(ParmAddress, DataType.VarChar, 255, userInfo.Address),
                GetParameter(ParmWeixin, DataType.VarChar, 255, userInfo.WeiXin),
                GetParameter(ParmQq, DataType.VarChar, 255, userInfo.Qq),
                GetParameter(ParmWeibo, DataType.VarChar, 255, userInfo.WeiBo),
                GetParameter(ParmInterests, DataType.VarChar, 255, userInfo.Interests),
                GetParameter(ParmSignature, DataType.VarChar, 255, userInfo.Signature)
            };

            ExecuteNonQuery(sqlString, parameters);
        }

        public bool IsPasswordCorrect(string password, out string errorMessage)
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

        public void Update(IUserInfo userInfo)
        {
            var sqlString = $"UPDATE {TableName} SET UserName = @UserName, CreateDate = @CreateDate, LastResetPasswordDate = @LastResetPasswordDate, LastActivityDate = @LastActivityDate, CountOfLogin = @CountOfLogin, CountOfFailedLogin = @CountOfFailedLogin, CountOfWriting = @CountOfWriting, IsChecked = @IsChecked, IsLockedOut = @IsLockedOut, DisplayName = @DisplayName, Email = @Email, Mobile = @Mobile, AvatarUrl = @AvatarUrl, Organization = @Organization, Department = @Department, Position = @Position, Gender = @Gender, Birthday = @Birthday, Education = @Education, Graduation = @Graduation, Address = @Address, WeiXin = @WeiXin, QQ = @QQ, WeiBo = @WeiBo, Interests = @Interests, Signature = @Signature WHERE Id = @Id";

            var updateParms = new IDataParameter[]
            {
                GetParameter(ParmUserName, DataType.VarChar, 255, userInfo.UserName),
                GetParameter(ParmCreateDate, DataType.DateTime, userInfo.CreateDate),
                GetParameter(ParmLastResetPasswordDate, DataType.DateTime, userInfo.LastResetPasswordDate),
                GetParameter(ParmLastActivityDate, DataType.DateTime, userInfo.LastActivityDate),
                GetParameter(ParmCountOfLogin, DataType.Integer, userInfo.CountOfLogin),
                GetParameter(ParmCountOfFailedLogin, DataType.Integer, userInfo.CountOfFailedLogin),
                GetParameter(ParmCountOfWriting, DataType.Integer, userInfo.CountOfWriting),
                GetParameter(ParmIsChecked, DataType.VarChar, 18, userInfo.IsChecked.ToString()),
                GetParameter(ParmIsLockedOut, DataType.VarChar, 18, userInfo.IsLockedOut.ToString()),
                GetParameter(ParmDisplayname, DataType.VarChar, 255, userInfo.DisplayName),
                GetParameter(ParmEmail, DataType.VarChar, 255, userInfo.Email),
                GetParameter(ParmMobile, DataType.VarChar, 20, userInfo.Mobile),
                GetParameter(ParmAvatarUrl, DataType.VarChar, 200, userInfo.AvatarUrl),
                GetParameter(ParmOrganization, DataType.VarChar, 255, userInfo.Organization),
                GetParameter(ParmDepartment, DataType.VarChar, 255, userInfo.Department),
                GetParameter(ParmPosition, DataType.VarChar, 255, userInfo.Position),
                GetParameter(ParmGender, DataType.VarChar, 255, userInfo.Gender),
                GetParameter(ParmBirthday, DataType.VarChar, 50, userInfo.Birthday),
                GetParameter(ParmEducation, DataType.VarChar, 255, userInfo.Education),
                GetParameter(ParmGraduation, DataType.VarChar, 255, userInfo.Graduation),
                GetParameter(ParmAddress, DataType.VarChar, 255, userInfo.Address),
                GetParameter(ParmWeixin, DataType.VarChar, 255, userInfo.WeiXin),
                GetParameter(ParmQq, DataType.VarChar, 255, userInfo.Qq),
                GetParameter(ParmWeibo, DataType.VarChar, 255, userInfo.WeiBo),
                GetParameter(ParmInterests, DataType.VarChar, 255, userInfo.Interests),
                GetParameter(ParmSignature, DataType.VarChar, 255, userInfo.Signature),
                GetParameter(ParmId, DataType.Integer, userInfo.Id)
            };

            ExecuteNonQuery(sqlString, updateParms);
        }

        public void UpdateLastActivityDate(string userName)
        {
            var sqlString = $"UPDATE {TableName} SET LastActivityDate = @LastActivityDate WHERE UserName = @UserName";

            var updateParms = new IDataParameter[]
            {
                GetParameter(ParmLastActivityDate, DataType.DateTime, DateTime.Now),
                GetParameter(ParmUserName, DataType.VarChar, 255, userName)
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
                var encryptor = new DesEncryptor
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
                var encryptor = new DesEncryptor
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

            var passwordFormat = EPasswordFormat.Encrypted;
            var passwordSalt = GenerateSalt();
            password = EncodePassword(password, passwordFormat, passwordSalt);
            return ChangePassword(userName, passwordFormat, passwordSalt, password);
        }

        private bool ChangePassword(string userName, EPasswordFormat passwordFormat, string passwordSalt, string password)
        {
            var isSuccess = false;

            var sqlString = $"UPDATE {TableName} SET Password = @Password, PasswordFormat = @PasswordFormat, PasswordSalt = @PasswordSalt, LastResetPasswordDate = @LastResetPasswordDate WHERE UserName = @UserName";

            var updateParms = new IDataParameter[]
            {
                GetParameter(ParmPassword, DataType.VarChar, 255, password),
                GetParameter(ParmPasswordFormat, DataType.VarChar, 50, EPasswordFormatUtils.GetValue(passwordFormat)),
                GetParameter(ParmPasswordSalt, DataType.VarChar, 128, passwordSalt),
                GetParameter(ParmLastResetPasswordDate, DataType.DateTime, DateTime.Now),
                GetParameter(ParmUserName, DataType.VarChar, 255, userName)
            };

            try
            {
                ExecuteNonQuery(sqlString, updateParms);
                LogUtils.AddUserLog(userName, "修改密码", string.Empty);
                isSuccess = true;
            }
            catch
            {
                // ignored
            }
            return isSuccess;
        }

        public void Delete(int id)
        {
            var sqlString = $"DELETE FROM {TableName} WHERE Id = @Id";

            var deleteParms = new IDataParameter[]
            {
                GetParameter(ParmId, DataType.Integer, id)
            };

            ExecuteNonQuery(sqlString, deleteParms);
        }

        public void Check(List<int> idList)
        {
            var sqlString =
                $"UPDATE {TableName} SET IsChecked = '{true}' WHERE Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";

            ExecuteNonQuery(sqlString);
        }

        public void Check(int id)
        {
            var sqlString = $"UPDATE {TableName} SET IsChecked = '{true}' WHERE Id = {id}";

            ExecuteNonQuery(sqlString);
        }

        public void Lock(List<int> idList)
        {
            var sqlString =
                $"UPDATE {TableName} SET IsLockedOut = '{true}' WHERE Id IN ({TranslateUtils.ToSqlInStringWithQuote(idList)})";

            ExecuteNonQuery(sqlString);
        }

        public void UnLock(List<int> idList)
        {
            var sqlString =
                $"UPDATE {TableName} SET IsLockedOut = '{false}', CountOfFailedLogin = 0 WHERE Id IN ({TranslateUtils.ToSqlInStringWithQuote(idList)})";

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
                Id = GetInt(rdr, i++),
                UserName = GetString(rdr, i++),
                Password = GetString(rdr, i++),
                PasswordFormat = GetString(rdr, i++),
                PasswordSalt = GetString(rdr, i++),
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
                Signature = GetString(rdr, i)
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
            var sqlString = $"SELECT Id, UserName, Password, PasswordFormat, PasswordSalt, CreateDate, LastResetPasswordDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, CountOfWriting, IsChecked, IsLockedOut, DisplayName, Email, Mobile, AvatarUrl, Organization, Department, Position, Gender, Birthday, Education, Graduation, Address, WeiXin, QQ, WeiBo, Interests, Signature FROM {TableName} WHERE UserName = @UserName";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmUserName, DataType.VarChar, 255, userName)
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
            var sqlString = $"SELECT Id, UserName, Password, PasswordFormat, PasswordSalt, CreateDate, LastResetPasswordDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, CountOfWriting, IsChecked, IsLockedOut, DisplayName, Email, Mobile, AvatarUrl, Organization, Department, Position, Gender, Birthday, Education, Graduation, Address, WeiXin, QQ, WeiBo, Interests, Signature FROM {TableName} WHERE Email = @Email";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmEmail, DataType.VarChar, 255, email)
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
            var sqlString = $"SELECT Id, UserName, Password, PasswordFormat, PasswordSalt, CreateDate, LastResetPasswordDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, CountOfWriting, IsChecked, IsLockedOut, DisplayName, Email, Mobile, AvatarUrl, Organization, Department, Position, Gender, Birthday, Education, Graduation, Address, WeiXin, QQ, WeiBo, Interests, Signature FROM {TableName} WHERE Mobile = @Mobile";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmMobile, DataType.VarChar, 20, mobile)
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
            var sqlString = $"SELECT UserName FROM {TableName} WHERE Mobile = @Mobile";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmMobile, DataType.VarChar, 20, mobile)
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

        public UserInfo GetUserInfo(int id)
        {
            if (id <= 0) return null;

            UserInfo userInfo = null;
            var sqlString = $"SELECT Id, UserName, Password, PasswordFormat, PasswordSalt, CreateDate, LastResetPasswordDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, CountOfWriting, IsChecked, IsLockedOut, DisplayName, Email, Mobile, AvatarUrl, Organization, Department, Position, Gender, Birthday, Education, Graduation, Address, WeiXin, QQ, WeiBo, Interests, Signature FROM {TableName} WHERE Id = @Id";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmId, DataType.Integer, id)
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

        public bool IsUserNameExists(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return false;

            var exists = false;

            var sqlString = $"SELECT Id FROM {TableName} WHERE UserName = @UserName";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmUserName, DataType.VarChar, 255, userName)
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

            var sqlSelect = $"SELECT Email FROM {TableName} WHERE Email = @Email";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmEmail, DataType.VarChar, 200, email)
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
            var sqlString = $"SELECT Mobile FROM {TableName} WHERE Mobile = @Mobile";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmMobile, DataType.VarChar, 20, mobile)
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

        public string GetUserName(int id)
        {
            var userName = string.Empty;

            var sqlString = $"SELECT UserName FROM {TableName} WHERE Id = @Id";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmId, DataType.Integer, id)
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

        public string GetEmail(int id)
        {
            var email = string.Empty;

            var sqlString = $"SELECT Email FROM {TableName} WHERE Id = @Id";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmId, DataType.Integer, id)
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

        public int GetId(string userName)
        {
            var id = 0;

            var sqlString = $"SELECT Id FROM {TableName} WHERE UserName = @UserName";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmUserName, DataType.VarChar, 255, userName)
            };

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    id = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return id;
        }

        public string GetDisplayName(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return string.Empty;
            var displayName = string.Empty;

            var sqlString = $"SELECT DisplayName FROM {TableName} WHERE UserName = @UserName";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmUserName, DataType.VarChar, 255, userName)
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

        public int GetIdByEmailOrMobile(string email, string mobile)
        {
            var id = 0;

            if (!string.IsNullOrEmpty(email))
            {
                var sqlString = $"SELECT Id FROM {TableName} WHERE Email = @Email";

                var parms = new IDataParameter[]
                {
                    GetParameter(ParmEmail, DataType.VarChar, 200, email)
                };

                using (var rdr = ExecuteReader(sqlString, parms))
                {
                    if (rdr.Read())
                    {
                        id = GetInt(rdr, 0);
                    }
                    rdr.Close();
                }
            }
            else if (!string.IsNullOrEmpty(mobile))
            {
                var sqlString = $"SELECT Id FROM {TableName} WHERE Mobile = @Mobile";

                var parms = new IDataParameter[]
                {
                    GetParameter(ParmMobile, DataType.VarChar, 20, mobile)
                };

                using (var rdr = ExecuteReader(sqlString, parms))
                {
                    if (rdr.Read())
                    {
                        id = GetInt(rdr, 0);
                    }
                    rdr.Close();
                }
            }

            return id;
        }

        public string GetMobile(int id)
        {
            var mobile = string.Empty;

            var sqlString = $"SELECT Mobile FROM {TableName} WHERE Id = @Id";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmId, DataType.Integer, id)
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
                sqlString = $"SELECT Mobile FROM {TableName} WHERE Mobile = @Mobile";
                parms = new IDataParameter[]
                {
                    GetParameter(ParmMobile, DataType.VarChar, 20, account)
                };
            }
            else if (StringUtils.IsEmail(account))
            {
                sqlString = $"SELECT Mobile FROM {TableName} WHERE Email = @Email";
                parms = new IDataParameter[]
                {
                    GetParameter(ParmEmail, DataType.VarChar, 200, account)
                };
            }
            else
            {
                sqlString = $"SELECT Mobile FROM {TableName} WHERE UserName = @UserName";
                parms = new IDataParameter[]
                {
                    GetParameter(ParmUserName, DataType.VarChar, 255, account)
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

            using (var rdr = ExecuteReader($"SELECT COUNT(*) AS TotalNum FROM {TableName}"))
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
            var sqlSelect =
                $"SELECT UserName FROM {TableName} WHERE IsChecked = '{isChecked}' ORDER BY Id DESC";

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

        public List<int> GetIdList(bool isChecked)
        {
            var idList = new List<int>();

            var sqlSelect =
                $"SELECT Id FROM {TableName} WHERE IsChecked = '{isChecked}' ORDER BY Id DESC";

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    idList.Add(GetInt(rdr, 0));
                }
                rdr.Close();
            }

            return idList;
        }

        public List<string> GetUserNameList(string searchWord, int dayOfCreate, int dayOfLastActivity, bool isChecked)
        {
            var list = new List<string>();

            var whereString = string.Empty;

            if (dayOfCreate > 0)
            {
                var dateTime = DateTime.Now.AddDays(-dayOfCreate);
                whereString += $" AND (CreateDate >= {SqlUtils.GetComparableDate(dateTime)}) ";
            }
            if (dayOfLastActivity > 0)
            {
                var dateTime = DateTime.Now.AddDays(-dayOfLastActivity);
                whereString += $" AND (LastActivityDate >= {SqlUtils.GetComparableDate(dateTime)}) ";
            }
            if (!string.IsNullOrEmpty(searchWord))
            {
                var word = PageUtils.FilterSql(searchWord);
                whereString += $" AND (UserName LIKE '%{word}%' OR EMAIL LIKE '%{word}%' OR MOBILE = '{word}') ";
            }
            var sqlString =
                $"SELECT UserName FROM {TableName} WHERE IsChecked = '{isChecked}' {whereString} ORDER BY Id DESC";

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
            return DataProvider.DatabaseDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
        }

        public string GetSelectCommand(string searchWord, int dayOfCreate, int dayOfLastActivity, bool isChecked, int loginCount, string searchType)
        {
            var whereBuilder = new StringBuilder();

            if (dayOfCreate > 0)
            {
                whereBuilder.Append(" AND ");

                var dateTime = DateTime.Now.AddDays(-dayOfCreate);
                whereBuilder.Append($"(CreateDate >= {SqlUtils.GetComparableDate(dateTime)})");
            }

            if (dayOfLastActivity > 0)
            {
                whereBuilder.Append(" AND ");

                var dateTime = DateTime.Now.AddDays(-dayOfLastActivity);
                whereBuilder.Append($"(LastActivityDate >= {SqlUtils.GetComparableDate(dateTime)}) ");
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

            return DataProvider.DatabaseDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
        }

        public string GetSortFieldName()
        {
            return "Id";
        }

        public IDataReader GetStlDataSource(int startNum, int totalNum, string orderByString, string whereString)
        {
            string sqlWhereString = $"WHERE IsChecked = '{true}' {whereString}";
            if (string.IsNullOrEmpty(orderByString))
            {
                orderByString = "ORDER BY Id DESC";
            }

            IDataReader enumerable;

            if (startNum <= 1)
            {
                var sqlString = DataProvider.DatabaseDao.GetSelectSqlString(TableName, totalNum, SqlUtils.Asterisk, sqlWhereString, orderByString);
                enumerable = ExecuteReader(sqlString);
            }
            else
            {
                var sqlSelect = DataProvider.DatabaseDao.GetSelectSqlString(TableName, startNum, totalNum, SqlUtils.Asterisk, sqlWhereString, orderByString);
                enumerable = ExecuteReader(sqlSelect);
            }

            return enumerable;
        }

        private bool CheckPassword(string password, bool isPasswordMd5, string dbpassword, EPasswordFormat passwordFormat, string passwordSalt)
        {
            var decodePassword = DecodePassword(dbpassword, passwordFormat, passwordSalt);
            if (isPasswordMd5)
            {
                return password == AuthUtils.Md5ByString(decodePassword);
            }
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
            if (IsUserNameExists(userInfo.UserName))
            {
                return false;
            }
            try
            {
                InsertWithoutValidation(userInfo, userInfo.Password, EPasswordFormatUtils.GetEnumType(userInfo.PasswordFormat), userInfo.PasswordSalt);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void UpdateLastActivityDateAndCountOfFailedLogin(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return;

            var sqlString = $"UPDATE {TableName} SET LastActivityDate = @LastActivityDate, {SqlUtils.ToPlusSqlString("CountOfFailedLogin")} WHERE UserName = @UserName";

            IDataParameter[] updateParms = {
                GetParameter(ParmLastActivityDate, DataType.DateTime, DateTime.Now),
                GetParameter(ParmUserName, DataType.VarChar, 255, userName)
            };

            ExecuteNonQuery(sqlString, updateParms);
        }

        public void UpdateLastActivityDateAndCountOfLogin(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return;

            var sqlString = $"UPDATE {TableName} SET LastActivityDate = @LastActivityDate, {SqlUtils.ToPlusSqlString("CountOfLogin")}, CountOfFailedLogin = 0 WHERE UserName = @UserName";

            IDataParameter[] updateParms = {
                GetParameter(ParmLastActivityDate, DataType.DateTime, DateTime.Now),
                GetParameter(ParmUserName, DataType.VarChar, 255, userName)
            };

            ExecuteNonQuery(sqlString, updateParms);
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

            if (ConfigManager.SystemConfigInfo.IsUserLockLogin)
            {
                if (userInfo.CountOfFailedLogin > 0 && userInfo.CountOfFailedLogin >= ConfigManager.SystemConfigInfo.UserLockLoginCount)
                {
                    var lockType = EUserLockTypeUtils.GetEnumType(ConfigManager.SystemConfigInfo.UserLockLoginType);
                    if (lockType == EUserLockType.Forever)
                    {
                        errorMessage = "此账号错误登录次数过多，已被永久锁定";
                        return false;
                    }
                    if (lockType == EUserLockType.Hours)
                    {
                        var ts = new TimeSpan(DateTime.Now.Ticks - userInfo.LastActivityDate.Ticks);
                        var hours = Convert.ToInt32(ConfigManager.SystemConfigInfo.UserLockLoginHours - ts.TotalHours);
                        if (hours > 0)
                        {
                            errorMessage =
                                $"此账号错误登录次数过多，已被锁定，请等待{hours}小时后重试";
                            return false;
                        }
                    }
                }
            }

            if (!CheckPassword(password, isPasswordMd5, userInfo.Password, EPasswordFormatUtils.GetEnumType(userInfo.PasswordFormat), userInfo.PasswordSalt))
            {
                LogUtils.AddUserLog(userInfo.UserName, "用户登录失败", "帐号或密码错误");
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

        public int ApiGetCount()
        {
            return DataProvider.DatabaseDao.GetCount(TableName);
        }

        public List<UserInfo> ApiGetUsers(int offset, int limit)
        {
            var list = new List<UserInfo>();
            List<UserInfoDatabase> dbList;

            var sqlString =
                DataProvider.DatabaseDao.GetPageSqlString(TableName, "*", string.Empty, "ORDER BY Id", offset, limit);

            using (var connection = GetConnection())
            {
                dbList = connection.Query<UserInfoDatabase>(sqlString).ToList();
            }

            if (dbList.Count > 0)
            {
                foreach (var dbUserInfo in dbList)
                {
                    if (dbUserInfo != null)
                    {
                        list.Add(dbUserInfo.ToUserInfo());
                    }
                }
            }

            return list;
        }

        public UserInfo ApiGetUser(int id)
        {
            UserInfo userInfo = null;

            var sqlString = $"SELECT * FROM {TableName} WHERE Id = @Id";

            using (var connection = GetConnection())
            {
                var dbUserInfo = connection.QuerySingleOrDefault<UserInfoDatabase>(sqlString, new { Id = id });
                if (dbUserInfo != null)
                {
                    userInfo = dbUserInfo.ToUserInfo();
                }
            }

            return userInfo;
        }

        public bool ApiIsExists(int id)
        {
            var sqlString = $"SELECT count(1) FROM {TableName} WHERE Id = @Id";

            using (var connection = GetConnection())
            {
                return connection.ExecuteScalar<bool>(sqlString, new { Id = id });
            }
        }

        public UserInfo ApiUpdate(int id, UserInfoCreateUpdate userInfoToUpdate, out string errorMessage)
        {
            var userInfo = ApiGetUser(id);

            if (!UpdateValidate(userInfoToUpdate, userInfo.UserName, userInfo.Email, userInfo.Mobile, out errorMessage)) return null;

            var dbUserInfo = new UserInfoDatabase(userInfo);

            userInfoToUpdate.Load(dbUserInfo);

            dbUserInfo.Password = userInfo.Password;
            dbUserInfo.PasswordFormat = userInfo.PasswordFormat;
            dbUserInfo.PasswordSalt = userInfo.PasswordSalt;

            using (var connection = GetConnection())
            {
                connection.Update(dbUserInfo);
            }

            return dbUserInfo.ToUserInfo();
        }

        public UserInfo ApiDelete(int id)
        {
            var userInfoToDelete = ApiGetUser(id);

            using (var connection = GetConnection())
            {
                connection.Delete(new UserInfoDatabase(userInfoToDelete));
            }

            return userInfoToDelete;
        }

        public UserInfo ApiInsert(UserInfoCreateUpdate userInfoToInsert, string ipAddress, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                var dbUserInfo = new UserInfoDatabase();

                userInfoToInsert.Load(dbUserInfo);

                if (!InsertValidate(dbUserInfo.UserName, dbUserInfo.Email, dbUserInfo.Mobile, dbUserInfo.Password, ipAddress, out errorMessage)) return null;

                dbUserInfo.PasswordSalt = GenerateSalt();
                dbUserInfo.Password = EncodePassword(dbUserInfo.Password, EPasswordFormatUtils.GetEnumType(dbUserInfo.PasswordFormat), dbUserInfo.PasswordSalt);
                dbUserInfo.CreateDate = DateTime.Now;
                dbUserInfo.LastActivityDate = DateTime.Now;
                dbUserInfo.LastResetPasswordDate = DateTime.Now;

                using (var connection = GetConnection())
                {
                    var identity = connection.Insert(dbUserInfo);
                    if (identity > 0)
                    {
                        dbUserInfo.Id = Convert.ToInt32(identity);
                    }
                }

                IpAddressCache(ipAddress);

                return dbUserInfo.ToUserInfo();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return null;
            }
        }
    }
}

