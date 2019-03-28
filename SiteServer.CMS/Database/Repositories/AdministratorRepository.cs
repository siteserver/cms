using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.Plugin;
using SiteServer.Utils;
using SiteServer.Utils.Auth;
using SiteServer.Utils.Enumerations;
using SqlKata;

namespace SiteServer.CMS.Database.Repositories
{
    public class AdministratorRepository : GenericRepository<AdministratorInfo>
    {
        private static class Attr
        {
            public const string Id = nameof(AdministratorInfo.Id);
            public const string UserName = nameof(AdministratorInfo.UserName);
            public const string DepartmentId = nameof(AdministratorInfo.DepartmentId);
            public const string AreaId = nameof(AdministratorInfo.AreaId);
            public const string Mobile = nameof(AdministratorInfo.Mobile);
            public const string Email = nameof(AdministratorInfo.Email);
            public const string Password = nameof(AdministratorInfo.Password);
            public const string PasswordFormat = nameof(AdministratorInfo.PasswordFormat);
            public const string PasswordSalt = nameof(AdministratorInfo.PasswordSalt);
            public const string IsLockedOut = "IsLockedOut";
        }

        public int GetCount(Query query)
        {
            return Count(query);
        }

        public int GetCount()
        {
            return Count();
        }

        public IList<AdministratorInfo> GetAll(Query query)
        {
            return GetObjectList(query);
        }

        public int Insert(AdministratorInfo adminInfo, out string errorMessage)
        {
            if (!InsertValidate(adminInfo.UserName, adminInfo.Password, adminInfo.Email, adminInfo.Mobile, out errorMessage)) return 0;

            try
            {
                adminInfo.CreationDate = DateTime.Now;
                adminInfo.PasswordFormat = EPasswordFormatUtils.GetValue(EPasswordFormat.Encrypted);
                adminInfo.Password = EncodePassword(adminInfo.Password, EPasswordFormatUtils.GetEnumType(adminInfo.PasswordFormat), out var passwordSalt);
                adminInfo.PasswordSalt = passwordSalt;

                var identity = InsertObject(adminInfo);

                //IDataParameter[] parameters =
                //{
                //    GetParameter(ParamUsername, adminInfo.UserName),
                //    GetParameter(ParamPassword, adminInfo.Password),
                //    GetParameter(ParamPasswordFormat, adminInfo.PasswordFormat),
                //    GetParameter(ParamPasswordSalt, adminInfo.PasswordSalt),
                //    GetParameter(ParamCreationDate, adminInfo.CreationDate),
                //    GetParameter(ParamLastActivityDate, adminInfo.LastActivityDate),
                //    GetParameter(ParamCountOfLogin, adminInfo.CountOfLogin),
                //    GetParameter(ParamCountOfFailedLogin, adminInfo.CountOfFailedLogin),
                //    GetParameter(ParamCreatorUsername, adminInfo.CreatorUserName),
                //    GetParameter(ParamIsLockedOut, adminInfo.IsLockedOut.ToString()),
                //    GetParameter(ParamSiteIdCollection, adminInfo.SiteIdCollection),
                //    GetParameter(ParamSiteId, adminInfo.SiteId),
                //    GetParameter(ParamDepartmentId, adminInfo.DepartmentId),
                //    GetParameter(ParamAreaId, adminInfo.AreaId),
                //    GetParameter(ParamDisplayName, adminInfo.DisplayName),
                //    GetParameter(ParamMobile, adminInfo.Mobile),
                //    GetParameter(ParamEmail, adminInfo.Email),
                //    GetParameter(ParamAvatarUrl, adminInfo.AvatarUrl)
                //};

                //DatabaseApi.ExecuteNonQuery(ConnectionString, SqlInsertUser, parameters);

                if (identity <= 0) return 0;

                DataProvider.Department.UpdateCountOfAdmin();
                DataProvider.Area.UpdateCountOfAdmin();

                //var roles = new[] { EPredefinedRoleUtils.GetValueById(EPredefinedRole.Administrator) };
                //DataProvider.AdministratorsInRoles.AddUserToRoles(adminInfo.UserName, roles);

                DataProvider.AdministratorsInRoles.AddUserToRole(adminInfo.UserName, EPredefinedRoleUtils.GetValue(EPredefinedRole.Administrator));
                
                return identity;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return 0;
            }
        }

