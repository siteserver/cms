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
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model.Attributes;

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
                AttributeName = nameof(UserInfo.Id),
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfo.UserName),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfo.Password),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfo.PasswordFormat),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfo.PasswordSalt),
                DataType = DataType.VarChar,
                DataLength = 128
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfo.CreateDate),
                DataType = DataType.DateTime
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfo.LastResetPasswordDate),
                DataType = DataType.DateTime
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfo.LastActivityDate),
                DataType = DataType.DateTime
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfo.CountOfLogin),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfo.CountOfFailedLogin),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfo.GroupId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfo.IsChecked),
                DataType = DataType.VarChar,
                DataLength = 18
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfo.IsLockedOut),
                DataType = DataType.VarChar,
                DataLength = 18
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfo.DisplayName),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfo.Email),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfo.Mobile),
                DataType = DataType.VarChar,
                DataLength = 20
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfo.AvatarUrl),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfo.Gender),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfo.Birthday),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfo.WeiXin),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfo.Qq),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfo.WeiBo),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfo.Bio),
                DataType = DataType.Text
            },
            new TableColumn
            {
                AttributeName = nameof(UserInfo.SettingsXml),
                DataType = DataType.Text
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
        private const string ParmGroupId = "@GroupId";
        private const string ParmIsChecked = "@IsChecked";
        private const string ParmIsLockedOut = "@IsLockedOut";
        private const string ParmDisplayname = "@DisplayName";
        private const string ParmEmail = "@Email";
        private const string ParmMobile = "@Mobile";
        private const string ParmAvatarUrl = "@AvatarUrl";
        private const string ParmGender = "@Gender";
        private const string ParmBirthday = "@Birthday";
        private const string ParmWeixin = "@WeiXin";
        private const string ParmQq = "@QQ";
        private const string ParmWeibo = "@WeiBo";
        private const string ParmBio = "@Bio";
        private const string ParmSettingsXml = "@SettingsXml";

        private bool InsertValidate(string userName, string email, string mobile, string password, string ipAddress, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (!UserManager.IsIpAddressCached(ipAddress))
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

        private bool UpdateValidate(Dictionary<string, object> body, string userName, string email, string mobile, out string errorMessage)
        {
            errorMessage = string.Empty;

            var bodyUserName = string.Empty;
            if (body.ContainsKey("userName"))
            {
                bodyUserName = (string) body["userName"];
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

            if (!ConfigManager.SystemConfigInfo.IsUserRegistrationAllowed)
            {
                errorMessage = "对不起，系统已禁止新用户注册！";
                return 0;
            }

            try
            {
                userInfo.IsChecked = ConfigManager.SystemConfigInfo.IsUserRegistrationChecked;
                if (StringUtils.IsMobile(userInfo.UserName) && string.IsNullOrEmpty(userInfo.Mobile))
                {
                    userInfo.Mobile = userInfo.UserName;
                }

                if (!InsertValidate(userInfo.UserName, userInfo.Email, userInfo.Mobile, password, ipAddress, out errorMessage)) return 0;

                var asswordSalt = GenerateSalt();
                password = EncodePassword(password, EPasswordFormat.Encrypted, asswordSalt);
                userInfo.CreateDate = DateTime.Now;
                userInfo.LastActivityDate = DateTime.Now;
                userInfo.LastResetPasswordDate = DateTime.Now;

                userInfo.Id = InsertWithoutValidation(userInfo, password, EPasswordFormat.Encrypted, asswordSalt);

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
            var sqlString = $"INSERT INTO {TableName} (UserName, Password, PasswordFormat, PasswordSalt, CreateDate, LastResetPasswordDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, GroupId, IsChecked, IsLockedOut, DisplayName, Email, Mobile, AvatarUrl, Gender, Birthday, WeiXin, QQ, WeiBo, Bio, SettingsXml) VALUES (@UserName, @Password, @PasswordFormat, @PasswordSalt, @CreateDate, @LastResetPasswordDate, @LastActivityDate, @CountOfLogin, @CountOfFailedLogin, @GroupId, @IsChecked, @IsLockedOut, @DisplayName, @Email, @Mobile, @AvatarUrl, @Gender, @Birthday, @WeiXin, @QQ, @WeiBo, @Bio, @SettingsXml)";

            userInfo.CreateDate = DateTime.Now;
            userInfo.LastActivityDate = DateTime.Now;
            userInfo.LastResetPasswordDate = DateTime.Now;

            userInfo.DisplayName = AttackUtils.FilterXss(userInfo.DisplayName);
            userInfo.Email = AttackUtils.FilterXss(userInfo.Email);
            userInfo.Mobile = AttackUtils.FilterXss(userInfo.Mobile);
            userInfo.AvatarUrl = AttackUtils.FilterXss(userInfo.AvatarUrl);
            userInfo.Gender = AttackUtils.FilterXss(userInfo.Gender);
            userInfo.Birthday = AttackUtils.FilterXss(userInfo.Birthday);
            userInfo.WeiXin = AttackUtils.FilterXss(userInfo.WeiXin);
            userInfo.Qq = AttackUtils.FilterXss(userInfo.Qq);
            userInfo.WeiBo = AttackUtils.FilterXss(userInfo.WeiBo);
            userInfo.Bio = AttackUtils.FilterXss(userInfo.Bio);
            var settingsXml = userInfo.ToString(UserAttribute.AllAttributes.Value);

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
                GetParameter(ParmGroupId, DataType.Integer, userInfo.GroupId),
                GetParameter(ParmIsChecked, DataType.VarChar, 18, userInfo.IsChecked.ToString()),
                GetParameter(ParmIsLockedOut, DataType.VarChar, 18, userInfo.IsLockedOut.ToString()),
                GetParameter(ParmDisplayname, DataType.VarChar, 255, userInfo.DisplayName),
                GetParameter(ParmEmail, DataType.VarChar, 255, userInfo.Email),
                GetParameter(ParmMobile, DataType.VarChar, 20, userInfo.Mobile),
                GetParameter(ParmAvatarUrl, DataType.VarChar, 200, userInfo.AvatarUrl),
                GetParameter(ParmGender, DataType.VarChar, 255, userInfo.Gender),
                GetParameter(ParmBirthday, DataType.VarChar, 50, userInfo.Birthday),
                GetParameter(ParmWeixin, DataType.VarChar, 255, userInfo.WeiXin),
                GetParameter(ParmQq, DataType.VarChar, 255, userInfo.Qq),
                GetParameter(ParmWeibo, DataType.VarChar, 255, userInfo.WeiBo),
                GetParameter(ParmBio, DataType.Text, userInfo.Bio),
                GetParameter(ParmSettingsXml, DataType.Text, settingsXml)
            };

            return ExecuteNonQueryAndReturnId(TableName, UserAttribute.Id, sqlString, parameters);
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

        public UserInfo Update(UserInfo userInfo, Dictionary<string, object> body, out string errorMessage)
        {
            if (!UpdateValidate(body, userInfo.UserName, userInfo.Email, userInfo.Mobile, out errorMessage)) return null;

            userInfo.Load(body);

            Update(userInfo);

            return userInfo;
        }

        public void Update(UserInfo userInfo)
        {
            if (userInfo == null) return;

            userInfo.DisplayName = AttackUtils.FilterXss(userInfo.DisplayName);
            userInfo.Email = AttackUtils.FilterXss(userInfo.Email);
            userInfo.Mobile = AttackUtils.FilterXss(userInfo.Mobile);
            userInfo.AvatarUrl = AttackUtils.FilterXss(userInfo.AvatarUrl);
            userInfo.Gender = AttackUtils.FilterXss(userInfo.Gender);
            userInfo.Birthday = AttackUtils.FilterXss(userInfo.Birthday);
            userInfo.WeiXin = AttackUtils.FilterXss(userInfo.WeiXin);
            userInfo.Qq = AttackUtils.FilterXss(userInfo.Qq);
            userInfo.WeiBo = AttackUtils.FilterXss(userInfo.WeiBo);
            userInfo.Bio = AttackUtils.FilterXss(userInfo.Bio);

            var sqlString = $"UPDATE {TableName} SET UserName = @UserName, CreateDate = @CreateDate, LastResetPasswordDate = @LastResetPasswordDate, LastActivityDate = @LastActivityDate, CountOfLogin = @CountOfLogin, CountOfFailedLogin = @CountOfFailedLogin, GroupId = @GroupId, IsChecked = @IsChecked, IsLockedOut = @IsLockedOut, DisplayName = @DisplayName, Email = @Email, Mobile = @Mobile, AvatarUrl = @AvatarUrl, Gender = @Gender, Birthday = @Birthday, WeiXin = @WeiXin, QQ = @QQ, WeiBo = @WeiBo, Bio = @Bio, SettingsXml = @SettingsXml WHERE Id = @Id";

            var updateParms = new IDataParameter[]
            {
                GetParameter(ParmUserName, DataType.VarChar, 255, userInfo.UserName),
                GetParameter(ParmCreateDate, DataType.DateTime, userInfo.CreateDate),
                GetParameter(ParmLastResetPasswordDate, DataType.DateTime, userInfo.LastResetPasswordDate),
                GetParameter(ParmLastActivityDate, DataType.DateTime, userInfo.LastActivityDate),
                GetParameter(ParmCountOfLogin, DataType.Integer, userInfo.CountOfLogin),
                GetParameter(ParmCountOfFailedLogin, DataType.Integer, userInfo.CountOfFailedLogin),
                GetParameter(ParmGroupId, DataType.Integer, userInfo.GroupId),
                GetParameter(ParmIsChecked, DataType.VarChar, 18, userInfo.IsChecked.ToString()),
                GetParameter(ParmIsLockedOut, DataType.VarChar, 18, userInfo.IsLockedOut.ToString()),
                GetParameter(ParmDisplayname, DataType.VarChar, 255, userInfo.DisplayName),
                GetParameter(ParmEmail, DataType.VarChar, 255, userInfo.Email),
                GetParameter(ParmMobile, DataType.VarChar, 20, userInfo.Mobile),
                GetParameter(ParmAvatarUrl, DataType.VarChar, 200, userInfo.AvatarUrl),
                GetParameter(ParmGender, DataType.VarChar, 255, userInfo.Gender),
                GetParameter(ParmBirthday, DataType.VarChar, 50, userInfo.Birthday),
                GetParameter(ParmWeixin, DataType.VarChar, 255, userInfo.WeiXin),
                GetParameter(ParmQq, DataType.VarChar, 255, userInfo.Qq),
                GetParameter(ParmWeibo, DataType.VarChar, 255, userInfo.WeiBo),
                GetParameter(ParmBio, DataType.Text, userInfo.Bio),
                GetParameter(ParmSettingsXml, DataType.Text, userInfo.ToString(UserAttribute.AllAttributes.Value)),
                GetParameter(ParmId, DataType.Integer, userInfo.Id)
            };

            ExecuteNonQuery(sqlString, updateParms);

            UserManager.UpdateCache(userInfo);
        }

        private void UpdateLastActivityDateAndCountOfFailedLogin(UserInfo userInfo)
        {
            if (userInfo == null) return;

            userInfo.LastActivityDate = DateTime.Now;
            userInfo.CountOfFailedLogin += 1;

            var sqlString = $"UPDATE {TableName} SET LastActivityDate = @LastActivityDate, CountOfFailedLogin = @CountOfFailedLogin WHERE Id = @Id";

            IDataParameter[] updateParms = {
                GetParameter(ParmLastActivityDate, DataType.DateTime, userInfo.LastActivityDate),
                GetParameter(ParmCountOfFailedLogin, DataType.Integer, userInfo.CountOfFailedLogin),
                GetParameter(ParmId, DataType.Integer, userInfo.Id)
            };

            ExecuteNonQuery(sqlString, updateParms);

            UserManager.UpdateCache(userInfo);
        }

        public void UpdateLastActivityDateAndCountOfLogin(UserInfo userInfo)
        {
            if (userInfo == null) return;

            userInfo.LastActivityDate = DateTime.Now;
            userInfo.CountOfLogin += 1;
            userInfo.CountOfFailedLogin = 0;

            var sqlString = $"UPDATE {TableName} SET LastActivityDate = @LastActivityDate, CountOfLogin = @CountOfLogin, CountOfFailedLogin = @CountOfFailedLogin WHERE Id = @Id";

            IDataParameter[] updateParms = {
                GetParameter(ParmLastActivityDate, DataType.DateTime, userInfo.LastActivityDate),
                GetParameter(ParmCountOfLogin, DataType.Integer, userInfo.CountOfLogin),
                GetParameter(ParmCountOfFailedLogin, DataType.Integer, userInfo.CountOfFailedLogin),
                GetParameter(ParmId, DataType.Integer, userInfo.Id)
            };

            ExecuteNonQuery(sqlString, updateParms);

            UserManager.UpdateCache(userInfo);
        }

        private string EncodePassword(string password, EPasswordFormat passwordFormat, string passwordSalt)
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

        private string DecodePassword(string password, EPasswordFormat passwordFormat, string passwordSalt)
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

            var sqlString = $"UPDATE {TableName} SET Password = @Password, PasswordFormat = @PasswordFormat, PasswordSalt = @PasswordSalt, LastResetPasswordDate = @LastResetPasswordDate WHERE UserName = @UserName";

            var updateParms = new IDataParameter[]
            {
                GetParameter(ParmPassword, DataType.VarChar, 255, userInfo.Password),
                GetParameter(ParmPasswordFormat, DataType.VarChar, 50, userInfo.PasswordFormat),
                GetParameter(ParmPasswordSalt, DataType.VarChar, 128, userInfo.PasswordSalt),
                GetParameter(ParmLastResetPasswordDate, DataType.DateTime, userInfo.LastResetPasswordDate),
                GetParameter(ParmUserName, DataType.VarChar, 255, userName)
            };

            ExecuteNonQuery(sqlString, updateParms);
            LogUtils.AddUserLog(userName, "修改密码", string.Empty);

            UserManager.UpdateCache(userInfo);
        }

        public void Check(List<int> idList)
        {
            var sqlString =
                $"UPDATE {TableName} SET IsChecked = '{true}' WHERE Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";

            ExecuteNonQuery(sqlString);

            UserManager.ClearCache();
        }

        public void Lock(List<int> idList)
        {
            var sqlString =
                $"UPDATE {TableName} SET IsLockedOut = '{true}' WHERE Id IN ({TranslateUtils.ToSqlInStringWithQuote(idList)})";

            ExecuteNonQuery(sqlString);

            UserManager.ClearCache();
        }

        public void UnLock(List<int> idList)
        {
            var sqlString =
                $"UPDATE {TableName} SET IsLockedOut = '{false}', CountOfFailedLogin = 0 WHERE Id IN ({TranslateUtils.ToSqlInStringWithQuote(idList)})";

            ExecuteNonQuery(sqlString);

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

            UserInfo userInfo = null;
            var sqlString = $"SELECT Id, UserName, Password, PasswordFormat, PasswordSalt, CreateDate, LastResetPasswordDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, GroupId, IsChecked, IsLockedOut, DisplayName, Email, Mobile, AvatarUrl, Gender, Birthday, WeiXin, QQ, WeiBo, Bio, SettingsXml FROM {TableName} WHERE UserName = @UserName";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmUserName, DataType.VarChar, 255, userName)
            };

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    userInfo = new UserInfo(rdr);
                }
                rdr.Close();
            }

            UserManager.UpdateCache(userInfo);

            return userInfo;
        }

        public UserInfo GetByEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return null;

            UserInfo userInfo = null;
            var sqlString = $"SELECT Id, UserName, Password, PasswordFormat, PasswordSalt, CreateDate, LastResetPasswordDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, GroupId, IsChecked, IsLockedOut, DisplayName, Email, Mobile, AvatarUrl, Gender, Birthday, WeiXin, QQ, WeiBo, Bio, SettingsXml FROM {TableName} WHERE Email = @Email";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmEmail, DataType.VarChar, 255, email)
            };

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    userInfo = new UserInfo(rdr);
                }
                rdr.Close();
            }

            UserManager.UpdateCache(userInfo);

            return userInfo;
        }

        public UserInfo GetByMobile(string mobile)
        {
            if (string.IsNullOrEmpty(mobile)) return null;

            UserInfo userInfo = null;
            var sqlString = $"SELECT Id, UserName, Password, PasswordFormat, PasswordSalt, CreateDate, LastResetPasswordDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, GroupId, IsChecked, IsLockedOut, DisplayName, Email, Mobile, AvatarUrl, Gender, Birthday, WeiXin, QQ, WeiBo, Bio, SettingsXml FROM {TableName} WHERE Mobile = @Mobile";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmMobile, DataType.VarChar, 20, mobile)
            };

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    userInfo = new UserInfo(rdr);
                }
                rdr.Close();
            }

            UserManager.UpdateCache(userInfo);

            return userInfo;
        }

        public UserInfo GetByUserId(int id)
        {
            if (id <= 0) return null;

            UserInfo userInfo = null;
            var sqlString = $"SELECT Id, UserName, Password, PasswordFormat, PasswordSalt, CreateDate, LastResetPasswordDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, GroupId, IsChecked, IsLockedOut, DisplayName, Email, Mobile, AvatarUrl, Gender, Birthday, WeiXin, QQ, WeiBo, Bio, SettingsXml FROM {TableName} WHERE Id = @Id";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmId, DataType.Integer, id)
            };

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    userInfo = new UserInfo(rdr);
                }
                rdr.Close();
            }

            UserManager.UpdateCache(userInfo);

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

            var exists = IsUserNameExists(mobile);
            if (exists) return true;

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

        public string GetSelectCommand()
        {
            return DataProvider.DatabaseDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, string.Empty);
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

            return DataProvider.DatabaseDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
        }

        public bool CheckPassword(string password, bool isPasswordMd5, string dbpassword, EPasswordFormat passwordFormat, string passwordSalt)
        {
            var decodePassword = DecodePassword(dbpassword, passwordFormat, passwordSalt);
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

            if (!userInfo.IsChecked)
            {
                errorMessage = "此账号未审核，无法登录";
                return null;
            }

            if (userInfo.IsLockedOut)
            {
                errorMessage = "此账号被锁定，无法登录";
                return null;
            }

            if (ConfigManager.SystemConfigInfo.IsUserLockLogin)
            {
                if (userInfo.CountOfFailedLogin > 0 && userInfo.CountOfFailedLogin >= ConfigManager.SystemConfigInfo.UserLockLoginCount)
                {
                    var lockType = EUserLockTypeUtils.GetEnumType(ConfigManager.SystemConfigInfo.UserLockLoginType);
                    if (lockType == EUserLockType.Forever)
                    {
                        errorMessage = "此账号错误登录次数过多，已被永久锁定";
                        return null;
                    }
                    if (lockType == EUserLockType.Hours)
                    {
                        var ts = new TimeSpan(DateTime.Now.Ticks - userInfo.LastActivityDate.Ticks);
                        var hours = Convert.ToInt32(ConfigManager.SystemConfigInfo.UserLockLoginHours - ts.TotalHours);
                        if (hours > 0)
                        {
                            errorMessage =
                                $"此账号错误登录次数过多，已被锁定，请等待{hours}小时后重试";
                            return null;
                        }
                    }
                }
            }

            if (!CheckPassword(password, isPasswordMd5, userInfo.Password, EPasswordFormatUtils.GetEnumType(userInfo.PasswordFormat), userInfo.PasswordSalt))
            {
                DataProvider.UserDao.UpdateLastActivityDateAndCountOfFailedLogin(userInfo);
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

        public int GetCount()
        {
            return DataProvider.DatabaseDao.GetCount(TableName);
        }

        public List<UserInfo> GetUsers(int offset, int limit)
        {
            var list = new List<UserInfo>();
            List<int> dbList;

            var sqlString =
                DataProvider.DatabaseDao.GetPageSqlString(TableName, "Id", string.Empty, "ORDER BY Id", offset, limit);

            using (var connection = GetConnection())
            {
                dbList = connection.Query<int>(sqlString).ToList();
            }

            if (dbList.Count > 0)
            {
                foreach (var userId in dbList)
                {
                    list.Add(UserManager.GetUserInfoByUserId(userId));
                }
            }

            return list;
        }

        public bool IsExists(int id)
        {
            var sqlString = $"SELECT COUNT(1) FROM {TableName} WHERE Id = @Id";

            using (var connection = GetConnection())
            {
                return connection.ExecuteScalar<bool>(sqlString, new { Id = id });
            }
        }

        public void Delete(UserInfo userInfo)
        {
            var sqlString = $"DELETE FROM {TableName} WHERE Id = @Id";

            var deleteParms = new IDataParameter[]
            {
                GetParameter(ParmId, DataType.Integer, userInfo.Id)
            };

            ExecuteNonQuery(sqlString, deleteParms);

            UserManager.RemoveCache(userInfo);
        }
    }
}

