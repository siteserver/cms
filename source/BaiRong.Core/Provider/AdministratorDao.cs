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
using BaiRong.Core.Permissions;

namespace BaiRong.Core.Provider
{
    public class AdministratorDao : DataProviderBase
    {
        public const string TableName = "bairong_Administrator";

        private const string SqlSelectUser = "SELECT UserName, Password, PasswordFormat, PasswordSalt, CreationDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, CreatorUserName, IsLockedOut, PublishmentSystemIDCollection, PublishmentSystemID, DepartmentID, AreaID, DisplayName, Email, Mobile FROM bairong_Administrator WHERE UserName = @UserName";

        private const string SqlSelectUserByEmail = "SELECT UserName, Password, PasswordFormat, PasswordSalt, CreationDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, CreatorUserName, IsLockedOut, PublishmentSystemIDCollection, PublishmentSystemID, DepartmentID, AreaID, DisplayName, Email, Mobile FROM bairong_Administrator WHERE Email = @Email";

        private const string SqlSelectUserByMobile = "SELECT UserName, Password, PasswordFormat, PasswordSalt, CreationDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, CreatorUserName, IsLockedOut, PublishmentSystemIDCollection, PublishmentSystemID, DepartmentID, AreaID, DisplayName, Email, Mobile FROM bairong_Administrator WHERE Mobile = @Mobile";

        private const string SqlSelectUsername = "SELECT UserName FROM bairong_Administrator WHERE UserName = @UserName";

        private const string SqlSelectUsernameByEmail = "SELECT UserName FROM bairong_Administrator WHERE Email = @Email";

        private const string SqlSelectUsernameByMobile = "SELECT UserName FROM bairong_Administrator WHERE Mobile = @Mobile";

        private const string SqlSelectMobileByUsername = "SELECT Mobile FROM bairong_Administrator WHERE UserName = @UserName";

        private const string SqlSelectCreatorUserName = "SELECT CreatorUserName FROM bairong_Administrator WHERE UserName = @UserName";

        private const string SqlSelectDisplayName = "SELECT DisplayName FROM bairong_Administrator WHERE UserName = @UserName";

        private const string SqlSelectDepartmentId = "SELECT DepartmentID FROM bairong_Administrator WHERE UserName = @UserName";

        private const string SqlSelectAreaId = "SELECT AreaID FROM bairong_Administrator WHERE UserName = @UserName";

        private const string SqlSelectPublishmentsystemidCollection = "SELECT PublishmentSystemIDCollection FROM bairong_Administrator WHERE UserName = @UserName";

        private const string SqlSelectPublishmentsystemid = "SELECT PublishmentSystemID FROM bairong_Administrator WHERE UserName = @UserName";

        private const string SqlInsertUser = "INSERT INTO bairong_Administrator (UserName, Password, PasswordFormat, PasswordSalt, CreationDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, CreatorUserName, IsLockedOut, PublishmentSystemIDCollection, PublishmentSystemID, DepartmentID, AreaID, DisplayName, Email, Mobile) VALUES (@UserName, @Password, @PasswordFormat, @PasswordSalt, @CreationDate, @LastActivityDate, @CountOfLogin, @CountOfFailedLogin, @CreatorUserName, @IsLockedOut, @PublishmentSystemIDCollection, @PublishmentSystemID, @DepartmentID, @AreaID, @DisplayName, @Email, @Mobile)";

        private const string SqlUpdateUser = "UPDATE bairong_Administrator SET LastActivityDate = @LastActivityDate, CountOfLogin = @CountOfLogin, CountOfFailedLogin = @CountOfFailedLogin, IsLockedOut = @IsLockedOut, PublishmentSystemIDCollection = @PublishmentSystemIDCollection, PublishmentSystemID = @PublishmentSystemID, DepartmentID = @DepartmentID, AreaID = @AreaID, DisplayName = @DisplayName, Email = @Email, Mobile = @Mobile WHERE UserName = @UserName";

        private const string SqlUpdatePublishmentsystemid = "UPDATE bairong_Administrator SET PublishmentSystemID = @PublishmentSystemID WHERE UserName = @UserName";

