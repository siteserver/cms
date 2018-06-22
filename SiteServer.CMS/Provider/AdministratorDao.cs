using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using Dapper.Contrib.Extensions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Data;
using SiteServer.CMS.Model;
using SiteServer.Utils.Auth;
using SiteServer.Plugin;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Provider
{
    public class AdministratorDao : DataProviderBase
    {
        public const string DatabaseTableName = "siteserver_Administrator";

        public override string TableName => DatabaseTableName;

        public override List<TableColumn> TableColumns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(AdministratorInfoDatabase.Id),
                DataType = DataType.Integer,
                IsPrimaryKey = true,
                IsIdentity = true
            },
            new TableColumn
            {
                AttributeName = nameof(AdministratorInfoDatabase.UserName),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(AdministratorInfoDatabase.Password),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(AdministratorInfoDatabase.PasswordFormat),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(AdministratorInfoDatabase.PasswordSalt),
                DataType = DataType.VarChar,
                DataLength = 128
            },
            new TableColumn
            {
                AttributeName = nameof(AdministratorInfoDatabase.CreationDate),
                DataType = DataType.DateTime
            },
            new TableColumn
            {
                AttributeName = nameof(AdministratorInfoDatabase.LastActivityDate),
                DataType = DataType.DateTime
            },
            new TableColumn
            {
                AttributeName = nameof(AdministratorInfoDatabase.CountOfLogin),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(AdministratorInfoDatabase.CountOfFailedLogin),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(AdministratorInfoDatabase.CreatorUserName),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(AdministratorInfoDatabase.IsLockedOut),
                DataType = DataType.VarChar,
                DataLength = 18
            },
            new TableColumn
            {
                AttributeName = nameof(AdministratorInfoDatabase.SiteIdCollection),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(AdministratorInfoDatabase.SiteId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(AdministratorInfoDatabase.DepartmentId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(AdministratorInfoDatabase.AreaId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(AdministratorInfoDatabase.DisplayName),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(AdministratorInfoDatabase.Email),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(AdministratorInfoDatabase.Mobile),
                DataType = DataType.VarChar,
                DataLength = 20
            }
        };

        private const string SqlSelectUser =
            "SELECT Id, UserName, Password, PasswordFormat, PasswordSalt, CreationDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, CreatorUserName, IsLockedOut, SiteIdCollection, SiteId, DepartmentId, AreaId, DisplayName, Email, Mobile FROM siteserver_Administrator WHERE UserName = @UserName";

        private const string SqlSelectUserByEmail =
            "SELECT Id, UserName, Password, PasswordFormat, PasswordSalt, CreationDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, CreatorUserName, IsLockedOut, SiteIdCollection, SiteId, DepartmentId, AreaId, DisplayName, Email, Mobile FROM siteserver_Administrator WHERE Email = @Email";

        private const string SqlSelectUserByMobile =
            "SELECT Id, UserName, Password, PasswordFormat, PasswordSalt, CreationDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, CreatorUserName, IsLockedOut, SiteIdCollection, SiteId, DepartmentId, AreaId, DisplayName, Email, Mobile FROM siteserver_Administrator WHERE Mobile = @Mobile";

        private const string SqlSelectUsername = "SELECT UserName FROM siteserver_Administrator WHERE UserName = @UserName";

        private const string SqlSelectUsernameByEmail =
            "SELECT UserName FROM siteserver_Administrator WHERE Email = @Email";

        private const string SqlSelectUsernameByMobile =
            "SELECT UserName FROM siteserver_Administrator WHERE Mobile = @Mobile";

        private const string SqlSelectMobileByUsername =
            "SELECT Mobile FROM siteserver_Administrator WHERE UserName = @UserName";

        private const string SqlSelectCreatorUserName =
            "SELECT CreatorUserName FROM siteserver_Administrator WHERE UserName = @UserName";

        private const string SqlSelectDisplayName =
            "SELECT DisplayName FROM siteserver_Administrator WHERE UserName = @UserName";

        private const string SqlSelectDepartmentId =
            "SELECT DepartmentId FROM siteserver_Administrator WHERE UserName = @UserName";

        private const string SqlSelectAreaId = "SELECT AreaId FROM siteserver_Administrator WHERE UserName = @UserName";

        private const string SqlSelectSiteIdCollection =
            "SELECT SiteIdCollection FROM siteserver_Administrator WHERE UserName = @UserName";

        private const string SqlSelectSiteId =
            "SELECT SiteId FROM siteserver_Administrator WHERE UserName = @UserName";

        private const string SqlInsertUser =
            "INSERT INTO siteserver_Administrator (UserName, Password, PasswordFormat, PasswordSalt, CreationDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, CreatorUserName, IsLockedOut, SiteIdCollection, SiteId, DepartmentId, AreaId, DisplayName, Email, Mobile) VALUES (@UserName, @Password, @PasswordFormat, @PasswordSalt, @CreationDate, @LastActivityDate, @CountOfLogin, @CountOfFailedLogin, @CreatorUserName, @IsLockedOut, @SiteIdCollection, @SiteId, @DepartmentId, @AreaId, @DisplayName, @Email, @Mobile)";

        private const string SqlUpdateUser =
            "UPDATE siteserver_Administrator SET LastActivityDate = @LastActivityDate, CountOfLogin = @CountOfLogin, CountOfFailedLogin = @CountOfFailedLogin, IsLockedOut = @IsLockedOut, SiteIdCollection = @SiteIdCollection, SiteId = @SiteId, DepartmentId = @DepartmentId, AreaId = @AreaId, DisplayName = @DisplayName, Email = @Email, Mobile = @Mobile WHERE UserName = @UserName";

        private const string SqlUpdateSiteId =
            "UPDATE siteserver_Administrator SET SiteId = @SiteId WHERE UserName = @UserName";

        private const string SqlUpdateSiteIdCollection =
            "UPDATE siteserver_Administrator SET SiteIdCollection = @SiteIdCollection WHERE UserName = @UserName";

        private const string SqlUpdatePassword =
            "UPDATE siteserver_Administrator SET Password = @Password, PasswordFormat = @PasswordFormat, PasswordSalt = @PasswordSalt WHERE UserName = @UserName";

        private const string SqlDeleteUser = "DELETE FROM siteserver_Administrator WHERE UserName = @UserName";

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
        private const string ParmSiteIdCollection = "@SiteIdCollection";
        private const string ParmSiteId = "@SiteId";
        private const string ParmDepartmentId = "@DepartmentId";
        private const string ParmAreaId = "@AreaId";
        private const string ParmDisplayname = "@DisplayName";
        private const string ParmEmail = "@Email";
        private const string ParmMobile = "@Mobile";

        public void Update(IAdministratorInfo info)
        {
            IDataParameter[] parms =
            {
                GetParameter(ParmLastActivityDate, DataType.DateTime, info.LastActivityDate),
                GetParameter(ParmCountOfLogin, DataType.Integer, info.CountOfLogin),
                GetParameter(ParmCountOfFailedLogin, DataType.Integer, info.CountOfFailedLogin),
                GetParameter(ParmIsLockedOut, DataType.VarChar, 18, info.IsLockedOut.ToString()),
                GetParameter(ParmSiteIdCollection, DataType.VarChar, 50, info.SiteIdCollection),
                GetParameter(ParmSiteId, DataType.Integer, info.SiteId),
                GetParameter(ParmDepartmentId, DataType.Integer, info.DepartmentId),
                GetParameter(ParmAreaId, DataType.Integer, info.AreaId),
                GetParameter(ParmDisplayname, DataType.VarChar, 255, info.DisplayName),
                GetParameter(ParmEmail, DataType.VarChar, 255, info.Email),
                GetParameter(ParmMobile, DataType.VarChar, 20, info.Mobile),
                GetParameter(ParmUsername, DataType.VarChar, 255, info.UserName)
            };

            ExecuteNonQuery(SqlUpdateUser, parms);

            DataProvider.DepartmentDao.UpdateCountOfAdmin();
            DataProvider.AreaDao.UpdateCountOfAdmin();

            AdminManager.RemoveCache(info.UserName);
        }

        public void UpdateLastActivityDateAndCountOfFailedLogin(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return;

            var sqlString =
                $"UPDATE siteserver_Administrator SET LastActivityDate = @LastActivityDate, {SqlUtils.ToPlusSqlString("CountOfFailedLogin")} WHERE UserName = @UserName";

            IDataParameter[] updateParms =
            {
                GetParameter(ParmLastActivityDate, DataType.DateTime, DateTime.Now),
                GetParameter(ParmUsername, DataType.VarChar, 255, userName)
            };

            ExecuteNonQuery(sqlString, updateParms);

            AdminManager.RemoveCache(userName);
        }

        public void UpdateLastActivityDateAndCountOfLogin(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return;

            var sqlString =
                $"UPDATE siteserver_Administrator SET LastActivityDate = @LastActivityDate, {SqlUtils.ToPlusSqlString("CountOfLogin")}, CountOfFailedLogin = 0 WHERE UserName = @UserName";

            IDataParameter[] updateParms =
            {
                GetParameter(ParmLastActivityDate, DataType.DateTime, DateTime.Now),
                GetParameter(ParmUsername, DataType.VarChar, 255, userName)
            };

            ExecuteNonQuery(sqlString, updateParms);

            AdminManager.RemoveCache(userName);
        }

        public void UpdateSiteIdCollection(string userName, string siteIdCollection)
        {
            if (string.IsNullOrEmpty(userName)) return;

            IDataParameter[] updateParms =
            {
                GetParameter(ParmSiteIdCollection, DataType.VarChar, 50, siteIdCollection),
                GetParameter(ParmUsername, DataType.VarChar, 255, userName)
            };

            ExecuteNonQuery(SqlUpdateSiteIdCollection, updateParms);

            AdminManager.RemoveCache(userName);
        }

        public void UpdateSiteId(string userName, int siteId)
        {
            if (string.IsNullOrEmpty(userName)) return;

            IDataParameter[] updateParms =
            {
                GetParameter(ParmSiteId, DataType.Integer, siteId),
                GetParameter(ParmUsername, DataType.VarChar, 255, userName)
            };

            ExecuteNonQuery(SqlUpdateSiteId, updateParms);

            AdminManager.RemoveCache(userName);
        }

        private bool ChangePassword(string userName, EPasswordFormat passwordFormat, string passwordSalt,
            string password)
        {
            var isSuccess = false;
            IDataParameter[] updateParms =
            {
                GetParameter(ParmPassword, DataType.VarChar, 255, password),
                GetParameter(ParmPasswordFormat, DataType.VarChar, 50, EPasswordFormatUtils.GetValue(passwordFormat)),
                GetParameter(ParmPasswordSalt, DataType.VarChar, 128, passwordSalt),
                GetParameter(ParmUsername, DataType.VarChar, 255, userName)
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
            IDataParameter[] deleteParms =
            {
                GetParameter(ParmUsername, DataType.VarChar, 255, userName)
            };

            ExecuteNonQuery(SqlDeleteUser, deleteParms);

            AdminManager.RemoveCache(userName);

            DataProvider.DepartmentDao.UpdateCountOfAdmin();
            DataProvider.AreaDao.UpdateCountOfAdmin();
        }

        public void Lock(List<string> userNameList)
        {
            string sqlString =
                $"UPDATE siteserver_Administrator SET IsLockedOut = '{true}' WHERE UserName IN ({TranslateUtils.ToSqlInStringWithQuote(userNameList)})";

            ExecuteNonQuery(sqlString);

            AdminManager.Clear();
        }

        public void UnLock(List<string> userNameList)
        {
            string sqlString =
                $"UPDATE siteserver_Administrator SET IsLockedOut = '{false}', CountOfFailedLogin = 0 WHERE UserName IN ({TranslateUtils.ToSqlInStringWithQuote(userNameList)})";

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
                    GetParameter(ParmMobile, DataType.VarChar, 50, account)
                };
            }
            else if (StringUtils.IsEmail(account))
            {
                sqlString = SqlSelectUserByEmail;
                parms = new IDataParameter[]
                {
                    GetParameter(ParmEmail, DataType.VarChar, 50, account)
                };
            }
            else
            {
                sqlString = SqlSelectUser;
                parms = new IDataParameter[]
                {
                    GetParameter(ParmUsername, DataType.VarChar, 255, account)
                };
            }

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    info = new AdministratorInfo(GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++),
                        GetString(rdr, i++), GetString(rdr, i++), GetDateTime(rdr, i++), GetDateTime(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), TranslateUtils.ToBool(GetString(rdr, i++)), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
                }
                rdr.Close();
            }

            return info;
        }

        public AdministratorInfo GetByUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return null;

            AdministratorInfo info = null;

            IDataParameter[] parms =
            {
                GetParameter(ParmUsername, DataType.VarChar, 255, userName)
            };

            using (var rdr = ExecuteReader(SqlSelectUser, parms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    info = new AdministratorInfo(GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++),
                        GetString(rdr, i++), GetString(rdr, i++), GetDateTime(rdr, i++), GetDateTime(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++),
                        GetString(rdr, i++), TranslateUtils.ToBool(GetString(rdr, i++)), GetString(rdr, i++),
                        GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++),
                        GetString(rdr, i));
                }
                rdr.Close();
            }

            return info;
        }

        public int GetDepartmentId(string userName)
        {
            var departmentId = 0;

            IDataParameter[] parms =
            {
                GetParameter(ParmUsername, DataType.VarChar, 255, userName)
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

            IDataParameter[] parms =
            {
                GetParameter(ParmUsername, DataType.VarChar, 255, userName)
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

        public string GetWhereSqlString(bool isConsoleAdministrator, string creatorUserName, string searchWord, string roleName, int dayOfLastActivity, int departmentId, int areaId)
        {
            var whereBuilder = new StringBuilder();

            if (dayOfLastActivity > 0)
            {
                var dateTime = DateTime.Now.AddDays(-dayOfLastActivity);
                whereBuilder.Append($"(LastActivityDate >= {SqlUtils.GetComparableDate(dateTime)}) ");
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
                whereBuilder.Append($"DepartmentId = {departmentId}");
            }

            if (areaId != 0)
            {
                if (whereBuilder.Length > 0)
                {
                    whereBuilder.Append(" AND ");
                }
                whereBuilder.Append($"AreaId = {areaId}");
            }

            var whereString = string.Empty;
            if (!string.IsNullOrEmpty(roleName))
            {
                if (whereBuilder.Length > 0)
                {
                    whereString = $"AND {whereBuilder}";
                }
                whereString =
                    $"WHERE (UserName IN (SELECT UserName FROM {DataProvider.AdministratorsInRolesDao.TableName} WHERE RoleName = '{PageUtils.FilterSql(roleName)}')) {whereString}";
            }
            else
            {
                if (whereBuilder.Length > 0)
                {
                    whereString = $"WHERE {whereBuilder}";
                }
            }

            return whereString;
        }

        public string GetOrderSqlString(string order)
        {
            return $"ORDER BY {order} {(StringUtils.EqualsIgnoreCase(order, nameof(AdministratorInfo.UserName)) ? "ASC" : "DESC")}";
        }

        public bool IsAdminNameExists(string adminName)
        {
            if (string.IsNullOrEmpty(adminName))
            {
                return false;
            }

            var exists = false;

            IDataParameter[] parms =
            {
                GetParameter(ParmUsername, DataType.VarChar, 255, adminName)
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

            IDataParameter[] parms =
            {
                GetParameter(ParmEmail, DataType.VarChar, 50, email)
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
            if (string.IsNullOrEmpty(mobile)) return string.Empty;

            var userName = string.Empty;

            IDataParameter[] parms =
            {
                GetParameter(ParmMobile, DataType.VarChar, 50, mobile)
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

            IDataParameter[] parms =
            {
                GetParameter(ParmUsername, DataType.VarChar, 50, userName)
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

            IDataParameter[] parms =
            {
                GetParameter(ParmUsername, DataType.VarChar, 255, userName)
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

            IDataParameter[] parms =
            {
                GetParameter(ParmUsername, DataType.VarChar, 255, userName)
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

        public List<int> GetSiteIdList(string userName)
        {
            var siteIdList = new List<int>();

            IDataParameter[] parms =
            {
                GetParameter(ParmUsername, DataType.VarChar, 255, userName)
            };

            using (var rdr = ExecuteReader(SqlSelectSiteIdCollection, parms))
            {
                if (rdr.Read())
                {
                    var collection = GetString(rdr, 0);
                    if (!string.IsNullOrEmpty(collection))
                    {
                        siteIdList = TranslateUtils.StringCollectionToIntList(collection);
                    }
                }
                rdr.Close();
            }
            return siteIdList;
        }

        public int GetSiteId(string userName)
        {
            var siteId = 0;

            IDataParameter[] parms =
            {
                GetParameter(ParmUsername, DataType.VarChar, 255, userName)
            };

            using (var rdr = ExecuteReader(SqlSelectSiteId, parms))
            {
                if (rdr.Read())
                {
                    siteId = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return siteId;
        }

        public int GetCountByAreaId(int areaId)
        {
            var sqlString = $"SELECT COUNT(*) FROM {TableName} WHERE {nameof(AdministratorInfo.AreaId)} = {areaId}";
            
            return DataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public int GetCountByDepartmentId(int departmentId)
        {
            var sqlString = $"SELECT COUNT(*) FROM {TableName} WHERE {nameof(AdministratorInfo.DepartmentId)} = {departmentId}";

            return DataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public List<string> GetUserNameListByCreatorUserName(string creatorUserName)
        {
            var list = new List<string>();
            if (creatorUserName != null)
            {
                const string sqlString =
                    "SELECT UserName FROM siteserver_Administrator WHERE CreatorUserName = @CreatorUserName";

                IDataParameter[] parms =
                {
                    GetParameter(ParmCreatorUsername, DataType.VarChar, 255, creatorUserName)
                };

                using (var rdr = ExecuteReader(sqlString, parms))
                {
                    while (rdr.Read())
                    {
                        list.Add(GetString(rdr, 0));
                    }
                    rdr.Close();
                }
            }
            return list;
        }

        public List<string> GetUserNameList()
        {
            var list = new List<string>();
            const string sqlSelect = "SELECT UserName FROM siteserver_Administrator";

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

        public List<string> GetUserNameList(List<int> departmentIdList)
        {
            var list = new List<string>();
            string sqlSelect =
                $"SELECT UserName FROM siteserver_Administrator WHERE DepartmentId IN ({TranslateUtils.ToSqlInStringWithoutQuote(departmentIdList)}) ORDER BY DepartmentId";

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

        public List<string> GetUserNameList(int departmentId, bool isAll)
        {
            var list = new List<string>();
            string sqlSelect = $"SELECT UserName FROM siteserver_Administrator WHERE DepartmentId = {departmentId}";
            if (isAll)
            {
                var departmentIdList = DataProvider.DepartmentDao.GetIdListForDescendant(departmentId);
                departmentIdList.Add(departmentId);
                sqlSelect =
                    $"SELECT UserName FROM siteserver_Administrator WHERE DepartmentId IN ({TranslateUtils.ObjectCollectionToString(departmentIdList)})";
            }

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

        public List<string> GetUserNameList(string searchWord, int dayOfCreation, int dayOfLastActivity, bool isChecked)
        {
            var arraylist = new List<string>();

            var whereString = string.Empty;
            if (dayOfCreation > 0)
            {
                var dateTime = DateTime.Now.AddDays(-dayOfCreation);
                whereString += $" AND (CreationDate >= {SqlUtils.GetComparableDate(dateTime)}) ";
            }
            if (dayOfLastActivity > 0)
            {
                var dateTime = DateTime.Now.AddDays(-dayOfLastActivity);
                whereString += $" AND (LastActivityDate >= {SqlUtils.GetComparableDate(dateTime)}) ";
            }
            if (!string.IsNullOrEmpty(searchWord))
            {
                var word = PageUtils.FilterSql(searchWord);
                whereString += $" AND (UserName LIKE '%{word}%' OR EMAIL LIKE '%{word}%') ";
            }

            string sqlString =
                $"SELECT * FROM siteserver_Administrator WHERE IsChecked = '{isChecked}' {whereString}";

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

        public string EncodePassword(string password, EPasswordFormat passwordFormat, out string passwordSalt)
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

        public string GenerateSalt()
        {
            var data = new byte[0x10];
            new RNGCryptoServiceProvider().GetBytes(data);
            return Convert.ToBase64String(data);
        }

        public bool IsEmailExists(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;

            var exists = false;

            var sqlSelect = $"SELECT {nameof(AdministratorInfoDatabase.Email)} FROM {TableName} WHERE {nameof(AdministratorInfoDatabase.Email)} = @{nameof(AdministratorInfoDatabase.Email)}";

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
            var sqlString = $"SELECT {nameof(AdministratorInfoDatabase.Mobile)} FROM {TableName} WHERE {nameof(AdministratorInfoDatabase.Mobile)} = @{nameof(AdministratorInfoDatabase.Mobile)}";

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

        private bool UpdateValidate(AdministratorInfoCreateUpdate adminInfoToUpdate, string userName, string email, string mobile, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (adminInfoToUpdate.UserName != null && adminInfoToUpdate.UserName != userName)
            {
                if (string.IsNullOrEmpty(adminInfoToUpdate.UserName))
                {
                    errorMessage = "用户名不能为空";
                    return false;
                }
                if (adminInfoToUpdate.UserName.Length < ConfigManager.SystemConfigInfo.AdminUserNameMinLength)
                {
                    errorMessage = $"用户名长度必须大于等于{ConfigManager.SystemConfigInfo.AdminUserNameMinLength}";
                    return false;
                }
                if (IsAdminNameExists(adminInfoToUpdate.UserName))
                {
                    errorMessage = "用户名已存在，请更换用户名";
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

            if (adminInfoToUpdate.Mobile != null && adminInfoToUpdate.Mobile != mobile)
            {
                if (!string.IsNullOrEmpty(adminInfoToUpdate.Mobile) && IsMobileExists(adminInfoToUpdate.Mobile))
                {
                    errorMessage = "手机号码已被注册，请更换手机号码";
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
            if (userName.Length < ConfigManager.SystemConfigInfo.AdminUserNameMinLength)
            {
                errorMessage = $"用户名长度必须大于等于{ConfigManager.SystemConfigInfo.AdminUserNameMinLength}";
                return false;
            }
            if (IsAdminNameExists(userName))
            {
                errorMessage = "用户名已存在，请更换用户名";
                return false;
            }

            if (string.IsNullOrEmpty(password))
            {
                errorMessage = "密码不能为空";
                return false;
            }
            if (password.Length < ConfigManager.SystemConfigInfo.AdminPasswordMinLength)
            {
                errorMessage = $"密码长度必须大于等于{ConfigManager.SystemConfigInfo.AdminPasswordMinLength}";
                return false;
            }
            if (
                !EUserPasswordRestrictionUtils.IsValid(password,
                    ConfigManager.SystemConfigInfo.AdminPasswordRestriction))
            {
                errorMessage =
                    $"密码不符合规则，请包含{EUserPasswordRestrictionUtils.GetText(EUserPasswordRestrictionUtils.GetEnumType(ConfigManager.SystemConfigInfo.AdminPasswordRestriction))}";
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

        public bool Insert(AdministratorInfo adminInfo, out string errorMessage)
        {
            if (!InsertValidate(adminInfo.UserName, adminInfo.Password, adminInfo.Email, adminInfo.Mobile, out errorMessage)) return false;

            try
            {
                string passwordSalt;
                adminInfo.Password = EncodePassword(adminInfo.Password, EPasswordFormatUtils.GetEnumType(adminInfo.PasswordFormat), out passwordSalt);
                adminInfo.PasswordSalt = passwordSalt;

                IDataParameter[] insertParms =
                {
                    GetParameter(ParmUsername, DataType.VarChar, 255, adminInfo.UserName),
                    GetParameter(ParmPassword, DataType.VarChar, 255, adminInfo.Password),
                    GetParameter(ParmPasswordFormat, DataType.VarChar, 50, adminInfo.PasswordFormat),
                    GetParameter(ParmPasswordSalt, DataType.VarChar, 128, adminInfo.PasswordSalt),
                    GetParameter(ParmCreationDate, DataType.DateTime, adminInfo.CreationDate),
                    GetParameter(ParmLastActivityDate, DataType.DateTime, adminInfo.LastActivityDate),
                    GetParameter(ParmCountOfLogin, DataType.Integer, adminInfo.CountOfLogin),
                    GetParameter(ParmCountOfFailedLogin, DataType.Integer, adminInfo.CountOfFailedLogin),
                    GetParameter(ParmCreatorUsername, DataType.VarChar, 255, adminInfo.CreatorUserName),
                    GetParameter(ParmIsLockedOut, DataType.VarChar, 18, adminInfo.IsLockedOut.ToString()),
                    GetParameter(ParmSiteIdCollection, DataType.VarChar, 50, adminInfo.SiteIdCollection),
                    GetParameter(ParmSiteId, DataType.Integer, adminInfo.SiteId),
                    GetParameter(ParmDepartmentId, DataType.Integer, adminInfo.DepartmentId),
                    GetParameter(ParmAreaId, DataType.Integer, adminInfo.AreaId),
                    GetParameter(ParmDisplayname, DataType.VarChar, 255, adminInfo.DisplayName),
                    GetParameter(ParmEmail, DataType.VarChar, 255, adminInfo.Email),
                    GetParameter(ParmMobile, DataType.VarChar, 20, adminInfo.Mobile)
                };

                ExecuteNonQuery(SqlInsertUser, insertParms);

                DataProvider.DepartmentDao.UpdateCountOfAdmin();
                DataProvider.AreaDao.UpdateCountOfAdmin();

                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public bool ChangePassword(string userName, string password, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrEmpty(password))
            {
                errorMessage = "密码不能为空";
                return false;
            }
            if (password.Length < ConfigManager.SystemConfigInfo.AdminPasswordMinLength)
            {
                errorMessage = $"密码长度必须大于等于{ConfigManager.SystemConfigInfo.AdminPasswordMinLength}";
                return false;
            }
            if (
                !EUserPasswordRestrictionUtils.IsValid(password, ConfigManager.SystemConfigInfo.AdminPasswordRestriction))
            {
                errorMessage =
                    $"密码不符合规则，请包含{EUserPasswordRestrictionUtils.GetText(EUserPasswordRestrictionUtils.GetEnumType(ConfigManager.SystemConfigInfo.AdminPasswordRestriction))}";
                return false;
            }

            string passwordSalt;
            password = EncodePassword(password, EPasswordFormat.Encrypted, out passwordSalt);
            return ChangePassword(userName, EPasswordFormat.Encrypted, passwordSalt, password);
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

            if (adminInfo.IsLockedOut)
            {
                errorMessage = "此账号被锁定，无法登录";
                return false;
            }

            if (ConfigManager.SystemConfigInfo.IsAdminLockLogin)
            {
                if (adminInfo.CountOfFailedLogin > 0 &&
                    adminInfo.CountOfFailedLogin >= ConfigManager.SystemConfigInfo.AdminLockLoginCount)
                {
                    var lockType = EUserLockTypeUtils.GetEnumType(ConfigManager.SystemConfigInfo.AdminLockLoginType);
                    if (lockType == EUserLockType.Forever)
                    {
                        errorMessage = "此账号错误登录次数过多，已被永久锁定";
                        return false;
                    }
                    if (lockType == EUserLockType.Hours)
                    {
                        var ts = new TimeSpan(DateTime.Now.Ticks - adminInfo.LastActivityDate.Ticks);
                        var hours = Convert.ToInt32(ConfigManager.SystemConfigInfo.AdminLockLoginHours - ts.TotalHours);
                        if (hours > 0)
                        {
                            errorMessage =
                                $"此账号错误登录次数过多，已被锁定，请等待{hours}小时后重试";
                            return false;
                        }
                    }
                }
            }

            if (CheckPassword(password, isPasswordMd5, adminInfo.Password, EPasswordFormatUtils.GetEnumType(adminInfo.PasswordFormat), adminInfo.PasswordSalt))
                return true;

            errorMessage = "账号或密码不正确";
            return false;
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

        public bool CheckPassword(string password, bool isPasswordMd5, string dbpassword, EPasswordFormat passwordFormat,
            string passwordSalt)
        {
            var decodePassword = DecodePassword(dbpassword, passwordFormat, passwordSalt);
            if (isPasswordMd5)
            {
                return password == AuthUtils.Md5ByString(decodePassword);
            }
            return password == decodePassword;
        }

        public int ApiGetCount()
        {
            return DataProvider.DatabaseDao.GetCount(TableName);
        }

        public List<AdministratorInfo> ApiGetAdministrators(int offset, int limit)
        {
            var list = new List<AdministratorInfo>();
            List<AdministratorInfoDatabase> dbList;

            var sqlString =
                DataProvider.DatabaseDao.GetPageSqlString(TableName, "*", string.Empty, "ORDER BY Id", offset, limit);

            using (var connection = GetConnection())
            {
                dbList = connection.Query<AdministratorInfoDatabase>(sqlString).ToList();
            }

            if (dbList.Count > 0)
            {
                foreach (var dbInfo in dbList)
                {
                    if (dbInfo != null)
                    {
                        list.Add(dbInfo.ToAdministratorInfo());
                    }
                }
            }

            return list;
        }

        public AdministratorInfo ApiGetAdministrator(int id)
        {
            AdministratorInfo adminInfo = null;

            var sqlString = $"SELECT * FROM {TableName} WHERE Id = @Id";

            using (var connection = GetConnection())
            {
                var dbInfo = connection.QuerySingleOrDefault<AdministratorInfoDatabase>(sqlString, new { Id = id });
                if (dbInfo != null)
                {
                    adminInfo = dbInfo.ToAdministratorInfo();
                }
            }

            return adminInfo;
        }

        public bool ApiIsExists(int id)
        {
            var sqlString = $"SELECT count(1) FROM {TableName} WHERE Id = @Id";

            using (var connection = GetConnection())
            {
                return connection.ExecuteScalar<bool>(sqlString, new { Id = id });
            }
        }

        public AdministratorInfo ApiUpdate(int id, AdministratorInfoCreateUpdate adminInfoToUpdate, out string errorMessage)
        {
            var adminInfo = ApiGetAdministrator(id);

            if (!UpdateValidate(adminInfoToUpdate, adminInfo.UserName, adminInfo.Email, adminInfo.Mobile, out errorMessage)) return null;

            var dbUserInfo = new AdministratorInfoDatabase(adminInfo);

            adminInfoToUpdate.Load(dbUserInfo);

            dbUserInfo.Password = adminInfo.Password;
            dbUserInfo.PasswordFormat = adminInfo.PasswordFormat;
            dbUserInfo.PasswordSalt = adminInfo.PasswordSalt;

            using (var connection = GetConnection())
            {
                connection.Update(dbUserInfo);
            }

            return dbUserInfo.ToAdministratorInfo();
        }

        public AdministratorInfo ApiDelete(int id)
        {
            var adminInfoToDelete = ApiGetAdministrator(id);

            using (var connection = GetConnection())
            {
                connection.Delete(new AdministratorInfoDatabase(adminInfoToDelete));
            }

            return adminInfoToDelete;
        }

        public AdministratorInfo ApiInsert(AdministratorInfoCreateUpdate adminInfoToInsert, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                var dbAdminInfo = new AdministratorInfoDatabase();

                adminInfoToInsert.Load(dbAdminInfo);

                if (!InsertValidate(dbAdminInfo.UserName, dbAdminInfo.Password, dbAdminInfo.Email, dbAdminInfo.Mobile, out errorMessage)) return null;

                string passwordSalt;
                dbAdminInfo.Password = EncodePassword(dbAdminInfo.Password, EPasswordFormatUtils.GetEnumType(dbAdminInfo.PasswordFormat), out passwordSalt);
                dbAdminInfo.PasswordSalt = passwordSalt;
                dbAdminInfo.CreationDate = DateTime.Now;
                dbAdminInfo.LastActivityDate = DateTime.Now;

                using (var connection = GetConnection())
                {
                    var identity = connection.Insert(dbAdminInfo);
                    if (identity > 0)
                    {
                        dbAdminInfo.Id = Convert.ToInt32(identity);
                    }
                }

                return dbAdminInfo.ToAdministratorInfo();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return null;
            }
        }
    }
}