        public bool Update(AdministratorInfo administratorInfo, out string errorMessage)
        {
            var adminInfo = AdminManager.GetAdminInfoByUserId(administratorInfo.Id);

            administratorInfo.Password = adminInfo.Password;
            administratorInfo.PasswordFormat = adminInfo.PasswordFormat;
            administratorInfo.PasswordSalt = adminInfo.PasswordSalt;

            if (!UpdateValidate(administratorInfo, adminInfo.UserName, adminInfo.Email, adminInfo.Mobile, out errorMessage)) return false;

            var updated = UpdateObject(administratorInfo);

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamLastActivityDate, administratorInfo.LastActivityDate),
            //    GetParameter(ParamCountOfLogin, administratorInfo.CountOfLogin),
            //    GetParameter(ParamCountOfFailedLogin, administratorInfo.CountOfFailedLogin),
            //    GetParameter(ParamIsLockedOut, administratorInfo.IsLockedOut.ToString()),
            //    GetParameter(ParamSiteIdCollection, administratorInfo.SiteIdCollection),
            //    GetParameter(ParamSiteId, administratorInfo.SiteId),
            //    GetParameter(ParamDepartmentId, administratorInfo.DepartmentId),
            //    GetParameter(ParamAreaId, administratorInfo.AreaId),
            //    GetParameter(ParamDisplayName, administratorInfo.DisplayName),
            //    GetParameter(ParamMobile, administratorInfo.Mobile),
            //    GetParameter(ParamEmail, administratorInfo.Email),
            //    GetParameter(ParamAvatarUrl, administratorInfo.AvatarUrl),
            //    GetParameter(ParamUsername, administratorInfo.UserName)
            //};

            //DatabaseApi.ExecuteNonQuery(ConnectionString, SqlUpdateUser, parameters);

            if (updated)
            {
                DataProvider.Department.UpdateCountOfAdmin();
                DataProvider.Area.UpdateCountOfAdmin();

                AdminManager.UpdateCache(administratorInfo);
            }

            return updated;
        }

        public bool UpdateLastActivityDateAndCountOfFailedLogin(AdministratorInfo adminInfo)
        {
            if (adminInfo == null) return false;

            adminInfo.LastActivityDate = DateTime.Now;
            adminInfo.CountOfFailedLogin += 1;

            //var sqlString = $"UPDATE {TableName} SET LastActivityDate = @LastActivityDate, CountOfFailedLogin = @CountOfFailedLogin WHERE Id = @Id";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamLastActivityDate, adminInfo.LastActivityDate),
            //    GetParameter(ParamCountOfFailedLogin, adminInfo.CountOfFailedLogin),
            //    GetParameter(ParamId, adminInfo.Id)
            //};

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

            var updated = Update(adminInfo, out _);
            if (updated)
            {
                AdminManager.UpdateCache(adminInfo);
            }
            return updated;
        }

        public void UpdateLastActivityDateAndCountOfLogin(AdministratorInfo adminInfo)
        {
            if (adminInfo == null) return;

            adminInfo.LastActivityDate = DateTime.Now;
            adminInfo.CountOfLogin += 1;
            adminInfo.CountOfFailedLogin = 0;

            //var sqlString =
            //    $"UPDATE {TableName} SET LastActivityDate = @LastActivityDate, CountOfLogin = @CountOfLogin, CountOfFailedLogin = @CountOfFailedLogin WHERE Id = @Id";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamLastActivityDate, adminInfo.LastActivityDate),
            //    GetParameter(ParamCountOfLogin, adminInfo.CountOfLogin),
            //    GetParameter(ParamCountOfFailedLogin, adminInfo.CountOfFailedLogin),
            //    GetParameter(ParamId, adminInfo.Id)
            //};

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

            var updated = Update(adminInfo, out _);
            if (updated)
            {
                AdminManager.UpdateCache(adminInfo);
            }
        }