        private const string SqlUpdatePublishmentsystemidCollection = "UPDATE bairong_Administrator SET PublishmentSystemIDCollection = @PublishmentSystemIDCollection WHERE UserName = @UserName";

        private const string SqlUpdatePassword = "UPDATE bairong_Administrator SET Password = @Password, PasswordFormat = @PasswordFormat, PasswordSalt = @PasswordSalt WHERE UserName = @UserName";

        private const string SqlDeleteUser = "DELETE FROM bairong_Administrator WHERE UserName = @UserName";

        private const string ParmUsername = "@UserName";
        private const string ParmPassword = "@Password";
        private const string ParmPasswordFormat = "@PasswordFormat";
        private const string ParmPasswordSalt = "@PasswordSalt";
        private const string ParmCreationDate = "@CreationDate";
        private const string ParmLastActivityDate = "@LastActivityDate";
        private const string ParmCountOfLogin = "@CountOfLogin";
        private const string ParmCountOfFailedLogin = "@CountOfFailedLogin";
        private const string ParmCreatorUsername = "@CreatorUserName";
        private const string ParmIsLockedOut = "@IsLockedOut";
        private const string ParmPublishmentsystemidCollection = "@PublishmentSystemIDCollection";
        private const string ParmPublishmentsystemid = "@PublishmentSystemID";
        private const string ParmDepartmentId = "@DepartmentID";
        private const string ParmAreaId = "@AreaID";
        private const string ParmDisplayname = "@DisplayName";
        private const string ParmEmail = "@Email";
        private const string ParmMobile = "@Mobile";

        private void Insert(AdministratorInfo info)
        {
            IDataParameter[] insertParms =
            {
                GetParameter(ParmUsername, EDataType.NVarChar, 255, info.UserName),
                GetParameter(ParmPassword, EDataType.NVarChar, 255, info.Password),
                GetParameter(ParmPasswordFormat, EDataType.VarChar, 50, EPasswordFormatUtils.GetValue(info.PasswordFormat)),
                GetParameter(ParmPasswordSalt, EDataType.NVarChar, 128, info.PasswordSalt),
                GetParameter(ParmCreationDate, EDataType.DateTime, info.CreationDate),
                GetParameter(ParmLastActivityDate, EDataType.DateTime, info.LastActivityDate),
                GetParameter(ParmCountOfLogin, EDataType.Integer, info.CountOfLogin),
                GetParameter(ParmCountOfFailedLogin, EDataType.Integer, info.CountOfFailedLogin),
                GetParameter(ParmCreatorUsername, EDataType.NVarChar, 255, info.CreatorUserName),
                GetParameter(ParmIsLockedOut, EDataType.VarChar, 18, info.IsLockedOut.ToString()),
                GetParameter(ParmPublishmentsystemidCollection, EDataType.VarChar, 50, info.PublishmentSystemIdCollection),
                GetParameter(ParmPublishmentsystemid, EDataType.Integer, info.PublishmentSystemId),
                GetParameter(ParmDepartmentId, EDataType.Integer, info.DepartmentId),
                GetParameter(ParmAreaId, EDataType.Integer, info.AreaId),
                GetParameter(ParmDisplayname, EDataType.NVarChar, 255, info.DisplayName),
                GetParameter(ParmEmail, EDataType.NVarChar, 255, info.Email),
                GetParameter(ParmMobile, EDataType.VarChar, 20, info.Mobile)
            };

            ExecuteNonQuery(SqlInsertUser, insertParms);

            BaiRongDataProvider.DepartmentDao.UpdateCountOfAdmin();
            BaiRongDataProvider.AreaDao.UpdateCountOfAdmin();
        }

