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
using SiteServer.CMS.DataCache;
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
                AttributeName = nameof(AdministratorInfoDatabase.Mobile),
                DataType = DataType.VarChar,
                DataLength = 20
            },
            new TableColumn
            {
                AttributeName = nameof(AdministratorInfoDatabase.Email),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(AdministratorInfoDatabase.AvatarUrl),
                DataType = DataType.VarChar,
                DataLength = 200
            }
        };

        private const string SqlSelectUserByUserName =
            "SELECT Id, UserName, Password, PasswordFormat, PasswordSalt, CreationDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, CreatorUserName, IsLockedOut, SiteIdCollection, SiteId, DepartmentId, AreaId, DisplayName, Mobile, Email, AvatarUrl FROM siteserver_Administrator WHERE UserName = @UserName";

        private const string SqlSelectUserByUserId =
            "SELECT Id, UserName, Password, PasswordFormat, PasswordSalt, CreationDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, CreatorUserName, IsLockedOut, SiteIdCollection, SiteId, DepartmentId, AreaId, DisplayName, Mobile, Email, AvatarUrl FROM siteserver_Administrator WHERE Id = @Id";

        private const string SqlSelectUserByEmail =
            "SELECT Id, UserName, Password, PasswordFormat, PasswordSalt, CreationDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, CreatorUserName, IsLockedOut, SiteIdCollection, SiteId, DepartmentId, AreaId, DisplayName, Mobile, Email, AvatarUrl FROM siteserver_Administrator WHERE Email = @Email";

        private const string SqlSelectUserByMobile =
            "SELECT Id, UserName, Password, PasswordFormat, PasswordSalt, CreationDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, CreatorUserName, IsLockedOut, SiteIdCollection, SiteId, DepartmentId, AreaId, DisplayName, Mobile, Email, AvatarUrl FROM siteserver_Administrator WHERE Mobile = @Mobile";

        private const string SqlSelectUsername = "SELECT UserName FROM siteserver_Administrator WHERE UserName = @UserName";

        private const string SqlSelectUsernameByEmail =
            "SELECT UserName FROM siteserver_Administrator WHERE Email = @Email";

        private const string SqlSelectUsernameByMobile =
            "SELECT UserName FROM siteserver_Administrator WHERE Mobile = @Mobile";

        private const string SqlInsertUser =
            "INSERT INTO siteserver_Administrator (UserName, Password, PasswordFormat, PasswordSalt, CreationDate, LastActivityDate, CountOfLogin, CountOfFailedLogin, CreatorUserName, IsLockedOut, SiteIdCollection, SiteId, DepartmentId, AreaId, DisplayName, Mobile, Email, AvatarUrl) VALUES (@UserName, @Password, @PasswordFormat, @PasswordSalt, @CreationDate, @LastActivityDate, @CountOfLogin, @CountOfFailedLogin, @CreatorUserName, @IsLockedOut, @SiteIdCollection, @SiteId, @DepartmentId, @AreaId, @DisplayName, @Mobile, @Email, @AvatarUrl)";

        private const string SqlUpdateUser =
            "UPDATE siteserver_Administrator SET LastActivityDate = @LastActivityDate, CountOfLogin = @CountOfLogin, CountOfFailedLogin = @CountOfFailedLogin, IsLockedOut = @IsLockedOut, SiteIdCollection = @SiteIdCollection, SiteId = @SiteId, DepartmentId = @DepartmentId, AreaId = @AreaId, DisplayName = @DisplayName, Mobile = @Mobile, Email = @Email, AvatarUrl = @AvatarUrl WHERE UserName = @UserName";

        private const string ParmId = "@Id";
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
        private const string ParmMobile = "@Mobile";
        private const string ParmEmail = "@Email";
        private const string ParmAvatarUrl = "@AvatarUrl";

        public void Update(AdministratorInfo info)
        {
            info.DisplayName = AttackUtils.FilterXss(info.DisplayName);
            info.Mobile = AttackUtils.FilterXss(info.Mobile);
            info.Email = AttackUtils.FilterXss(info.Email);

            IDataParameter[] parameters =
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
                GetParameter(ParmMobile, DataType.VarChar, 20, info.Mobile),
                GetParameter(ParmEmail, DataType.VarChar, 255, info.Email),
                GetParameter(ParmAvatarUrl, DataType.VarChar, 200, info.AvatarUrl),
                GetParameter(ParmUsername, DataType.VarChar, 255, info.UserName)
            };

            ExecuteNonQuery(SqlUpdateUser, parameters);

            DataProvider.DepartmentDao.UpdateCountOfAdmin();
            DataProvider.AreaDao.UpdateCountOfAdmin();

            AdminManager.UpdateCache(info);
        }

        public void UpdateLastActivityDateAndCountOfFailedLogin(AdministratorInfo adminInfo)
        {
            if (adminInfo == null) return;

            adminInfo.LastActivityDate = DateTime.Now;
            adminInfo.CountOfFailedLogin += 1;

            var sqlString = $"UPDATE {TableName} SET LastActivityDate = @LastActivityDate, CountOfFailedLogin = @CountOfFailedLogin WHERE Id = @Id";

            IDataParameter[] parameters =
            {
                GetParameter(ParmLastActivityDate, DataType.DateTime, adminInfo.LastActivityDate),
                GetParameter(ParmCountOfFailedLogin, DataType.Integer, adminInfo.CountOfFailedLogin),
                GetParameter(ParmId, DataType.Integer, adminInfo.Id)
            };

            ExecuteNonQuery(sqlString, parameters);

            AdminManager.UpdateCache(adminInfo);
        }

        public void UpdateLastActivityDateAndCountOfLogin(AdministratorInfo adminInfo)
        {
            if (adminInfo == null) return;

            adminInfo.LastActivityDate = DateTime.Now;
            adminInfo.CountOfLogin += 1;
            adminInfo.CountOfFailedLogin = 0;

            var sqlString =
                $"UPDATE {TableName} SET LastActivityDate = @LastActivityDate, CountOfLogin = @CountOfLogin, CountOfFailedLogin = @CountOfFailedLogin WHERE Id = @Id";

            IDataParameter[] parameters =
            {
                GetParameter(ParmLastActivityDate, DataType.DateTime, adminInfo.LastActivityDate),
                GetParameter(ParmCountOfLogin, DataType.Integer, adminInfo.CountOfLogin),
                GetParameter(ParmCountOfFailedLogin, DataType.Integer, adminInfo.CountOfFailedLogin),
                GetParameter(ParmId, DataType.Integer, adminInfo.Id)
            };

            ExecuteNonQuery(sqlString, parameters);

            AdminManager.UpdateCache(adminInfo);
        }

        public void UpdateSiteIdCollection(AdministratorInfo adminInfo, string siteIdCollection)
        {
            if (adminInfo == null) return;

            adminInfo.SiteIdCollection = siteIdCollection;

            var sqlString = $"UPDATE {TableName} SET SiteIdCollection = @SiteIdCollection WHERE Id = @Id";

            IDataParameter[] parameters =
            {
                GetParameter(ParmSiteIdCollection, DataType.VarChar, 50, adminInfo.SiteIdCollection),
                GetParameter(ParmId, DataType.Integer, adminInfo.Id)
            };

            ExecuteNonQuery(sqlString, parameters);

            AdminManager.UpdateCache(adminInfo);
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

                var sqlString =
                    $"UPDATE {TableName} SET SiteIdCollection = @SiteIdCollection, SiteId = @SiteId WHERE Id = @Id";

                IDataParameter[] parameters =
                {
                    GetParameter(ParmSiteIdCollection, DataType.VarChar, 50, adminInfo.SiteIdCollection),
                    GetParameter(ParmSiteId, DataType.Integer, adminInfo.SiteId),
                    GetParameter(ParmId, DataType.Integer, adminInfo.Id)
                };

                ExecuteNonQuery(sqlString, parameters);

                AdminManager.UpdateCache(adminInfo);
            }

            return siteIdListLatestAccessed;
        }

        private void ChangePassword(AdministratorInfo adminInfo, EPasswordFormat passwordFormat, string passwordSalt,
            string password)
        {
            adminInfo.Password = password;
            adminInfo.PasswordFormat = EPasswordFormatUtils.GetValue(passwordFormat);
            adminInfo.PasswordSalt = passwordSalt;

            var sqlString =
                $"UPDATE {TableName} SET Password = @Password, PasswordFormat = @PasswordFormat, PasswordSalt = @PasswordSalt WHERE Id = @Id";

            IDataParameter[] parameters =
            {
                GetParameter(ParmPassword, DataType.VarChar, 255, adminInfo.Password),
                GetParameter(ParmPasswordFormat, DataType.VarChar, 50, adminInfo.PasswordFormat),
                GetParameter(ParmPasswordSalt, DataType.VarChar, 128, adminInfo.PasswordSalt),
                GetParameter(ParmId, DataType.Integer, adminInfo.Id)
            };

            ExecuteNonQuery(sqlString, parameters);

            AdminManager.RemoveCache(adminInfo);
        }

        public void Delete(AdministratorInfo adminInfo)
        {
            if (adminInfo == null) return;

            var sqlString = $"DELETE FROM {TableName} WHERE Id = @Id";

            IDataParameter[] parameters =
            {
                GetParameter(ParmId, DataType.Integer, adminInfo.Id)
            };

            ExecuteNonQuery(sqlString, parameters);

            AdminManager.RemoveCache(adminInfo);

            DataProvider.DepartmentDao.UpdateCountOfAdmin();
            DataProvider.AreaDao.UpdateCountOfAdmin();
        }

        public void Lock(List<string> userNameList)
        {
            var sqlString =
                $"UPDATE {TableName} SET IsLockedOut = '{true}' WHERE UserName IN ({TranslateUtils.ToSqlInStringWithQuote(userNameList)})";

            ExecuteNonQuery(sqlString);

            AdminManager.ClearCache();
        }

        public void UnLock(List<string> userNameList)
        {
            var sqlString =
                $"UPDATE {TableName} SET IsLockedOut = '{false}', CountOfFailedLogin = 0 WHERE UserName IN ({TranslateUtils.ToSqlInStringWithQuote(userNameList)})";

            ExecuteNonQuery(sqlString);

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
            if (userId <= 0) return null;

            AdministratorInfo info = null;

            IDataParameter[] parameters =
            {
                GetParameter(ParmId, DataType.Integer, userId)
            };

            using (var rdr = ExecuteReader(SqlSelectUserByUserId, parameters))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    info = new AdministratorInfo(GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++),
                        GetString(rdr, i++), GetString(rdr, i++), GetDateTime(rdr, i++), GetDateTime(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++),
                        GetString(rdr, i++), TranslateUtils.ToBool(GetString(rdr, i++)), GetString(rdr, i++),
                        GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++),
                        GetString(rdr, i));
                }
                rdr.Close();
            }

            return info;
        }

        public AdministratorInfo GetByUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return null;

            AdministratorInfo info = null;

            IDataParameter[] parameters =
            {
                GetParameter(ParmUsername, DataType.VarChar, 255, userName)
            };

            using (var rdr = ExecuteReader(SqlSelectUserByUserName, parameters))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    info = new AdministratorInfo(GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++),
                        GetString(rdr, i++), GetString(rdr, i++), GetDateTime(rdr, i++), GetDateTime(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++),
                        GetString(rdr, i++), TranslateUtils.ToBool(GetString(rdr, i++)), GetString(rdr, i++),
                        GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++),
                        GetString(rdr, i));
                }
                rdr.Close();
            }

            return info;
        }

        public AdministratorInfo GetByMobile(string mobile)
        {
            if (string.IsNullOrEmpty(mobile)) return null;

            AdministratorInfo info = null;

            IDataParameter[] parameters =
            {
                GetParameter(ParmMobile, DataType.VarChar, 50, mobile)
            };

            using (var rdr = ExecuteReader(SqlSelectUserByMobile, parameters))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    info = new AdministratorInfo(GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++),
                        GetString(rdr, i++), GetString(rdr, i++), GetDateTime(rdr, i++), GetDateTime(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++),
                        GetString(rdr, i++), TranslateUtils.ToBool(GetString(rdr, i++)), GetString(rdr, i++),
                        GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++),
                        GetString(rdr, i));
                }
                rdr.Close();
            }

            return info;
        }

        public AdministratorInfo GetByEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return null;

            AdministratorInfo info = null;

            IDataParameter[] parameters =
            {
                GetParameter(ParmEmail, DataType.VarChar, 50, email)
            };

            using (var rdr = ExecuteReader(SqlSelectUserByEmail, parameters))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    info = new AdministratorInfo(GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++),
                        GetString(rdr, i++), GetString(rdr, i++), GetDateTime(rdr, i++), GetDateTime(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++),
                        GetString(rdr, i++), TranslateUtils.ToBool(GetString(rdr, i++)), GetString(rdr, i++),
                        GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++),
                        GetString(rdr, i));
                }
                rdr.Close();
            }

            return info;
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

                var filterSearchWord = AttackUtils.FilterSql(searchWord);
                whereBuilder.Append(
                    $"(UserName LIKE '%{filterSearchWord}%' OR EMAIL LIKE '%{filterSearchWord}%' OR DisplayName LIKE '%{filterSearchWord}%')");
            }

            if (!isConsoleAdministrator)
            {
                if (whereBuilder.Length > 0)
                {
                    whereBuilder.Append(" AND ");
                }
                whereBuilder.Append($"CreatorUserName = '{AttackUtils.FilterSql(creatorUserName)}'");
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
                    $"WHERE (UserName IN (SELECT UserName FROM {DataProvider.AdministratorsInRolesDao.TableName} WHERE RoleName = '{AttackUtils.FilterSql(roleName)}')) {whereString}";
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

        public bool IsUserNameExists(string adminName)
        {
            if (string.IsNullOrEmpty(adminName))
            {
                return false;
            }

            var exists = false;

            IDataParameter[] parameters =
            {
                GetParameter(ParmUsername, DataType.VarChar, 255, adminName)
            };

            using (var rdr = ExecuteReader(SqlSelectUsername, parameters))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    exists = true;
                }
                rdr.Close();
            }
            return exists;
        }

        public bool IsEmailExists(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;

            var exists = IsUserNameExists(email);
            if (exists) return true;

            var sqlSelect = $"SELECT {nameof(AdministratorInfoDatabase.Email)} FROM {TableName} WHERE {nameof(AdministratorInfoDatabase.Email)} = @{nameof(AdministratorInfoDatabase.Email)}";

            var parameters = new IDataParameter[]
            {
                GetParameter(ParmEmail, DataType.VarChar, 200, email)
            };

            using (var rdr = ExecuteReader(sqlSelect, parameters))
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

            var sqlString = $"SELECT {nameof(AdministratorInfoDatabase.Mobile)} FROM {TableName} WHERE {nameof(AdministratorInfoDatabase.Mobile)} = @{nameof(AdministratorInfoDatabase.Mobile)}";

            var parameters = new IDataParameter[]
            {
                GetParameter(ParmMobile, DataType.VarChar, 20, mobile)
            };

            using (var rdr = ExecuteReader(sqlString, parameters))
            {
                if (rdr.Read())
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

            IDataParameter[] parameters =
            {
                GetParameter(ParmEmail, DataType.VarChar, 50, email)
            };

            using (var rdr = ExecuteReader(SqlSelectUsernameByEmail, parameters))
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

            IDataParameter[] parameters =
            {
                GetParameter(ParmMobile, DataType.VarChar, 50, mobile)
            };

            using (var rdr = ExecuteReader(SqlSelectUsernameByMobile, parameters))
            {
                if (rdr.Read())
                {
                    userName = GetString(rdr, 0);
                }
                rdr.Close();
            }
            return userName;
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

        public List<string> GetUserNameList()
        {
            var list = new List<string>();
            var sqlSelect = $"SELECT UserName FROM {TableName}";

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
            var sqlSelect = $"SELECT UserName FROM {TableName} WHERE DepartmentId = {departmentId}";
            if (isAll)
            {
                var departmentIdList = DataProvider.DepartmentDao.GetIdListForDescendant(departmentId);
                departmentIdList.Add(departmentId);
                sqlSelect =
                    $"SELECT UserName FROM {TableName} WHERE DepartmentId IN ({TranslateUtils.ObjectCollectionToString(departmentIdList)})";
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

        private string EncodePassword(string password, EPasswordFormat passwordFormat, out string passwordSalt)
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

        private string GenerateSalt()
        {
            var data = new byte[0x10];
            new RNGCryptoServiceProvider().GetBytes(data);
            return Convert.ToBase64String(data);
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
            if (userName.Length < ConfigManager.SystemConfigInfo.AdminUserNameMinLength)
            {
                errorMessage = $"用户名长度必须大于等于{ConfigManager.SystemConfigInfo.AdminUserNameMinLength}";
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

        public bool Insert(AdministratorInfo adminInfo, out string errorMessage)
        {
            if (!InsertValidate(adminInfo.UserName, adminInfo.Password, adminInfo.Email, adminInfo.Mobile, out errorMessage)) return false;

            try
            {
                adminInfo.LastActivityDate = DateUtils.SqlMinValue;
                adminInfo.CreationDate = DateTime.Now;
                adminInfo.PasswordFormat = EPasswordFormatUtils.GetValue(EPasswordFormat.Encrypted);
                adminInfo.Password = EncodePassword(adminInfo.Password, EPasswordFormatUtils.GetEnumType(adminInfo.PasswordFormat), out var passwordSalt);
                adminInfo.PasswordSalt = passwordSalt;

                adminInfo.DisplayName = AttackUtils.FilterXss(adminInfo.DisplayName);
                adminInfo.Email = AttackUtils.FilterXss(adminInfo.Email);
                adminInfo.Mobile = AttackUtils.FilterXss(adminInfo.Mobile);

                IDataParameter[] parameters =
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
                    GetParameter(ParmMobile, DataType.VarChar, 20, adminInfo.Mobile),
                    GetParameter(ParmEmail, DataType.VarChar, 255, adminInfo.Email),
                    GetParameter(ParmAvatarUrl, DataType.VarChar, 200, adminInfo.AvatarUrl)
                };

                ExecuteNonQuery(SqlInsertUser, parameters);

                DataProvider.DepartmentDao.UpdateCountOfAdmin();
                DataProvider.AreaDao.UpdateCountOfAdmin();

                var roles = new[] { EPredefinedRoleUtils.GetValue(EPredefinedRole.Administrator) };
                DataProvider.AdministratorsInRolesDao.AddUserToRoles(adminInfo.UserName, roles);

                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public bool ChangePassword(AdministratorInfo adminInfo, string password, out string errorMessage)
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

            errorMessage = "账号或密码错误";
            return false;
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

                dbAdminInfo.Password = EncodePassword(dbAdminInfo.Password, EPasswordFormatUtils.GetEnumType(dbAdminInfo.PasswordFormat), out var passwordSalt);
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