        public void UpdateSiteIdCollection(AdministratorInfo adminInfo, string siteIdCollection)
        {
            if (adminInfo == null) return;

            adminInfo.SiteIdCollection = siteIdCollection;

            //var sqlString = $"UPDATE {TableName} SET SiteIdCollection = @SiteIdCollection WHERE Id = @Id";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamSiteIdCollection, adminInfo.SiteIdCollection),
            //    GetParameter(ParamId, adminInfo.Id)
            //};

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

            var updated = Update(adminInfo, out _);
            if (updated)
            {
                AdminManager.UpdateCache(adminInfo);
            }
        }

        public List<int> UpdateSiteId(AdministratorInfo adminInfo, int siteId)
        {
            if (adminInfo == null) return null;

            var siteIdListLatestAccessed = TranslateUtils.StringCollectionToIntList(adminInfo.SiteIdCollection);
            if (adminInfo.SiteId != siteId || siteIdListLatestAccessed.FirstOrDefault() != siteId)
            {
                siteIdListLatestAccessed.Remove(siteId);
                siteIdListLatestAccessed.Insert(0, siteId);

                adminInfo.SiteIdCollection = TranslateUtils.ObjectCollectionToString(siteIdListLatestAccessed);
                adminInfo.SiteId = siteId;

                //var sqlString =
                //    $"UPDATE {TableName} SET SiteIdCollection = @SiteIdCollection, SiteId = @SiteId WHERE Id = @Id";

                //IDataParameter[] parameters =
                //{
                //    GetParameter(ParamSiteIdCollection, adminInfo.SiteIdCollection),
                //    GetParameter(ParamSiteId, adminInfo.SiteId),
                //    GetParameter(ParamId, adminInfo.Id)
                //};

                //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

                //AdminManager.UpdateCache(adminInfo);

                if (Update(adminInfo, out _))
                {
                    AdminManager.UpdateCache(adminInfo);
                }
            }

            return siteIdListLatestAccessed;
        }

        private void ChangePassword(AdministratorInfo adminInfo, EPasswordFormat passwordFormat, string passwordSalt, string password)
        {
            adminInfo.Password = password;
            adminInfo.PasswordFormat = EPasswordFormatUtils.GetValue(passwordFormat);
            adminInfo.PasswordSalt = passwordSalt;

            //UpdateValue(new Dictionary<string, object>
            //{
            //    {Attr.Password, adminInfo.Password},
            //    {Attr.PasswordFormat, adminInfo.PasswordFormat},
            //    {Attr.PasswordSalt, adminInfo.PasswordSalt}
            //}, Q.Where(nameof(Attr.Id), adminInfo.Id));

            UpdateObject(adminInfo, Attr.Password, Attr.PasswordFormat, Attr.PasswordSalt);

            //var sqlString =
            //    $"UPDATE {TableName} SET Password = @Password, PasswordFormat = @PasswordFormat, PasswordSalt = @PasswordSalt WHERE Id = @Id";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamPassword, adminInfo.Password),
            //    GetParameter(ParamPasswordFormat, adminInfo.PasswordFormat),
            //    GetParameter(ParamPasswordSalt, adminInfo.PasswordSalt),
            //    GetParameter(ParamId, adminInfo.Id)
            //};

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

            AdminManager.RemoveCache(adminInfo);
        }

        public bool Delete(AdministratorInfo adminInfo)
        {
            if (adminInfo == null) return false;

            var deleted = DeleteById(adminInfo.Id);

            //var sqlString = $"DELETE FROM {TableName} WHERE Id = @Id";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamId, adminInfo.Id)
            //};

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

            if (deleted)
            {
                AdminManager.RemoveCache(adminInfo);

                DataProvider.AdministratorsInRoles.RemoveUser(adminInfo.UserName);
                DataProvider.Department.UpdateCountOfAdmin();
                DataProvider.Area.UpdateCountOfAdmin();
            }

            return deleted;
        }