        public void Update(AdministratorInfo info)
        {
            IDataParameter[] parms =
            {
                GetParameter(ParmLastActivityDate, EDataType.DateTime, info.LastActivityDate),
                GetParameter(ParmCountOfLogin, EDataType.Integer, info.CountOfLogin),
                GetParameter(ParmCountOfFailedLogin, EDataType.Integer, info.CountOfFailedLogin),
                GetParameter(ParmIsLockedOut, EDataType.VarChar, 18, info.IsLockedOut.ToString()),
                GetParameter(ParmPublishmentsystemidCollection, EDataType.VarChar, 50, info.PublishmentSystemIdCollection),
                GetParameter(ParmPublishmentsystemid, EDataType.Integer, info.PublishmentSystemId),
                GetParameter(ParmDepartmentId, EDataType.Integer, info.DepartmentId),
                GetParameter(ParmAreaId, EDataType.Integer, info.AreaId),
                GetParameter(ParmDisplayname, EDataType.NVarChar, 255, info.DisplayName),
                GetParameter(ParmEmail, EDataType.NVarChar, 255, info.Email),
                GetParameter(ParmMobile, EDataType.VarChar, 20, info.Mobile),
                GetParameter(ParmUsername, EDataType.NVarChar, 255, info.UserName)
            };

            ExecuteNonQuery(SqlUpdateUser, parms);

            BaiRongDataProvider.DepartmentDao.UpdateCountOfAdmin();
            BaiRongDataProvider.AreaDao.UpdateCountOfAdmin();

            AdminManager.RemoveCache(info.UserName);
        }

        public void UpdateLastActivityDateAndCountOfFailedLogin(string userName)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                var sqlString = $"UPDATE bairong_Administrator SET LastActivityDate = @LastActivityDate, {SqlUtils.GetAddOne("CountOfFailedLogin")} WHERE UserName = @UserName";

                IDataParameter[] updateParms = {
                    GetParameter(ParmLastActivityDate, EDataType.DateTime, DateTime.Now),
                    GetParameter(ParmUsername, EDataType.NVarChar, 255, userName)
                };

                ExecuteNonQuery(sqlString, updateParms);

