using System.Collections.Generic;
using System.Data;
using SiteServer.CMS.Core;
using SiteServer.CMS.Data;
using SiteServer.CMS.Model;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Provider
{
    public class AdministratorsInRolesDao : DataProviderBase
    {
        public override string TableName => "siteserver_AdministratorsInRoles";

        public override List<TableColumn> TableColumns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(AdministratorsInRolesInfo.Id),
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumn
            {
                AttributeName = nameof(AdministratorsInRolesInfo.RoleName),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(AdministratorsInRolesInfo.UserName),
                DataType = DataType.VarChar,
                DataLength = 255
            }
        };

        public string[] GetRolesForUser(string userName)
        {
            var tmpRoleNames = string.Empty;
            var sqlString = "SELECT RoleName FROM siteserver_AdministratorsInRoles WHERE UserName = @UserName ORDER BY RoleName";
            var parms = new IDataParameter[]
            {
                GetParameter("@UserName", DataType.VarChar, 255, userName)
            };

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                while (rdr.Read())
                {
                    tmpRoleNames += GetString(rdr, 0) + ",";
                }
                rdr.Close();
            }

            if (tmpRoleNames.Length > 0)
            {
                tmpRoleNames = tmpRoleNames.Substring(0, tmpRoleNames.Length - 1);
                return tmpRoleNames.Split(',');
            }

            return new string[0];
        }

        public string[] GetUsersInRole(string roleName)
        {
            var tmpUserNames = string.Empty;
            var sqlString = "SELECT UserName FROM siteserver_AdministratorsInRoles WHERE RoleName = @RoleName ORDER BY userName";
            var parms = new IDataParameter[]
            {
                GetParameter("@RoleName", DataType.VarChar, 255, roleName)
            };

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                while (rdr.Read())
                {
                    tmpUserNames += GetString(rdr, 0) + ",";
                }
                rdr.Close();
            }

            if (tmpUserNames.Length > 0)
            {
                tmpUserNames = tmpUserNames.Substring(0, tmpUserNames.Length - 1);
                return tmpUserNames.Split(',');
            }

            return new string[0];
        }

        public void RemoveUserFromRoles(string userName, string[] roleNames)
        {
            var sqlString = "DELETE FROM siteserver_AdministratorsInRoles WHERE UserName = @UserName AND RoleName = @RoleName";
            foreach (var roleName in roleNames)
            {
                var parms = new IDataParameter[]
                {
                    GetParameter("@UserName", DataType.VarChar, 255, userName),
                    GetParameter("@RoleName", DataType.VarChar, 255, roleName)
                };
                ExecuteNonQuery(sqlString, parms);
            }
        }

        public void RemoveUserFromRole(string userName, string roleName)
        {
            var sqlString = "DELETE FROM siteserver_AdministratorsInRoles WHERE UserName = @UserName AND RoleName = @RoleName";
            var parms = new IDataParameter[]
            {
                GetParameter("@UserName", DataType.VarChar, 255, userName),
                GetParameter("@RoleName", DataType.VarChar, 255, roleName)
            };

            ExecuteNonQuery(sqlString, parms);
        }

        public string[] FindUsersInRole(string roleName, string userNameToMatch)
        {
            var tmpUserNames = string.Empty;
            string sqlString =
                $"SELECT UserName FROM siteserver_AdministratorsInRoles WHERE RoleName = @RoleName AND UserName LIKE '%{AttackUtils.FilterSql(userNameToMatch)}%'";

            var parms = new IDataParameter[]
            {
                GetParameter("@RoleName", DataType.VarChar, 255, roleName)
            };

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                while (rdr.Read())
                {
                    tmpUserNames += GetString(rdr, 0) + ",";
                }
                rdr.Close();
            }

            if (tmpUserNames.Length > 0)
            {
                tmpUserNames = tmpUserNames.Substring(0, tmpUserNames.Length - 1);
                return tmpUserNames.Split(',');
            }

            return new string[0];
        }

        public bool IsUserInRole(string userName, string roleName)
        {
            var isUserInRole = false;
            const string sqlString = "SELECT * FROM siteserver_AdministratorsInRoles WHERE UserName = @UserName AND RoleName = @RoleName";
            var parms = new IDataParameter[]
            {
                GetParameter("@UserName", DataType.VarChar, 255, userName),
                GetParameter("@RoleName", DataType.VarChar, 255, roleName)
            };
            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    if (!rdr.IsDBNull(0))
                    {
                        isUserInRole = true;
                    }
                }
                rdr.Close();
            }
            return isUserInRole;
        }

        public void AddUserToRoles(string userName, string[] roleNames)
        {
            foreach (var roleName in roleNames)
            {
                AddUserToRole(userName, roleName);
            }
        }

        public void AddUserToRole(string userName, string roleName)
        {
            if (!DataProvider.AdministratorDao.IsUserNameExists(userName)) return;
            if (!IsUserInRole(userName, roleName))
            {
                var sqlString = "INSERT INTO siteserver_AdministratorsInRoles (UserName, RoleName) VALUES (@UserName, @RoleName)";

                var parms = new IDataParameter[]
                {
                    GetParameter("@UserName", DataType.VarChar, 255, userName),
                    GetParameter("@RoleName", DataType.VarChar, 255, roleName)
                };

                ExecuteNonQuery(sqlString, parms);
            }
        }
    }
}