        public void Lock(List<int> userIdList)
        {
            //var sqlString =
            //    $"UPDATE {TableName} SET IsLockedOut = '{true}' WHERE UserName IN ({TranslateUtils.ToSqlInStringWithQuote(userNameList)})";

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);

            //UpdateValue(new Dictionary<string, object>
            //{
            //    {Attr.IsLockedOut, true.ToString()}
            //}, Q.WhereIn(Attr.Id, userIdList));

            UpdateAll(Q
                .Set(Attr.IsLockedOut, true.ToString())
                .WhereIn(Attr.Id, userIdList)
            );

            AdminManager.ClearCache();
        }

        public void UnLock(List<int> userIdList)
        {
            //var sqlString =
            //    $"UPDATE {TableName} SET IsLockedOut = '{false}', CountOfFailedLogin = 0 WHERE UserName IN ({TranslateUtils.ToSqlInStringWithQuote(userNameList)})";

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);

            //UpdateValue(new Dictionary<string, object>
            //{
            //    {Attr.IsLockedOut, false.ToString()}
            //}, Q.WhereIn(Attr.Id, userIdList));

            UpdateAll(Q
                .Set(Attr.IsLockedOut, false.ToString())
                .WhereIn(Attr.Id, userIdList)
            );

            AdminManager.ClearCache();
        }

        private AdministratorInfo GetByAccount(string account)
        {
            var administratorInfo = GetByUserName(account);
            if (administratorInfo != null) return administratorInfo;
            if (StringUtils.IsMobile(account)) return GetByMobile(account);
            if (StringUtils.IsEmail(account)) return GetByEmail(account);

            return null;
        }

        public AdministratorInfo GetByUserId(int userId)
        {
            return GetObjectById(userId);
            //if (userId <= 0) return null;

            //AdministratorInfo info = null;

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamId, userId)
            //};

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectUserByUserId, parameters))
            //{
            //    if (rdr.Read())
            //    {
            //        var i = 0;
            //        info = new AdministratorInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++),
            //            DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetDateTime(rdr, i++), DatabaseApi.GetDateTime(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++),
            //            DatabaseApi.GetString(rdr, i++), TranslateUtils.ToBool(DatabaseApi.GetString(rdr, i++)), DatabaseApi.GetString(rdr, i++),
            //            DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++),
            //            DatabaseApi.GetString(rdr, i));
            //    }
            //    rdr.Close();
            //}

            //return info;
        }

        public AdministratorInfo GetByUserName(string userName)
        {
            return GetObject(Q.Where(Attr.UserName, userName));

            //AdministratorInfo info = null;

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamUsername, userName)
            //};

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectUserByUserName, parameters))
            //{
            //    if (rdr.Read())
            //    {
            //        var i = 0;
            //        info = new AdministratorInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++),
            //            DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetDateTime(rdr, i++), DatabaseApi.GetDateTime(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++),
            //            DatabaseApi.GetString(rdr, i++), TranslateUtils.ToBool(DatabaseApi.GetString(rdr, i++)), DatabaseApi.GetString(rdr, i++),
            //            DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++),
            //            DatabaseApi.GetString(rdr, i));
            //    }
            //    rdr.Close();
            //}

            //return info;
        }

        public AdministratorInfo GetByMobile(string mobile)
        {
            return GetObject(Q.Where(Attr.Mobile, mobile));

            //if (string.IsNullOrEmpty(mobile)) return null;

            //AdministratorInfo info = null;

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamMobile, mobile)
            //};

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectUserByMobile, parameters))
            //{
            //    if (rdr.Read())
            //    {
            //        var i = 0;
            //        info = new AdministratorInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++),
            //            DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetDateTime(rdr, i++), DatabaseApi.GetDateTime(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++),
            //            DatabaseApi.GetString(rdr, i++), TranslateUtils.ToBool(DatabaseApi.GetString(rdr, i++)), DatabaseApi.GetString(rdr, i++),
            //            DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++),
            //            DatabaseApi.GetString(rdr, i));
            //    }
            //    rdr.Close();
            //}

            //return info;
        }

        public AdministratorInfo GetByEmail(string email)
        {
            return GetObject(Q.Where(Attr.Email, email));
            //if (string.IsNullOrEmpty(email)) return null;

            //AdministratorInfo info = null;

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamEmail, email)
            //};

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectUserByEmail, parameters))
            //{
            //    if (rdr.Read())
            //    {
            //        var i = 0;
            //        info = new AdministratorInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++),
            //            DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetDateTime(rdr, i++), DatabaseApi.GetDateTime(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++),
            //            DatabaseApi.GetString(rdr, i++), TranslateUtils.ToBool(DatabaseApi.GetString(rdr, i++)), DatabaseApi.GetString(rdr, i++),
            //            DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++),
            //            DatabaseApi.GetString(rdr, i));
            //    }
            //    rdr.Close();
            //}

            //return info;
        }

        public bool IsUserNameExists(string adminName)
        {
            return Exists(Q.Where(Attr.UserName, adminName));
            //if (string.IsNullOrEmpty(adminName))
            //{
            //    return false;
            //}

            //var exists = false;

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamUsername, adminName)
            //};

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectUsername, parameters))
            //{
            //    if (rdr.Read() && !rdr.IsDBNull(0))
            //    {
            //        exists = true;
            //    }
            //    rdr.Close();
            //}
            //return exists;
        }

        public bool IsEmailExists(string email)
        {
            //if (string.IsNullOrEmpty(email)) return false;

            var exists = IsUserNameExists(email);
            if (exists) return true;

            return Exists(Q
                .Where(Attr.Email, email));

            //var sqlSelect = $"SELECT {nameof(AdministratorInfo.Email)} FROM {TableName} WHERE {nameof(AdministratorInfo.Email)} = @{nameof(AdministratorInfo.Email)}";

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

            //return exists;
        }

        public bool IsMobileExists(string mobile)
        {
            //if (string.IsNullOrEmpty(mobile)) return false;

            var exists = IsUserNameExists(mobile);
            if (exists) return true;

            return Exists(Q
                .Where(Attr.Mobile, mobile));

            //var sqlString = $"SELECT {nameof(AdministratorInfo.Mobile)} FROM {TableName} WHERE {nameof(AdministratorInfo.Mobile)} = @{nameof(AdministratorInfo.Mobile)}";

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
        }

        public int GetCountByAreaId(int areaId)
        {
            return Count(Q
                .Where(Attr.AreaId, areaId));
            //var sqlString = $"SELECT COUNT(*) FROM {TableName} WHERE {nameof(AdministratorInfo.AreaId)} = {areaId}";
            
            //return DatabaseApi.Instance.GetIntResult(sqlString);
        }

        public int GetCountByDepartmentId(int departmentId)
        {
            return Count(Q
                .Where(Attr.DepartmentId, departmentId));
            //var sqlString = $"SELECT COUNT(*) FROM {TableName} WHERE {nameof(AdministratorInfo.DepartmentId)} = {departmentId}";

            //return DatabaseApi.Instance.GetIntResult(sqlString);
        }

        public IList<string> GetUserNameList()
        {
            return GetValueList<string>(Q.Select(Attr.UserName));
            //var list = new List<string>();
            //var sqlSelect = $"SELECT UserName FROM {TableName}";

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlSelect))
            //{
            //    while (rdr.Read())
            //    {
            //        list.Add(DatabaseApi.GetString(rdr, 0));
            //    }
            //    rdr.Close();
            //}
            //return list;
        }

        public IList<string> GetUserNameList(int departmentId)
        {
            return GetValueList<string>(Q
                .Select(Attr.UserName)
                .Where(Attr.DepartmentId, departmentId));

            //var list = new List<string>();
            //var sqlSelect = $"SELECT UserName FROM {TableName} WHERE DepartmentId = {departmentId}";

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlSelect))
            //{
            //    while (rdr.Read())
            //    {
            //        list.Add(DatabaseApi.GetString(rdr, 0));
            //    }
            //    rdr.Close();
            //}
            //return list;
        }

        private bool UpdateValidate(IAdministratorInfo adminInfoToUpdate, string userName, string email, string mobile, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (adminInfoToUpdate.UserName != null && adminInfoToUpdate.UserName != userName)
            {
                if (string.IsNullOrEmpty(adminInfoToUpdate.UserName))
                {
                    errorMessage = "用户名不能为空";
                    return false;
                }
                if (adminInfoToUpdate.UserName.Length < ConfigManager.Instance.AdminUserNameMinLength)
                {
                    errorMessage = $"用户名长度必须大于等于{ConfigManager.Instance.AdminUserNameMinLength}";
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
            if (userName.Length < ConfigManager.Instance.AdminUserNameMinLength)
            {
                errorMessage = $"用户名长度必须大于等于{ConfigManager.Instance.AdminUserNameMinLength}";
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
            if (password.Length < ConfigManager.Instance.AdminPasswordMinLength)
            {
                errorMessage = $"密码长度必须大于等于{ConfigManager.Instance.AdminPasswordMinLength}";
                return false;
            }
            if (
                !EUserPasswordRestrictionUtils.IsValid(password,
                    ConfigManager.Instance.AdminPasswordRestriction))
            {
                errorMessage =
                    $"密码不符合规则，请包含{EUserPasswordRestrictionUtils.GetText(EUserPasswordRestrictionUtils.GetEnumType(ConfigManager.Instance.AdminPasswordRestriction))}";
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

        public bool ChangePassword(AdministratorInfo adminInfo, string password, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrEmpty(password))
            {
                errorMessage = "密码不能为空";
                return false;
            }
            if (password.Length < ConfigManager.Instance.AdminPasswordMinLength)
            {
                errorMessage = $"密码长度必须大于等于{ConfigManager.Instance.AdminPasswordMinLength}";
                return false;
            }
            if (
                !EUserPasswordRestrictionUtils.IsValid(password, ConfigManager.Instance.AdminPasswordRestriction))
            {
                errorMessage =
                    $"密码不符合规则，请包含{EUserPasswordRestrictionUtils.GetText(EUserPasswordRestrictionUtils.GetEnumType(ConfigManager.Instance.AdminPasswordRestriction))}";
                return false;
            }

            password = EncodePassword(password, EPasswordFormat.Encrypted, out var passwordSalt);
            ChangePassword(adminInfo, EPasswordFormat.Encrypted, passwordSalt, password);
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

            var adminInfo = GetByAccount(account);
            if (string.IsNullOrEmpty(adminInfo?.UserName))
            {
                errorMessage = "帐号或密码错误";
                return false;
            }

            userName = adminInfo.UserName;

            if (adminInfo.Locked)
            {
                errorMessage = "此账号被锁定，无法登录";
                return false;
            }

            if (ConfigManager.Instance.IsAdminLockLogin)
            {
                if (adminInfo.CountOfFailedLogin > 0 &&
                    adminInfo.CountOfFailedLogin >= ConfigManager.Instance.AdminLockLoginCount)
                {
                    var lockType = EUserLockTypeUtils.GetEnumType(ConfigManager.Instance.AdminLockLoginType);
                    if (lockType == EUserLockType.Forever)
                    {
                        errorMessage = "此账号错误登录次数过多，已被永久锁定";
                        return false;
                    }
                    if (lockType == EUserLockType.Hours)
                    {
                        if (adminInfo.LastActivityDate.HasValue)
                        {
                            var ts = new TimeSpan(DateTime.Now.Ticks - adminInfo.LastActivityDate.Value.Ticks);
                            var hours = Convert.ToInt32(ConfigManager.Instance.AdminLockLoginHours - ts.TotalHours);
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

            if (CheckPassword(password, isPasswordMd5, adminInfo.Password, EPasswordFormatUtils.GetEnumType(adminInfo.PasswordFormat), adminInfo.PasswordSalt))
                return true;

            errorMessage = "账号或密码错误";
            return false;
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

        private static string EncodePassword(string password, EPasswordFormat passwordFormat, out string passwordSalt)
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