                AdminManager.RemoveCache(userName);
            }
        }

        public void UpdateLastActivityDateAndCountOfLogin(string userName)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                var sqlString = $"UPDATE bairong_Administrator SET LastActivityDate = @LastActivityDate, {SqlUtils.GetAddOne("CountOfLogin")}, CountOfFailedLogin = 0 WHERE UserName = @UserName";

                IDataParameter[] updateParms = {
                    GetParameter(ParmLastActivityDate, EDataType.DateTime, DateTime.Now),
                    GetParameter(ParmUsername, EDataType.NVarChar, 255, userName)
                };

                ExecuteNonQuery(sqlString, updateParms);

                AdminManager.RemoveCache(userName);
            }
        }

        public void UpdatePublishmentSystemIdCollection(string userName, string publishmentSystemIdCollection)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                IDataParameter[] updateParms = {
                    GetParameter(ParmPublishmentsystemidCollection, EDataType.VarChar, 50, publishmentSystemIdCollection),
                    GetParameter(ParmUsername, EDataType.NVarChar, 255, userName)
                };

                ExecuteNonQuery(SqlUpdatePublishmentsystemidCollection, updateParms);

                AdminManager.RemoveCache(userName);
            }
        }

        public void UpdatePublishmentSystemId(string userName, int publishmentSystemId)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                IDataParameter[] updateParms = {
                    GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId),
                    GetParameter(ParmUsername, EDataType.NVarChar, 255, userName)
                };

                ExecuteNonQuery(SqlUpdatePublishmentsystemid, updateParms);

                AdminManager.RemoveCache(userName);
            }
        }

        private bool ChangePassword(string userName, EPasswordFormat passwordFormat, string passwordSalt, string password)
        {
            var isSuccess = false;
            IDataParameter[] updateParms = {
                GetParameter(ParmPassword, EDataType.NVarChar, 255, password),
                GetParameter(ParmPasswordFormat, EDataType.VarChar, 50, EPasswordFormatUtils.GetValue(passwordFormat)),
                GetParameter(ParmPasswordSalt, EDataType.NVarChar, 128, passwordSalt),
                GetParameter(ParmUsername, EDataType.NVarChar, 255, userName)
            };

            try
            {
                ExecuteNonQuery(SqlUpdatePassword, updateParms);

                AdminManager.RemoveCache(userName);
                isSuccess = true;
            }
            catch
            {
                // ignored
            }
            return isSuccess;
        }

        public void Delete(string userName)
        {
            IDataParameter[] deleteParms = {
                GetParameter(ParmUsername, EDataType.NVarChar, 255, userName)
            };

            ExecuteNonQuery(SqlDeleteUser, deleteParms);

            AdminManager.RemoveCache(userName);

            BaiRongDataProvider.DepartmentDao.UpdateCountOfAdmin();
            BaiRongDataProvider.AreaDao.UpdateCountOfAdmin();
        }

        public void Lock(List<string> userNameList)
        {
            string sqlString =
                $"UPDATE bairong_Administrator SET IsLockedOut = '{true}' WHERE UserName IN ({TranslateUtils.ToSqlInStringWithQuote(userNameList)})";

            ExecuteNonQuery(sqlString);

            AdminManager.Clear();
        }

        public void UnLock(List<string> userNameList)
        {
            string sqlString =
                $"UPDATE bairong_Administrator SET IsLockedOut = '{false}', CountOfFailedLogin = 0 WHERE UserName IN ({TranslateUtils.ToSqlInStringWithQuote(userNameList)})";

            ExecuteNonQuery(sqlString);

            AdminManager.Clear();
        }

        public AdministratorInfo GetByAccount(string account)
        {
            AdministratorInfo info = null;

            string sqlString;
            IDataParameter[] parms;
            if (StringUtils.IsMobile(account))
            {
                sqlString = SqlSelectUserByMobile;
                parms = new IDataParameter[]
                {
                    GetParameter(ParmMobile, EDataType.VarChar, 50, account)
                };
            }
            else if (StringUtils.IsEmail(account))
            {
                sqlString = SqlSelectUserByEmail;
                parms = new IDataParameter[]
                {
                    GetParameter(ParmEmail, EDataType.VarChar, 50, account)
                };
            }
            else
            {
                sqlString = SqlSelectUser;
                parms = new IDataParameter[]
                {
                    GetParameter(ParmUsername, EDataType.NVarChar, 255, account)
                };
            }

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    info = new AdministratorInfo(GetString(rdr, i++), GetString(rdr, i++), EPasswordFormatUtils.GetEnumType(GetString(rdr, i++)), GetString(rdr, i++), GetDateTime(rdr, i++), GetDateTime(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), TranslateUtils.ToBool(GetString(rdr, i++)), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
                }
                rdr.Close();
            }

            return info;
        }

        public AdministratorInfo GetByUserName(string userName)
        {
            AdministratorInfo info = null;

            IDataParameter[] parms =
            {
                GetParameter(ParmUsername, EDataType.NVarChar, 255, userName)
            };

            using (var rdr = ExecuteReader(SqlSelectUser, parms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    info = new AdministratorInfo(GetString(rdr, i++), GetString(rdr, i++), EPasswordFormatUtils.GetEnumType(GetString(rdr, i++)), GetString(rdr, i++), GetDateTime(rdr, i++), GetDateTime(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), TranslateUtils.ToBool(GetString(rdr, i++)), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
                }
                rdr.Close();
            }

            return info;
        }

        public int GetDepartmentId(string userName)
        {
            var departmentId = 0;

            IDataParameter[] parms = {
                GetParameter(ParmUsername, EDataType.NVarChar, 255, userName)
            };

            using (var rdr = ExecuteReader(SqlSelectDepartmentId, parms))
            {
                if (rdr.Read())
                {
                    departmentId = GetInt(rdr, 0);
                }
                rdr.Close();
            }

            return departmentId;
        }

        public int GetAreaId(string userName)
        {
            var areaId = 0;

            IDataParameter[] parms = {
                GetParameter(ParmUsername, EDataType.NVarChar, 255, userName)
            };

            using (var rdr = ExecuteReader(SqlSelectAreaId, parms))
            {
                if (rdr.Read())
                {
                    areaId = GetInt(rdr, 0);
                }
                rdr.Close();
            }

            return areaId;
        }

        public string GetSelectCommand(bool isConsoleAdministrator, string creatorUserName, int departmentId)
        {
            string sqlString;
            if (departmentId == 0)
            {
                sqlString = "SELECT UserName, Password, PasswordFormat, PasswordSalt, CreationDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, CreatorUserName, IsLockedOut, PublishmentSystemIDCollection, PublishmentSystemID, DepartmentID, AreaID, DisplayName, Email, Mobile FROM bairong_Administrator";
                if (!isConsoleAdministrator)
                {
                    sqlString =
                        $"SELECT UserName, Password, PasswordFormat, PasswordSalt, CreationDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, CreatorUserName, IsLockedOut, PublishmentSystemIDCollection, PublishmentSystemID, DepartmentID, AreaID, DisplayName, Email, Mobile FROM bairong_Administrator WHERE CreatorUserName = '{PageUtils.FilterSql(creatorUserName)}'";
                }
            }
            else
            {
                sqlString =
                    $"SELECT UserName, Password, PasswordFormat, PasswordSalt, CreationDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, CreatorUserName, IsLockedOut, PublishmentSystemIDCollection, PublishmentSystemID, DepartmentID, AreaID, DisplayName, Email, Mobile FROM bairong_Administrator WHERE DepartmentID = {departmentId}";
                if (!isConsoleAdministrator)
                {
                    sqlString =
                        $"SELECT UserName, Password, PasswordFormat, PasswordSalt, CreationDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, CreatorUserName, IsLockedOut, PublishmentSystemIDCollection, PublishmentSystemID, DepartmentID, AreaID, DisplayName, Email, Mobile FROM bairong_Administrator WHERE CreatorUserName = '{PageUtils.FilterSql(creatorUserName)}' AND DepartmentID = {departmentId}";
                }
            }
            return sqlString;
        }

        public string GetSelectCommand(string searchWord, string roleName, int dayOfLastActivity, bool isConsoleAdministrator, string creatorUserName, int departmentId, int areaId)
        {
            var whereBuilder = new StringBuilder();

            if (dayOfLastActivity > 0)
            {
                var dateTime = DateTime.Now.AddDays(-dayOfLastActivity);
                whereBuilder.Append($"(LastActivityDate >= '{dateTime:yyyy-MM-dd}') ");
            }
            if (!string.IsNullOrEmpty(searchWord))
            {
                if (whereBuilder.Length > 0)
                {
                    whereBuilder.Append(" AND ");
                }
                whereBuilder.Append(
                    $"(UserName LIKE '%{PageUtils.FilterSql(searchWord)}%' OR EMAIL LIKE '%{PageUtils.FilterSql(searchWord)}%' OR DisplayName LIKE '%{PageUtils.FilterSql(searchWord)}%')");
            }

            if (!isConsoleAdministrator)
            {
                if (whereBuilder.Length > 0)
                {
                    whereBuilder.Append(" AND ");
                }
                whereBuilder.Append($"CreatorUserName = '{PageUtils.FilterSql(creatorUserName)}'");
            }

            if (departmentId != 0)
            {
                if (whereBuilder.Length > 0)
                {
                    whereBuilder.Append(" AND ");
                }
                whereBuilder.Append($"DepartmentID = {departmentId}");
            }

            if (areaId != 0)
            {
                if (whereBuilder.Length > 0)
                {
                    whereBuilder.Append(" AND ");
                }
                whereBuilder.Append($"AreaID = {areaId}");
            }

            var whereString = string.Empty;
            if (!string.IsNullOrEmpty(roleName))
            {
                if (whereBuilder.Length > 0)
                {
                    whereString = $"AND {whereBuilder}";
                }
                whereString =
                    $"WHERE (UserName IN (SELECT UserName FROM bairong_AdministratorsInRoles WHERE RoleName = '{PageUtils.FilterSql(roleName)}')) {whereString}";
            }
            else
            {
                if (whereBuilder.Length > 0)
                {
                    whereString = $"WHERE {whereBuilder}";
                }
            }

            var sqlString = "SELECT UserName, Password, PasswordFormat, PasswordSalt, CreationDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, CreatorUserName, IsLockedOut, PublishmentSystemIDCollection, PublishmentSystemID, DepartmentID, AreaID, DisplayName, Email, Mobile FROM bairong_Administrator " + whereString;

            return sqlString;
        }

        public string GetSortFieldName()
        {
            return "UserName";
        }

        public bool IsUserNameExists(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return false;
            }

            var exists = false;

            IDataParameter[] parms = {
                GetParameter(ParmUsername, EDataType.NVarChar, 255, userName)
            };

            using (var rdr = ExecuteReader(SqlSelectUsername, parms))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    exists = true;
                }
                rdr.Close();
            }
            return exists;
        }

        public string GetUserNameByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return string.Empty;
            }

            var userName = string.Empty;

            IDataParameter[] parms = {
                GetParameter(ParmEmail, EDataType.NVarChar, 50, email)
            };

            using (var rdr = ExecuteReader(SqlSelectUsernameByEmail, parms))
            {
                if (rdr.Read())
                {
                    userName = GetString(rdr, 0);
                }
                rdr.Close();
            }
            return userName;
        }

        public string GetUserNameByMobile(string mobile)
        {
            if (string.IsNullOrEmpty(mobile))
            {
                return string.Empty;
            }

            var userName = string.Empty;

            IDataParameter[] parms = {
                GetParameter(ParmMobile, EDataType.VarChar, 50, mobile)
            };

            using (var rdr = ExecuteReader(SqlSelectUsernameByMobile, parms))
            {
                if (rdr.Read())
                {
                    userName = GetString(rdr, 0);
                }
                rdr.Close();
            }
            return userName;
        }

        public string GetMobileByUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return string.Empty;
            }

            var mobile = string.Empty;

            IDataParameter[] parms = {
                GetParameter(ParmUsername, EDataType.NVarChar, 50, userName)
            };

            using (var rdr = ExecuteReader(SqlSelectMobileByUsername, parms))
            {
                if (rdr.Read())
                {
                    mobile = GetString(rdr, 0);
                }
                rdr.Close();
            }
            return mobile;
        }

        public string GetCreatorUserName(string userName)
        {
            var creatorUserName = string.Empty;

            IDataParameter[] parms = {
                GetParameter(ParmUsername, EDataType.NVarChar, 255, userName)
            };

            using (var rdr = ExecuteReader(SqlSelectCreatorUserName, parms))
            {
                if (rdr.Read())
                {
                    creatorUserName = GetString(rdr, 0);
                }
                rdr.Close();
            }
            return creatorUserName;
        }

        public string GetDisplayName(string userName)
        {
            var displayName = string.Empty;

            IDataParameter[] parms = {
                GetParameter(ParmUsername, EDataType.NVarChar, 255, userName)
            };

            using (var rdr = ExecuteReader(SqlSelectDisplayName, parms))
            {
                if (rdr.Read())
                {
                    displayName = GetString(rdr, 0);
                }
                rdr.Close();
            }

            return (!string.IsNullOrEmpty(displayName)) ? displayName : userName;
        }

        public List<int> GetPublishmentSystemIdList(string userName)
        {
            var publishmentSystemIdList = new List<int>();

            IDataParameter[] parms = {
                GetParameter(ParmUsername, EDataType.NVarChar, 255, userName)
            };

            using (var rdr = ExecuteReader(SqlSelectPublishmentsystemidCollection, parms))
            {
                if (rdr.Read())
                {
                    var collection = GetString(rdr, 0);
                    if (!string.IsNullOrEmpty(collection))
                    {
                        publishmentSystemIdList = TranslateUtils.StringCollectionToIntList(collection);
                    }
                }
                rdr.Close();
            }
            return publishmentSystemIdList;
        }

        public int GetPublishmentSystemId(string userName)
        {
            var publishmentSystemId = 0;

            IDataParameter[] parms = {
                GetParameter(ParmUsername, EDataType.NVarChar, 255, userName)
            };

            using (var rdr = ExecuteReader(SqlSelectPublishmentsystemid, parms))
            {
                if (rdr.Read())
                {
                    publishmentSystemId = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return publishmentSystemId;
        }

        public ArrayList GetUserNameArrayListByCreatorUserName(string creatorUserName)
        {
            var arraylist = new ArrayList();
            if (creatorUserName != null)
            {
                const string sqlString = "SELECT UserName FROM bairong_Administrator WHERE CreatorUserName = @CreatorUserName";

                IDataParameter[] parms = {
                    GetParameter(ParmCreatorUsername, EDataType.NVarChar, 255, creatorUserName)
                };

                using (var rdr = ExecuteReader(sqlString, parms))
                {
                    while (rdr.Read())
                    {
                        arraylist.Add(GetString(rdr, 0));
                    }
                    rdr.Close();
                }
            }
            return arraylist;
        }

        public ArrayList GetUserNameArrayList()
        {
            var arraylist = new ArrayList();
            const string sqlSelect = "SELECT UserName FROM bairong_Administrator";

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    arraylist.Add(GetString(rdr, 0));
                }
                rdr.Close();
            }
            return arraylist;
        }

        public ArrayList GetUserNameArrayList(List<int> departmentIdList)
        {
            var arraylist = new ArrayList();
            string sqlSelect =
                $"SELECT UserName FROM bairong_Administrator WHERE DepartmentID IN ({TranslateUtils.ToSqlInStringWithoutQuote(departmentIdList)}) ORDER BY DepartmentID";

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    arraylist.Add(GetString(rdr, 0));
                }
                rdr.Close();
            }
            return arraylist;
        }

        public ArrayList GetUserNameArrayList(int departmentId, bool isAll)
        {
            var arraylist = new ArrayList();
            string sqlSelect = $"SELECT UserName FROM bairong_Administrator WHERE DepartmentID = {departmentId}";
            if (isAll)
            {
                var departmentIdList = BaiRongDataProvider.DepartmentDao.GetDepartmentIdListForDescendant(departmentId);
                departmentIdList.Add(departmentId);
                sqlSelect =
                    $"SELECT UserName FROM bairong_Administrator WHERE DepartmentID IN ({TranslateUtils.ObjectCollectionToString(departmentIdList)})";
            }

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    arraylist.Add(GetString(rdr, 0));
                }
                rdr.Close();
            }
            return arraylist;
        }

        public List<string> GetUserNameList(string searchWord, int dayOfCreation, int dayOfLastActivity, bool isChecked)
        {
            var arraylist = new List<string>();

            var whereString = string.Empty;
            if (dayOfCreation > 0)
            {
                var dateTime = DateTime.Now.AddDays(-dayOfCreation);
                whereString += $" AND (CreationDate >= '{dateTime:yyyy-MM-dd}') ";
            }
            if (dayOfLastActivity > 0)
            {
                var dateTime = DateTime.Now.AddDays(-dayOfLastActivity);
                whereString += $" AND (LastActivityDate >= '{dateTime:yyyy-MM-dd}') ";
            }
            if (!string.IsNullOrEmpty(searchWord))
            {
                var word = PageUtils.FilterSql(searchWord);
                whereString += $" AND (UserName LIKE '%{word}%' OR EMAIL LIKE '%{word}%') ";
            }

            string sqlString =
                $"SELECT * FROM bairong_Administrator WHERE IsChecked = '{isChecked}' {whereString}";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    arraylist.Add(GetString(rdr, 0));
                }
                rdr.Close();
            }
            return arraylist;
        }

        public static string EncodePassword(string password, EPasswordFormat passwordFormat, out string passwordSalt)
        {
            var retval = string.Empty;
            passwordSalt = string.Empty;

            if (passwordFormat == EPasswordFormat.Clear)
            {
                retval = password;
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
                if (algorithm == null) return retval;
                var inArray = algorithm.ComputeHash(dst);

                retval = Convert.ToBase64String(inArray);
            }
            else if (passwordFormat == EPasswordFormat.Encrypted)
            {
                passwordSalt = GenerateSalt();

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

        public static string GenerateSalt()
        {
            var data = new byte[0x10];
            new RNGCryptoServiceProvider().GetBytes(data);
            return Convert.ToBase64String(data);
        }

        public bool Insert(AdministratorInfo userInfo, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrEmpty(userInfo.UserName))
            {
                errorMessage = "用户名不能为空";
                return false;
            }
            if (userInfo.UserName.Length < ConfigManager.SystemConfigInfo.LoginUserNameMinLength)
            {
                errorMessage = $"用户名长度必须大于等于{ConfigManager.SystemConfigInfo.LoginUserNameMinLength}";
                return false;
            }
            if (IsUserNameExists(userInfo.UserName))
            {
                errorMessage = "用户名已存在，请更换用户名";
                return false;
            }

            if (string.IsNullOrEmpty(userInfo.Password))
            {
                errorMessage = "密码不能为空";
                return false;
            }
            if (userInfo.Password.Length < ConfigManager.SystemConfigInfo.LoginPasswordMinLength)
            {
                errorMessage = $"密码长度必须大于等于{ConfigManager.SystemConfigInfo.LoginPasswordMinLength}";
                return false;
            }
            if (!EUserPasswordRestrictionUtils.IsValid(userInfo.Password, ConfigManager.SystemConfigInfo.LoginPasswordRestriction))
            {
                errorMessage =
                    $"密码不符合规则，请包含{EUserPasswordRestrictionUtils.GetText(ConfigManager.SystemConfigInfo.LoginPasswordRestriction)}";
                return false;
            }

            try
            {
                string passwordSalt;
                userInfo.Password = EncodePassword(userInfo.Password, userInfo.PasswordFormat, out passwordSalt);
                userInfo.PasswordSalt = passwordSalt;
                Insert(userInfo);
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public bool ChangePassword(string userName, EPasswordFormat passwordFormat, string password, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrEmpty(password))
            {
                errorMessage = "密码不能为空";
                return false;
            }
            if (password.Length < ConfigManager.SystemConfigInfo.LoginPasswordMinLength)
            {
                errorMessage = $"密码长度必须大于等于{ConfigManager.SystemConfigInfo.LoginPasswordMinLength}";
                return false;
            }
            if (!EUserPasswordRestrictionUtils.IsValid(password, ConfigManager.SystemConfigInfo.LoginPasswordRestriction))
            {
                errorMessage =
                    $"密码不符合规则，请包含{EUserPasswordRestrictionUtils.GetText(ConfigManager.SystemConfigInfo.LoginPasswordRestriction)}";
                return false;
            }

            string passwordSalt;
            password = EncodePassword(password, passwordFormat, out passwordSalt);
            return ChangePassword(userName, passwordFormat, passwordSalt, password);
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

            var adminInfo = GetByAccount(account);
            if (string.IsNullOrEmpty(adminInfo?.UserName))
            {
                errorMessage = "帐号或密码错误";
                return false;
            }

            userName = adminInfo.UserName;

            if (adminInfo.IsLockedOut)
            {
                errorMessage = "此账号被锁定，无法登录";
                return false;
            }

            if (ConfigManager.SystemConfigInfo.IsLoginFailToLock)
            {
                if (adminInfo.CountOfFailedLogin > 0 && adminInfo.CountOfFailedLogin >= ConfigManager.SystemConfigInfo.LoginFailToLockCount)
                {
                    var lockType = EUserLockTypeUtils.GetEnumType(ConfigManager.SystemConfigInfo.LoginLockingType);
                    if (lockType == EUserLockType.Forever)
                    {
                        errorMessage = "此账号错误登录次数过多，已被永久锁定";
                        return false;
                    }
                    if (lockType == EUserLockType.Hours)
                    {
                        var ts = new TimeSpan(DateTime.Now.Ticks - adminInfo.LastActivityDate.Ticks);
                        var hours = Convert.ToInt32(ConfigManager.SystemConfigInfo.LoginLockingHours - ts.TotalHours);
                        if (hours > 0)
                        {
                            errorMessage =
                                $"此账号错误登录次数过多，已被锁定，请等待{hours}小时后重试";
                            return false;
                        }
                    }
                }
            }

            if (CheckPassword(password, adminInfo.Password, adminInfo.PasswordFormat, adminInfo.PasswordSalt)) return true;

            errorMessage = "账号或密码不正确";
            return false;
        }

        public static string DecodePassword(string password, EPasswordFormat passwordFormat, string passwordSalt)
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

        public bool CheckPassword(string password, string dbpassword, EPasswordFormat passwordFormat, string passwordSalt)
        {
            var pass1 = password;
            var pass2 = DecodePassword(dbpassword, passwordFormat, passwordSalt);

            return pass1 == pass2;
        }

        public string GetPassword(string password, EPasswordFormat passwordFormat, string passwordSalt)
        {
            return DecodePassword(password, passwordFormat, passwordSalt);
        }
    }
}
